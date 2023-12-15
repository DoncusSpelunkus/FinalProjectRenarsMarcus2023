 using System.Threading.Tasks;
using Application.Dtos;
using Application.IServices;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[Controller]")]

public class WarehouseController : ControllerBase{


    private readonly IWarehouseService _warehouseService;


    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }
    
    [Authorize(Roles = "admin")]
    [HttpPost("Create")]
    public async Task<ActionResult<object>> CreateWarehouse(WarehouseDto warehouseDto)
    {
        try
        {
            var warehouseResult = await _warehouseService.CreateWarehouse(warehouseDto);

            if (warehouseResult == null)
            {
                return BadRequest("Warehouse could not be created");
            }

            return warehouseResult;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in CreateWarehouse" + e);
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [HttpPut("Update/{id}")]
    public async Task<ActionResult<WarehouseDto>> UpdateWarehouse(WarehouseDto warehouseDto, int id)
    {
        try
        {
            var warehouseResult = await _warehouseService.UpdateWarehouseAsync(id, warehouseDto);

            if (warehouseResult == null)
            {
                return BadRequest("Warehouse could not be updated");
            }

            return warehouseResult;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in UpdateWarehouse" + e);
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<bool>> DeleteWarehouse(int id)
    {
        try
        {
            var warehouseResult = await _warehouseService.DeleteWarehouseAsync(id);

            if (warehouseResult == false)
            {
                return BadRequest("Warehouse could not be deleted");
            }

            return Ok("Warehouse deleted");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in DeleteWarehouse" + e);
            return BadRequest(e.Message);
        }
    }
}
 
 