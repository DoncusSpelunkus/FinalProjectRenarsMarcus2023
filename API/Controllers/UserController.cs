using System.Threading.Tasks;
using Application.Dtos;
using Application.IServices;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using sockets;

namespace API.Controllers;

[ApiController]
[Route("[Controller]")]
public class UserController : ControllerBase
{
    private readonly IEmployeeService _service;
    private readonly IHubContext<UserManagementSocket> _hubContext;

    public UserController(IEmployeeService service, IHubContext<UserManagementSocket> hubContext)
    {
        _service = service;
        _hubContext = hubContext;
    }

    [HttpGet("createDb")]
    public string CreateDb()
    {
        _service.CreateDB();
        return "DB build complete :D";
    }

    // [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(UserDto receivedUser)
    {
        try
        {
            var userDto = await _service.CreateEmployee(receivedUser);

            if (userDto == null)
            {
                return BadRequest("User is Taken");
            }

            TriggerGetAllUsers(userDto.WarehouseId); 

            return userDto;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in register" + e);
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        try
        {
            loginDto.Username = loginDto.Username.ToLower();
            
            var userDto = await _service.LoginAsync(loginDto);



            if (userDto == null)
            {
                return Unauthorized("Invalid username or password");
            }

            return userDto;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in login" + e);
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("GetAllByWareHouseId/{id}")]
    public async Task<ActionResult<List<UserDto>>> GetAllByWareHouseId(int id)
    {
        try
        {
            var users = await _service.GetEmployeesByWarehouseId(id);

            if (users == null)
            {
                return BadRequest("No employees found");
            }

            return users;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in GetAllByWareHouseId" + e);
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("GetById/{id}")]
    public async Task<ActionResult<UserDto>> GetEmployeeById(int id)
    {
        try
        {
            var user = await _service.GetEmployeeById(id);

            if (user == null)
            {
                return BadRequest("No employee found");
            }

            return user;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in GetEmployeeById" + e);
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("Update")] // Should take an id in param?
    public async Task<ActionResult<UserDto>> UpdateEmployee(UserDto userDto)
    {
        try
        {
            var updatedUser = await _service.UpdateEmployee(userDto);

            if (updatedUser == null)
            {
                return BadRequest("No employee found");
            }
            
            var userWarehouseIdClaim  = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId");
            
            TriggerGetAllUsers(int.Parse(userWarehouseIdClaim!.Value));   

            return updatedUser;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in UpdateEmployee" + e);
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<bool>> DeleteEmployee(int id)
    {
        try
        {
            var deleted = await _service.DeleteEmployee(id);

            if (!deleted)
            {
                return BadRequest("No employee found");
            }

            var userWarehouseIdClaim  = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId");
            
            TriggerGetAllUsers(int.Parse(userWarehouseIdClaim!.Value));   

            return Ok("Employee deleted");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in DeleteEmployee" + e);
            return BadRequest(e.Message);
        }
    }

    private async void TriggerGetAllUsers(int warehouseId)
    {
        try
        {
            var productLocations = await _service.GetEmployeesByWarehouseId(warehouseId);

            if (productLocations == null)
            {
                return;
            }

            await _hubContext.Clients.Group(warehouseId.ToString()).SendAsync("UserListUpdate", productLocations);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in TriggerGetAllProductLocations" + e);
        }
    }
}
