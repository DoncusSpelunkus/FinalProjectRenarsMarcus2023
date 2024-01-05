using System.Threading.Tasks;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Services;
using sockets;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace API.Controllers;

[ApiController]
[Route("[Controller]")]

public class TypeController : ControllerBase
{
    private readonly IHubContext<InventorySocket> _hubContext;
    private readonly ITypeService _typeService;

    public TypeController(ITypeService typeService, IHubContext<InventorySocket> hubContext)
    {
        _typeService = typeService;
        _hubContext = hubContext;
    }

    [Authorize]
    [HttpGet("GetByWarehouseId")]
    public async Task<ActionResult<List<TypeDto>>> GetTypesByWarehouse()
    {
        try
        {
            var userWarehouseIdClaim = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId")!.Value);
            var types = await _typeService.GetTypesByWarehouseAsync(userWarehouseIdClaim);

            if (types == null)
            {
                return NotFound("Types not found");
            }

            return types;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [HttpPost("Create")]
    public async Task<ActionResult<TypeDto>> CreateType(TypeDto typeDto)
    {
        try
        {
            typeDto = CrossMethodUserClaimExtractor(typeDto, HttpContext);
            var type = await _typeService.CreateTypeAsync(typeDto);

            if (type == null)
            {
                return BadRequest("Type not found");
            }

            await TriggerGetAllTypes(typeDto.WarehouseId);
            return type;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<bool>> DeleteType(int id)
    {
        try
        {
            var warehouseId = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId")!.Value);
            var type = await _typeService.DeletTypeAsync(id);

            if (type == false)
            {
                return BadRequest("Type not found");
            }
            await TriggerGetAllTypes(warehouseId);
            return Ok("Type deleted");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("GetByTypeId/{id}")]
    public async Task<ActionResult<TypeDto>> GetTypeById(int id)
    {
        try
        {
            var type = await _typeService.GetTypeByIdAsync(id);

            if (type == null)
            {
                return BadRequest("Type not found");
            }

            return type;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    private async Task TriggerGetAllTypes(int warehouseId)
    {
        try
        {
            var typeList = await _typeService.GetTypesByWarehouseAsync(warehouseId);

            if (typeList == null)
            {
                return;
            }

            await _hubContext.Clients.Group(warehouseId.ToString() + " InventoryManagement").SendAsync("TypeListUpdate", typeList);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in TriggerGetAllShipments" + e);
        }
    }
    private TypeDto CrossMethodUserClaimExtractor(TypeDto dto, HttpContext httpContext)
    {
        try
        {
            var userWarehouseIdClaim = int.Parse(httpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId")!.Value);
            dto.WarehouseId = userWarehouseIdClaim;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in CrossMethodUserClaimExtractor" + e);
        }

        return dto;
    }
    
    [Authorize(Roles = "admin")]
    [HttpPut("Update")]
    public async Task<ActionResult<TypeDto>> UpdateType(TypeDto typeDto)
    {
        try
        {
            typeDto = CrossMethodUserClaimExtractor(typeDto, HttpContext);
            var type = await _typeService.UpdateTypeAsync(typeDto);

            if (type == null)
            {
                return BadRequest("Type not found");
            }

            TriggerGetAllTypes(typeDto.WarehouseId);
            return type;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in UpdateType" + e);
            return BadRequest(e.Message);
        }
    }

}