using System.Threading.Tasks;
using Application.Dtos;
using Application.IServices;
using Application.Services;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using sockets;


namespace API.Controllers;

[ApiController]
[Route("[Controller]")]

public class LocationController : ControllerBase
{

    private readonly ILocationService _locationService;

    private readonly IHubContext<InventorySocket> _hubContext;

    public LocationController(ILocationService locationService, IHubContext<InventorySocket> hubContext)
    {
        _locationService = locationService;
        _hubContext = hubContext;
    }



    [Authorize]
    [HttpGet("GetAllByWarehouse")]
    public async Task<ActionResult<List<LocationDto>>> GetLocationsByWarehouse()
    {
        try
        {
            var userWarehouseIdClaim  = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId")!.Value);
            var locations = await _locationService.GetLocationsByWarehouseAsync(userWarehouseIdClaim);

            if (!locations.Any())
            {
                return NotFound("No locations found for the specified warehouse");
            }

            return locations;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in GetLocationsByWarehouse" + e);
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [HttpPost("Create")]
    public async Task<ActionResult<LocationDto>> CreateLocation(LocationDto locationDto)
    {
        try
        {
            locationDto = CrossMethodUserClaimExtractor(locationDto, HttpContext);
            var location = await _locationService.CreateLocationAsync(locationDto);

            if (location == null)
            {
                return BadRequest("Location not found");
            }

            var userWarehouseIdClaim  = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId");

            TriggerGetAllLocations(int.Parse(userWarehouseIdClaim!.Value));

            return location;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in CreateLocation" + e);
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [HttpPut("Update")]
    public async Task<ActionResult<LocationDto>> UpdateLocation(LocationDto locationDto)
    {
        try
        {
            locationDto = CrossMethodUserClaimExtractor(locationDto, HttpContext);
            var location = await _locationService.UpdateLocationAsync(locationDto);

            if (location == null)
            {
                return BadRequest("Location not found");
            }

            var userWarehouseIdClaim  = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId");

            TriggerGetAllLocations(int.Parse(userWarehouseIdClaim!.Value));

            return location;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in UpdateLocation" + e);
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<bool>> DeleteLocation(string id)
    {
        try
        {
            var location = await _locationService.DeleteLocationAsync(id);

            if (location == false)
            {
                return BadRequest("No location found");
            }
            
            var userWarehouseIdClaim  = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId");

            TriggerGetAllLocations(int.Parse(userWarehouseIdClaim!.Value));

            return Ok("Location deleted");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in DeleteEmployee" + e);
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [HttpPost("createBatch")]
    public async Task<ActionResult<List<LocationDto>>> CreateBatch(LocationDto locationDto){
        try {

            locationDto = CrossMethodUserClaimExtractor(locationDto, HttpContext);

            var list = await _locationService.CreateLocationBatch(locationDto);

            if (list == null)
            {
                return BadRequest("Something went wrong");
            }

            var userWarehouseIdClaim  = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId");

            TriggerGetAllLocations(int.Parse(userWarehouseIdClaim!.Value));

            return list;

        }
        catch(Exception e) {
            Console.WriteLine("Error in DeleteEmployee" + e);
            return BadRequest(e.Message);
        }
    }
    
    private async void TriggerGetAllLocations(int warehouseId)
    {
        var data = _locationService.GetLocationsByWarehouseAsync(warehouseId);
        await _hubContext.Clients.Group(warehouseId.ToString() + " InventoryManagement").SendAsync("LocationListUpdate", data);

    }

    private LocationDto CrossMethodUserClaimExtractor(LocationDto dto, HttpContext httpContext)
    {
        var userWarehouseIdClaim  = int.Parse(httpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId").Value!);

        dto.WarehouseId = userWarehouseIdClaim;
        
        return dto;
    }
}

