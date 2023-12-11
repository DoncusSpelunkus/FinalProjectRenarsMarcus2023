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

    private readonly UserDtoValidator _userDtoVal;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public EmployeeService(IEmployeeRepository employeeRepository, ITokenService tokenService, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _tokenService = tokenService;
        _loginVal = new LoginValidator();
        _mapper = mapper;
    }

    public async Task<UserDto> CreateEmployee(UserDto userDto)
    {


        if (await UserExists(userDto.Username))
        {
            throw new ApplicationException("User already exists");
        }

        using var hmac = new HMACSHA512();
        
        var employee = _mapper.Map<Employee>(userDto);

        employee.Name = employee.Name.ToLower();
        employee.Username = employee.Username.ToLower();
        employee.PasswordHash  = hmac.ComputeHash(Encoding.UTF8.GetBytes(userDto.Password));
        employee.PasswordSalt = hmac.Key;

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

        using var hmac = new HMACSHA512();
        string password = getRandomPassword();
        var employee = _mapper.Map<Employee>(userDto);
        employee.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        employee.PasswordSalt = hmac.Key; 
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

    public async Task<UserDto> LoginAsync(LoginDto loginDto)
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

        var userdto = _mapper.Map<UserDto>(user);

        userdto.Token = token;
    
        return userdto;

    }

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