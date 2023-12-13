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

public class Logscontroller : ControllerBase
{
    private readonly IProductLocationService _productLocationService;

    public Logscontroller(IProductLocationService productLocationService)
    {
        _productLocationService = productLocationService;
        

    }

    [Authorize]
    [HttpGet("getAllMovelogs/{warehouseId}")]
    public async Task<ActionResult<List<MoveLogDto>>> GetLog(int warehouseId)
    {
        try
        {
            
            var userWarehouseIdClaim  = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId"); // Ensures that the user is from the proper warehouse

            if (userWarehouseIdClaim  == null || !int.TryParse(userWarehouseIdClaim .Value, out var userWarehouseId))
            {  
                Console.WriteLine("User not authorized, step 1");
                return Forbid();
                
            }

            if (userWarehouseId != warehouseId)
            {
                Console.WriteLine("User not authorized, step 2");
                return Forbid();
            } 

            var log = await _productLocationService.GetLogsByWarehouseAsync(warehouseId);

            if (log == null)
            {
                return BadRequest("Log not found");
            }
            
            return log;


        }
        catch (Exception e)
        {
            Console.WriteLine("Error in GetLog" + e);
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [HttpGet("getAllAdminLogs/{warehouseId}")]
    public async Task<ActionResult<List<MoveLogDto>>> GetLogAdmin(int warehouseId)
    {
        try
        {
            
            var userWarehouseIdClaim  = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId"); // Ensures that the user is from the proper warehouse

            if (userWarehouseIdClaim  == null || !int.TryParse(userWarehouseIdClaim .Value, out var userWarehouseId))
            {  
                Console.WriteLine("User not authorized, step 1");
                return Forbid();
                
            }

            if (userWarehouseId != warehouseId)
            {
                Console.WriteLine("User not authorized, step 2");
                return Forbid();
            } 

            var log = await _productLocationService.GetAdminLogsByWarehouseAsync(warehouseId);

            if (log == null)
            {
                return BadRequest("Log not found");
            }
            
            return log;


        }
        catch (Exception e)
        {
            Console.WriteLine("Error in GetLog" + e);
            return BadRequest(e.Message);
        }
    }

}