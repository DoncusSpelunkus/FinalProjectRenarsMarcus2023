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
public class BrandController : ControllerBase
{

    private readonly IBrandService _brandService;

    private readonly IHubContext<InventorySocket> _hubContext;

    public BrandController(IBrandService brandService, IHubContext<InventorySocket> hubContext)
    {
        _brandService = brandService;
        _hubContext = hubContext;

    }

    [Authorize]
    [HttpGet("GetByWarehouseId")]
    public async Task<ActionResult<List<BrandDto>>> GetBrandsByWarehouse()
    {
        try
        {
            var userWarehouseIdClaim = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId")!.Value);
            var brands = await _brandService.GetBrandsByWarehouseAsync(userWarehouseIdClaim);

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

    [Authorize(Roles = "admin")]
    [HttpPost("Create")]
    public async Task<ActionResult<BrandDto>> CreateBrand(BrandDto BrandDto)
    {
        try
        {
            BrandDto = CrossMethodUserClaimExtractor(BrandDto, HttpContext);
            var brand = await _brandService.CreateBrandAsync(BrandDto);

            if (brand == null)
            {
                return BadRequest("Brand not found");
            }

            TriggerGetAllBrands(BrandDto.WarehouseId);

            return brand;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult<bool>> DeleteBrand(int id)
    {
        try
        {   
            var warehouseId = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId")!.Value);
            var brand = await _brandService.DeleteBrandAsync(id);

            if (brand == false)
            {
                return BadRequest("Brand not found");
            }

            TriggerGetAllBrands(warehouseId);

            return Ok("Brand deleted");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    
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

    [Authorize(Roles = "admin")]
    [HttpPut("Update")]
    public async Task<ActionResult<BrandDto>> UpdateBrand(BrandDto brandDto)
    {
        try
        {
            brandDto = CrossMethodUserClaimExtractor(brandDto, HttpContext);
            var brand = await _brandService.UpdateBrandAsync(brandDto);

            if (brand == null)
            {
                return BadRequest("Brand not found");
            }

            TriggerGetAllBrands(brandDto.WarehouseId);

            return brand;
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in UpdateBrand" + e);
            return BadRequest(e.Message);
        }
    }

    

    private async void TriggerGetAllBrands(int warehouseId)
    {
        try
        {
            var typeList = await _brandService.GetBrandsByWarehouseAsync(warehouseId);

            if (typeList == null)
            {
                return;
            }

            await _hubContext.Clients.Group(warehouseId.ToString() + " InventoryManagement").SendAsync("BrandListUpdate", typeList);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in TriggerGetAllShipments" + e);
        }
    }

    private BrandDto CrossMethodUserClaimExtractor(BrandDto dro, HttpContext httpContext)
    {

        var userWarehouseIdClaim = int.Parse(httpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId").Value!);

        dro.WarehouseId = userWarehouseIdClaim;

        return dro;
    }

}