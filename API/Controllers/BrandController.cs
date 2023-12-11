using System.Threading.Tasks;
using Application.Dtos;
using Application.IServices;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers;

[ApiController]
[Route("[Controller]")]
public class BrandController : ControllerBase {

private readonly IBrandService _brandService;

public BrandController(IBrandService brandService)
{
    _brandService = brandService;

}

// [Authorize]
[HttpGet("GetByWarehouseId/{warehouseId}")]
public async Task<ActionResult<List<BrandDto>>> GetBrandsByWarehouse(int warehouseId)
{
    try
    {
        var brands = await _brandService.GetBrandsByWarehouseAsync(warehouseId);

        if (brands == null)
        {
            return NotFound("Brand not found");
        }

        return brands;
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

// [Authorize(Roles = "Admin")]
[HttpPost("Create")]
public async Task<ActionResult<BrandDto>> CreateBrand(BrandDto BrandDto)
{
    try
    {
        var brand = await _brandService.CreateBrandAsync(BrandDto);

        if (brand == null)
        {
            return BadRequest("Brand not found");
        }

        return brand;
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

// [Authorize(Roles = "Admin")]
[HttpDelete("Delete/{id}")]
public async Task<ActionResult<bool>> DeleteBrand(int id)
{
    try
    {
        var brand = await _brandService.DeleteBrandAsync(id);

        if (brand == false)
        {
            return BadRequest("Brand not found");
        }

        return Ok("Brand deleted");
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

// [Authorize]
[HttpGet("GetById/{id}")]
public async Task<ActionResult<BrandDto>> GetBrandById(int id)
{
    try
    {
        var brand = await _brandService.GetBrandByIdAsync(id);

        if (brand == null)
        {
            return BadRequest("No brand found");
        }

        return brand;
    }
    catch (Exception e)
    {
        Console.WriteLine("Error in GetBrandById" + e);
        return BadRequest(e.Message);
    }

}

}