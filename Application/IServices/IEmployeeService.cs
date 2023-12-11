using System.Threading.Tasks;
using Application.Dtos;
using Core.Entities;

namespace Application.IServices;

public interface IEmployeeService
{
    Task<UserDto> CreateEmployee(RegisterDto registerDto);
    Task<UserDto> GetEmployeeById(int employeeId);
    Task<List<UserDto>> GetEmployeesByWarehouseId(int warehouseId);
    Task<UserDto> UpdateEmployee(UserDto userDto);
    Task<bool> DeleteEmployee(int employeeId);
    Task<bool> UserExists(string username);
    Task<UserDto> LoginAsync(LoginDto loginDto);
    void CreateDB();
}