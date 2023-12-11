using Application.InfraInterfaces;
using Core.Entities;
using Infrastructure.Contexts;

namespace Infrastructure.Repos;

 public class TimeMapRepository : ITimeMapRepository
    {
        private readonly DbContextManagement _context;

        public TimeMapRepository(DbContextManagement context)
        {
            _context = context;
        }

        public string SignIn(int employeeId)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.EmployeeId == employeeId);

            if (employee == null)
                return "Employee not found";

            // Check if the user has already signed in for the day
            if (_context.TimeMaps.Any(t => t.EmployeeId == employeeId && t.SignInTime.Date == DateTime.Now.Date))
            {
                // User has already signed in
                return "Already signed in";
            }

            // If the user has signed in before but hasn't signed out, add 8 hours
            var lastSignIn = _context.TimeMaps
                .Where(t => t.EmployeeId == employeeId && t.SignInTime.Date < DateTime.Now.Date)
                .OrderByDescending(t => t.SignInTime)
                .FirstOrDefault();

            if (lastSignIn != null && lastSignIn.SignOutTime == default)
            {
                lastSignIn.SignOutTime = lastSignIn.SignInTime.AddHours(8);
            }

            // Record the new sign-in
            var timeMapEntry = new TimeMap
            {
                EmployeeId = employeeId,
                SignInTime = DateTime.Now
            };

            _context.TimeMaps.Add(timeMapEntry);
            _context.SaveChanges(); 

            return "Signed in successfully";
        }

        public string SignOut(int employeeId)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.EmployeeId == employeeId);

            if (employee == null)
                return "Employee not found";

            // Check if the user has already signed out for the day
            if (_context.TimeMaps.Any(t => t.EmployeeId == employeeId && t.SignOutTime != default && t.SignOutTime.Date == DateTime.Now.Date))
            {
                // User has already signed out
                return "Already signed out";
            }

            // Find the latest sign-in for the user
            var lastSignIn = _context.TimeMaps
                .Where(t => t.EmployeeId == employeeId && t.SignInTime.Date == DateTime.Now.Date)
                .OrderByDescending(t => t.SignInTime)
                .FirstOrDefault();

            if (lastSignIn != null)
            {
                // If the user hasn't signed out yet, sign them out and calculate worked hours
                if (lastSignIn.SignOutTime == default)
                {
                    lastSignIn.SignOutTime = DateTime.Now;
                    lastSignIn.WorkedHours = (lastSignIn.SignOutTime - lastSignIn.SignInTime).TotalHours;
                    _context.SaveChanges(); 
                    return "Signed out successfully";
                }
            }

            return "Not signed in today";
        }
    }
