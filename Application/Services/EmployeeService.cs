using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos;
using Application.InfraInterfaces;
using Application.IServices;
using Application.Validators;
using AutoMapper;
using Core.Entities;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;

namespace Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly LoginValidator _loginVal;
    private readonly PasswordValidator _passwordVal;
    private readonly UserDtoValidator _userDtoVal;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;

    public EmployeeService(IEmployeeRepository employeeRepository, ITokenService tokenService, IMapper mapper,IEmailService emailService)
    {
        _employeeRepository = employeeRepository;
        _tokenService = tokenService;
        _loginVal = new LoginValidator();
        _passwordVal = new PasswordValidator();
        _userDtoVal =  new UserDtoValidator();
        _mapper = mapper;
        _emailService = emailService;
    }

    public async Task<UserDto> CreateEmployee(UserDto userDto)
    {
        if (await UserExists(userDto.Username))
        {
            throw new ApplicationException("User already exists");
        }

        var validation = _userDtoVal.Validate(userDto);

        if(!validation.IsValid){
            Console.WriteLine(validation.ToString());
            throw new ApplicationException("Invalid user data: " + validation);
        }
        

        Console.WriteLine("About to send");

        var password = userDto.Password ?? getRandomPassword(); // Allows us to still manually set the password for testing purposes
        
        string email = "renarsmednieks13@gmail.com";
        
        // _emailService.SendTemporaryCredentials(email,password); 

        var passwordValidation = _passwordVal.Validate(password);

        if(!passwordValidation.IsValid){
            Console.WriteLine(passwordValidation.ToString());
            throw new ApplicationException("Invalid user data: " + passwordValidation);
        }

        var employee = _mapper.Map<Employee>(userDto);
        
        employee.Name = employee.Name.ToLower();
        employee.Username = employee.Username.ToLower();
        var hashAndSalt = CreatePasswordHash(password);
        employee.PasswordHash = hashAndSalt.Hash;
        employee.PasswordSalt = hashAndSalt.Salt;

        var registeredEmployee = await _employeeRepository.CreateEmployee(employee);

        return _mapper.Map<UserDto>(registeredEmployee);
    }

    public async Task<List<UserDto>> GetEmployeesByWarehouseId(int warehouseId)
    {
        var employees = await _employeeRepository.GetEmployeesByWarehouseId(warehouseId);
        return _mapper.Map<List<UserDto>>(employees);
    }

    public async Task<UserDto> GetEmployeeById(int employeeId)
    {
        var employee = await _employeeRepository.GetEmployeeById(employeeId);
        return _mapper.Map<UserDto>(employee);
    }

    public async Task<UserDto> UpdateEmployee(UserDto userDto)
    {
        var validation = _userDtoVal.Validate(userDto);

        if(!validation.IsValid){
            throw new ApplicationException("Invalid user data: " + validation);
        }
        var hashAndSalt = CreatePasswordHash(getRandomPassword());
        var employee = _mapper.Map<Employee>(userDto);
        employee.PasswordHash = hashAndSalt.Hash;
        employee.PasswordSalt = hashAndSalt.Salt;
        var updatedEmployee = await _employeeRepository.UpdateEmployee(employee);
        return _mapper.Map<UserDto>(updatedEmployee);
    }

    public async Task<bool> DeleteEmployee(int employeeId)
    {
        return await _employeeRepository.DeleteEmployee(employeeId);
    }

    public async Task<bool> UserExists(string username)
    {
        return await _employeeRepository.UserExists(username);
    }

    public async Task<string> LoginAsync(LoginDto loginDto)
    {
        var validation = _loginVal.Validate(loginDto);

        if (!validation.IsValid)
        {
            throw new ApplicationException("Validation failed: " + validation);
        }

        var user = await _employeeRepository.GetUserByUsernameAsync(loginDto.Username);

        if (user == null)
        {
            throw new ApplicationException("User not found");
        }

        if (!VerifyPasswordHash(user, loginDto.Password))
        {
            throw new ApplicationException("Invalid password");
        }

        var token = _tokenService.CreateToken(user);
        
        return token;

    }
    
    public async Task<bool> UpdatePassword(int employeeId, string oldPassword, string newPassword)
    {
        var employee = await _employeeRepository.GetEmployeeById(employeeId) ?? throw new ApplicationException("User not found");

        if (!VerifyPasswordHash(employee, oldPassword))
        {
            throw new ApplicationException("Invalid password");
        }

        var validation = _passwordVal.Validate(newPassword);

        if (!validation.IsValid)
        {
            throw new ApplicationException("Validation failed: " + validation);
        }
        
        var hashAndSalt = CreatePasswordHash(newPassword);
        employee.PasswordHash = hashAndSalt.Hash;
        employee.PasswordSalt = hashAndSalt.Salt;

        try
        {
            await _employeeRepository.UpdateEmployee(employee);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new ApplicationException("Error updating password");
        }
        return true;
    }

   /* public async Task<bool> ResetPassword(string email) // requires mail system
    {
        var employee = await _employeeRepository.GetEmployeeById(employeeId) ?? throw new ApplicationException("User not found");

        if (employee.Email != email)
        {
            throw new ApplicationException("Invalid email");
        }

        var newPassword = getRandomPassword();

        var hashAndSalt = CreatePasswordHash(newPassword);
        employee.PasswordHash = hashAndSalt.Hash;
        employee.PasswordSalt = hashAndSalt.Salt;

        try
        {
            await _employeeRepository.UpdateEmployee(employee);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new ApplicationException("Error updating password");
        }
        return true;
    } */

    public void CreateDB()
    {
        _employeeRepository.CreateDB();
    }

    private bool VerifyPasswordHash(Employee user, string password)
    {
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i])
            {
                return false;
            }
        }

        return true;
    }

    private HashAndSalt CreatePasswordHash(string password)
    {
        using var hmac = new HMACSHA512();
        var Hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        var Salt = hmac.Key;
        var hashAndSalt = new HashAndSalt(Hash, Salt);
        return hashAndSalt;
    }

    private string getRandomPassword(){
        int passwordLen = new Random().Next(8, 15);
        int first = new Random().Next(1, 3);
        int second = new Random().Next(4, 5);
        int third = new Random().Next(5, 8);
        string password = "";
        char toAdd;

        int lastAdd = 1;

        for (int i = 0; i < passwordLen; i++)
        {

            toAdd = Convert.ToChar(new Random().Next(0, 26) + 97); // a-z
            
            int fiftyfifty = new Random().Next(0, 1);

            if (fiftyfifty == 1 || i == first || i == second || i == third)
            {
                if (lastAdd == 1)
                {
                    toAdd = Convert.ToChar(new Random().Next(0, 5) + 33); // 33 = '!'
                }

                if(lastAdd == 2){
                    toAdd = Convert.ToChar(new Random().Next(0, 26) + 65); // A-Z

                }
                if(lastAdd == 3){
                    toAdd = Convert.ToChar(new Random().Next(0, 9) + 48); // 0-9
                }

                if (lastAdd == 3)
                {
                    lastAdd = 1;
                }
                else
                {
                    lastAdd += 1;
                }
            }

            password += toAdd;
        }
        return password;
    }
}

internal class HashAndSalt
{
    public byte[] Hash { get; set; }
    public byte[] Salt { get; set; }

    public HashAndSalt(byte[] hash, byte[] salt)
    {
        Hash = hash;
        Salt = salt;
    }
}