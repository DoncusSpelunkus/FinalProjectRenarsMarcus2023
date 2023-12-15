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

    [Authorize(Roles = "admin")]
    [HttpGet("GetAllLogs")]
    public async Task<ActionResult<List<MoveLogDto>>> GetLog()
    {
        try
        {
            var userWarehouseIdClaim  = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId")!.Value);

            var log = await _productLocationService.GetLogsByWarehouseAsync(userWarehouseIdClaim);

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