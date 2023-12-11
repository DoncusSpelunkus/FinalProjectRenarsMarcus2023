using System.Threading.Tasks;
using Application.Dtos;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Services;

namespace API.Controllers;

[ApiController]
[Route("[Controller]")]

public class TypeController : ControllerBase{

    private readonly ITypeService _typeService;

    public TypeController(ITypeService typeService)
    {
        _typeService = typeService;
    }

    // [Authorize]
    [HttpGet("GetByWarehouseId/{warehouseId}")]
    public async Task<ActionResult<List<TypeDto>>> GetTypesByWarehouse(int warehouseId)
    {
        try
        {
            var types = await _typeService.GetTypesByWarehouseAsync(warehouseId);

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

    // [Authorize(Roles = "Admin")]
    [HttpPost("Create")]
    public async Task<ActionResult<TypeDto>> CreateType(TypeDto typeDto)
    {
        try
        {
            var type = await _typeService.CreateTypeAsync(typeDto);

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

    // [Authorize(Roles = "Admin")]
    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<bool>> DeleteType(int id)
    {
        try
        {
            var type = await _typeService.DeletTypeAsync(id);

            if (type == false)
            {
                return BadRequest("Type not found");
            }

            return Ok("Type deleted");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // [Authorize]
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

}