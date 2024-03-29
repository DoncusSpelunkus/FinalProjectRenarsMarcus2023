﻿using Application.Dtos;
using Application.IServices;
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
    private readonly IHubContext<AuthSocket> _authHubContext;

    public UserController(IEmployeeService service, IHubContext<UserManagementSocket> hubContext, IHubContext<AuthSocket> authHubContext)
    {
        _service = service;
        _hubContext = hubContext;
        _authHubContext = authHubContext;
    }


    [HttpGet("createDb")]
    public string CreateDb()
    {
        _service.CreateDB();
        return "DB build complete :D";
    }

    [Authorize(Roles = "admin")]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(UserDto receivedUser)
    {
        try
        {
            var warehouseId = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId")!.Value);
            receivedUser.WarehouseId = warehouseId;
            var userDto = await _service.CreateEmployee(receivedUser);

            if (userDto == null)
            {
                return BadRequest("User is Taken");
            }

            await TriggerGetAllUsers(userDto.WarehouseId);

            return userDto;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginDto loginDto)
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
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [HttpGet("GetAllByWareHouseId")]
    public async Task<ActionResult<List<UserDto>>> GetAllByWareHouseId()
    {
        try
        {

            var warehouseId = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId")!.Value);
            var users = await _service.GetEmployeesByWarehouseId(warehouseId);

            if (users == null)
            {
                return BadRequest("No employees found");
            }

            return users;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
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

            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [HttpPut("Update")] // Should take an id in param?
    public async Task<ActionResult<UserDto>> UpdateEmployee(UserDto userDto)
    {
        try
        {
            var warehouseId = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId")!.Value);

            userDto.WarehouseId = warehouseId;

            var updatedUser = await _service.UpdateEmployee(userDto);

            if (updatedUser == null)
            {
                return BadRequest("No employee found");
            }

            var userWarehouseIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId");

            TriggerGetAllUsers(int.Parse(userWarehouseIdClaim!.Value));

            return updatedUser;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
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

            var userWarehouseIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId");

            await TriggerGetAllUsers(int.Parse(userWarehouseIdClaim!.Value));
            await _authHubContext.Clients.Group(id + " AuthManagement").SendAsync("UserDelete");

            return Ok("Employee deleted");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [HttpPatch("PasswordUpdate")]
    public async Task<ActionResult<bool>> PasswordUpdate(NewPasswordDTO dto)
    {
        try
        {
            var userIdClaim = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "id").Value!);

            var updated = await _service.UpdatePassword(userIdClaim, dto.OldPassword, dto.NewPassword);

            if (!updated)
            {
                return BadRequest("No employee found");
            }

            return Ok("Password updated");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("ResetPassword/{email}")]
     public async Task<ActionResult<bool>> ResetPassword(string email)
     {
         try
         {
            var updated = await _service.ResetPassword(email);

            if (!updated)
            {
                return BadRequest("No employee found");
            }

             return Ok("Password updated");
         }
         catch (Exception e)
         {
             return BadRequest(e.Message);
         }
     }
     
    [HttpPost("ContactSupport")]
    public ActionResult<string> ContactSupport(string contactEmail,string description)
    {
        try
        {
            _service.sendEmailToSupport(contactEmail,description);
            return Ok("Message sent");
            
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private async Task TriggerGetAllUsers(int warehouseId)
    {
        try
        {
            var users = await _service.GetEmployeesByWarehouseId(warehouseId);

            if (users == null)
            {
                return;
            }

            await _hubContext.Clients.Group(warehouseId.ToString() + " UserMangement").SendAsync("UserListUpdate", users);

        }
        catch (Exception e)
        {
             throw new ApplicationException(e.Message);
        }
    }
}
