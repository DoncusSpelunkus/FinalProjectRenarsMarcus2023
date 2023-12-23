using System.Threading.Tasks;
using Core.Entities;

namespace Application.InfraInterfaces;

public interface IEmployeeRepository
{
    Task<Employee> CreateEmployee(Employee employee );

    Task<Employee> GetEmployeeById(int employeeId);
    
    Task<List<Employee>> GetEmployeesByWarehouseId(int warehouseId);


    Task<Employee> UpdateEmployee(Employee employee);

    Task<bool> DeleteEmployee(int employeeId);
    
    Task<bool> UserExists(string username, string email);
    
    Task<Employee> GetUserByUsernameAsync(string username);

    Task<Employee> GetUserByEmailAsync (string email);

    void CreateDB();

}