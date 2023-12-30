using System.Threading.Tasks;
using API.Helpers;
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

    public ProductLocationController(
        IProductLocationService productLocationService,
         IHubContext<InventorySocket> hubContext)
    {
        _productLocationService = productLocationService;
        _hubContext = hubContext;

    }

    [EnsureUserExists]
    [Authorize(Roles = "admin")]
    [HttpPost("Create")]
    public async Task<ActionResult<ProductLocationDto>> CreateProductLocation(ActionDto actionDto)
    {
        try
        {

            actionDto = CrossMethodUserClaimExtractor(actionDto, HttpContext);

            var productLocation = await _productLocationService.CreateProductLocationAsync(actionDto);

            if (productLocation == null)
            {
                return BadRequest("Product Location not found");
            }

            TriggerGetAllProductLocations(actionDto.WarehouseId);

            return productLocation;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    [Authorize]
    [HttpGet("GetListByWarehouseId")]
    public async Task<ActionResult<List<ProductLocationDto>>> GetProductLocationsByWarehouse()
    {
        try
        {
            var userWarehouseIdClaim = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId")!.Value);

            var productLocationss = await _productLocationService.GetProductLocationsByWarehouseAsync(userWarehouseIdClaim);

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
            actionDto = CrossMethodUserClaimExtractor(actionDto, HttpContext);

            await _productLocationService.ChangeQuantity(actionDto);

            TriggerGetAllProductLocations(actionDto.WarehouseId);

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [EnsureUserExists]
    [Authorize]
    [HttpPatch("MoveQuantity")]
    public async Task<ActionResult> MoveQuantity(ActionDto actionDto)
    {
        try
        {
            actionDto = CrossMethodUserClaimExtractor(actionDto, HttpContext);

            await _productLocationService.MoveQuantityAsync(actionDto);

            TriggerGetAllProductLocations(actionDto.WarehouseId);

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

            await _hubContext.Clients.Group(warehouseId.ToString() + " InventoryManagement").SendAsync("ProductLocationListUpdate", productLocations);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in TriggerGetAllProductLocations" + e);
        }
    }

    private ActionDto CrossMethodUserClaimExtractor(ActionDto actionDto, HttpContext httpContext)
    {
        var userIdClaim = int.Parse(httpContext.User.Claims.FirstOrDefault(x => x.Type == "id").Value!);
        var userWarehouseIdClaim = int.Parse(httpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId").Value!);

        actionDto.WarehouseId = userWarehouseIdClaim;
        actionDto.EmployeeId = userIdClaim;

        return actionDto;
    }
}