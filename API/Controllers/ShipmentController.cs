using System.Threading.Tasks;
using Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.IServices;
using sockets;
using Microsoft.AspNetCore.SignalR;



namespace API.Controllers;

[ApiController]
[Route("[Controller]")]
public class ShipmentController : ControllerBase
{

    private readonly IShipmentService _shipmentService;
    private readonly IHubContext<ShipmentSocket> _hubContext;

    public ShipmentController(IShipmentService shipmentService, IHubContext<ShipmentSocket> hubContext)
    {
        _shipmentService = shipmentService;
        _hubContext = hubContext;
    }

    [Authorize(Roles = "sales, admin")]
    [HttpPost("Create")]
    public async Task<ActionResult<ShipmentDto>> CreateShipment(ShipmentDto shipmentDto)
    {

        try
        {

            shipmentDto = CrossMethodUserClaimExtractor(shipmentDto, HttpContext);

            var shipment = await _shipmentService.CreateShipmentAsync(shipmentDto);

            if (shipment == null)
            {
                return BadRequest("Shipment not found");
            }

            await TriggerGetAllShipments(shipmentDto.WarehouseId);

            return shipment;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "sales, admin")]
    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<bool>> DeleteShipment(int id)
    {
        try
        {
            var warehouseId = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId")!.Value);
            var shipment = await _shipmentService.DeleteShipmentAsync(id);

            if (shipment = !true)
            {
                return BadRequest("Shipment not found");
            }

            await TriggerGetAllShipments(warehouseId);

            return Ok("Shipment deleted");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "sales, admin")]
    [HttpPut("AddToShipment/{shipmentId}")]
    public async Task<ActionResult<ShipmentDto>> AddToShipment(int shipmentId, AddToShipmentDetails shipmentDetailDto)
    {
        try
        {
            var shipment = await _shipmentService.AddProductToShipmentAsync(shipmentDetailDto, shipmentId);

            if (shipment == null)
            {
                return BadRequest("Shipment not found");
            }

            return shipment;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    [Authorize(Roles = "sales, admin")]
    [HttpPut("RemoveFromShipment/{shipmentId}")]
    public async Task<ActionResult<bool>> RemoveFromShipment(int shipmentId, IntArrayDto shipmentDetailDto)
    {
        try
        {

            var shipment = await _shipmentService.RemoveProductFromShipmentAsync(shipmentId, shipmentDetailDto.data);

            if (shipment = !true)
            {
                return BadRequest("Shipment not found");
            }

            return Ok("Removed from shipment");

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "sales, admin")]
    [HttpPatch("ChangeQuantiy/{shipmentId}/{shipmentDetailId}/{quantity}")]
    public async Task<ActionResult<bool>> ChangeQuantity(int shipmentId, int shipmentDetailId, int quantity)
    {
        try
        {

            var shipment = await _shipmentService.ChangeProductQuantityInShipmentAsync(shipmentId, shipmentDetailId, quantity);

            if (shipment == false)
            {
                return BadRequest("Shipment not found");
            }


            return shipment;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "sales, admin")]
    [HttpGet("GetAllByWarehouseId")]
    public async Task<ActionResult<List<ShipmentDto>>> GetShipmentsByWarehouse()
    {
        try
        {
            var warehouseId = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId")!.Value);
            var shipments = await _shipmentService.GetShipmentsByWarehouseAsync(warehouseId);

            if (shipments == null)
            {
                return NotFound("Shipment not found");
            }

            return shipments;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "sales, admin")]
    [HttpGet("GetByShipmentId/{shipmentId}")]
    public async Task<ActionResult<ShipmentDto>> GetShipmentById(int shipmentId)
    {
        try
        {

            var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);

            if (shipment == null)
            {
                return NotFound("Shipment not found");
            }



            return shipment;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private async Task TriggerGetAllShipments(int warehouseId)
    {
        try
        {
            var shipmentList = await _shipmentService.GetShipmentsByWarehouseAsync(warehouseId);

            if (shipmentList == null)
            {
                return;
            }

            await _hubContext.Clients.Group(warehouseId.ToString() + " ShipmentManagement").SendAsync("ShipmentListUpdate", shipmentList);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in TriggerGetAllProductLocations" + e);
        }
    }

    private ShipmentDto CrossMethodUserClaimExtractor(ShipmentDto dto, HttpContext httpContext)
    {
        try
        {
            var userIdClaim = int.Parse(httpContext.User.Claims.FirstOrDefault(x => x.Type == "id").Value!);
            var userWarehouseIdClaim = int.Parse(httpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId").Value!);

            dto.WarehouseId = userWarehouseIdClaim;
            dto.ShippedByEmployeeId = userIdClaim;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in CrossMethodUserClaimExtractor" + e);
        }

        return dto;
    }


}