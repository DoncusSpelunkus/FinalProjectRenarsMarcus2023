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

public class ProductLocationController : ControllerBase
{

    private readonly IProductLocationService _productLocationService;
    private readonly IHubContext<InventorySocket> _hubContext;

    public ProductLocationController(IProductLocationService productLocationService, IHubContext<InventorySocket> hubContext)
    {
        _productLocationService = productLocationService;
        _hubContext = hubContext;
    }

    [Authorize(Roles = "admin")]
    [HttpPost("Create")]
    public async Task<ActionResult<ProductLocationDto>> CreateProductLocation(ActionDto actionDto)
    {
        try
        {
            var userIdClaim  = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "id").Value!);
            var userWarehouseIdClaim  = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId").Value!);

            actionDto.WarehouseId = userWarehouseIdClaim;
            actionDto.EmployeeId = userIdClaim;

            var productLocation = await _productLocationService.CreateProductLocationAsync(actionDto);

            if (productLocation == null)
            {
                return BadRequest("Product Location not found");
            }
            
            TriggerGetAllProductLocations(userWarehouseIdClaim);   

            return productLocation;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    [Authorize]
    [HttpGet("GetListByWarehouseId/{warehouseId}")]
    public async Task<ActionResult<List<ProductLocationDto>>> GetProductLocationsByWarehouse(int warehouseId)
    {
        try
        {
            var productLocationss = await _productLocationService.GetProductLocationsByWarehouseAsync(warehouseId);

            if (productLocationss == null)
            {
                return NotFound("productLocationss not found");
            }

            return productLocationss;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [HttpPatch("ChangeQuantity")]
    public async Task<ActionResult> IncreaseQuantity(ActionDto actionDto)
    {
        try
        {
            var userIdClaim  = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "id").Value!);
            var userWarehouseIdClaim  = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId").Value!);

            actionDto.WarehouseId = userWarehouseIdClaim;
            actionDto.EmployeeId = userIdClaim;

            await _productLocationService.ChangeQuantity(actionDto);

            TriggerGetAllProductLocations(userWarehouseIdClaim);   

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [HttpPatch("MoveQuantity")]
    public async Task<ActionResult> MoveQuantity(ActionDto actionDto)
    {
        try
        {
             var userIdClaim  = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "id").Value!);
            var userWarehouseIdClaim  = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId").Value!);

            actionDto.WarehouseId = userWarehouseIdClaim;
            actionDto.EmployeeId = userIdClaim;

            await _productLocationService.MoveQuantityAsync(actionDto);
            
            TriggerGetAllProductLocations(userWarehouseIdClaim);   

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    private async void TriggerGetAllProductLocations(int warehouseId)
    {
        try
        {
            var productLocations = await _productLocationService.GetProductLocationsByWarehouseAsync(warehouseId);

            if (productLocations == null)
            {
                return;
            }

            await _hubContext.Clients.Group(warehouseId.ToString()).SendAsync("ReceiveMessage", productLocations);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in TriggerGetAllProductLocations" + e);
        }
    }
}