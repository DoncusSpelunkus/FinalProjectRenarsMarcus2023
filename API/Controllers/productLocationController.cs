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

    // [Authorize]
    [HttpGet("GetByWarehouseId/{id}")]
    public async Task<ActionResult<ProductLocationDto>> GetProductLocationByWarehouse(int id)
    {
        try
        {
            var productLocation = await _productLocationService.GetProductLocationAsync(id);

            if (productLocation == null)
            {
                return BadRequest("Product Location not found");
            }

            return productLocation;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // [Authorize(Roles = "Admin")]
    [HttpPost("Create")]
    public async Task<ActionResult<ProductLocationDto>> CreateProductLocation(CreateProductLocationDto createProductLocationDto)
    {
        try
        {
            var productLocation = await _productLocationService.CreateProductLocationAsync(createProductLocationDto);

            if (productLocation == null)
            {
                return BadRequest("Product Location not found");
            }

            var userWarehouseIdClaim  = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId");
            
            TriggerGetAllProductLocations(int.Parse(userWarehouseIdClaim!.Value));   

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

    [Authorize(Roles = "Admin")]
    [HttpPatch("IncreaseQuantity/{productLocationId}/{quantityToAdd}")]
    public async Task<ActionResult> IncreaseQuantity(int productLocationId, int quantityToAdd)
    {
        try
        {
            await _productLocationService.IncreaseQuantityAsync(productLocationId, quantityToAdd);

            var userWarehouseIdClaim  = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId");
            
            TriggerGetAllProductLocations(int.Parse(userWarehouseIdClaim!.Value));   

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("DecreaseQuantity/{productLocationId}/{quantityToRemove}")]
    public async Task<ActionResult> DecreaseQuantity(int productLocationId, int quantityToRemove)
    {
        try
        {
            await _productLocationService.DecreaseQuantityAsync(productLocationId, quantityToRemove);

            var userWarehouseIdClaim  = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId");
            
            TriggerGetAllProductLocations(int.Parse(userWarehouseIdClaim!.Value));   

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [HttpPatch("MoveQuantity/{productsku}/{sourceProductLocationId}/{destinationProductLocationId}/{quantityToMove}")]
    public async Task<ActionResult> MoveQuantity(string productsku, string sourceProductLocationId, string destinationProductLocationId, int quantityToMove)
    {
        try
        {
            await _productLocationService.MoveQuantityAsync(productsku, sourceProductLocationId, destinationProductLocationId, quantityToMove);

            var userWarehouseIdClaim  = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId");
            
            TriggerGetAllProductLocations(int.Parse(userWarehouseIdClaim!.Value));   

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