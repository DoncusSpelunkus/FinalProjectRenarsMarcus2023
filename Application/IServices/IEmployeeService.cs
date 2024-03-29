﻿
using Application.Dtos;

namespace Application.IServices;

public interface IEmployeeService
{
    Task<UserDto> CreateEmployee(UserDto userDto);
    Task<UserDto> GetEmployeeById(int employeeId);
    Task<List<UserDto>> GetEmployeesByWarehouseId(int warehouseId);
    Task<UserDto> UpdateEmployee(UserDto userDto);
    Task<bool> DeleteEmployee(int employeeId);
    Task<bool> UserExists(string username, string email);
    Task<string> LoginAsync(LoginDto loginDto);
    Task<bool> UpdatePassword(int id, string oldPassword, string newPassword); // for initial confirmation
    Task<bool> ResetPassword(string email); // for password reset to automatic password
    void sendEmailToSupport(string contactEmail, string description);
    void CreateDB();
}