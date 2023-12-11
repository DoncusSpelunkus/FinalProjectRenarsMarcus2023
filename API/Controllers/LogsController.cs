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

    [Authorize(Roles = "Admin")]
    [HttpGet("getAll/{warehouseId}")]
    public async Task<ActionResult<List<LogDto>>> GetLog(int warehouseId)
    {
        try
        {

            var userWarehouseIdClaim  = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId"); // Ensures that the user is from the proper warehouse

            if (userWarehouseIdClaim  == null || !int.TryParse(userWarehouseIdClaim .Value, out var userWarehouseId))
            {  
                return Forbid();
            }

            if (userWarehouseId != warehouseId)
            {
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

}