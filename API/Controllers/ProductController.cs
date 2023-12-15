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
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IHubContext<InventorySocket> _hubContext;
    public ProductController(IProductService productService, IHubContext<InventorySocket> hubContext)
    {
        _productService = productService;
        _hubContext = hubContext;
    }


    [Authorize]
    [HttpGet("GetBySKU/{sku}")]
    public async Task<ActionResult<ProductDto>> GetProductBySKU(string sku)
    {
        try
        {
            var products = await _productService.GetProductBySKUAsync(sku);

            if (products == null)
            {
                return NotFound("Product not found");
            }

            return products;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [HttpGet("GetAllByWarehouse")]
    public async Task<ActionResult<List<ProductDto>>> GetProductsByWarehouse()
    {

        var userWarehouseIdClaim  = int.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId")!.Value);

        try
        {
            var products = await _productService.GetProductsByWarehouseAsync(userWarehouseIdClaim);

            if (products == null)
            {
                return NotFound("Product not found");
            }

            return products;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [HttpPost("Create")]
    public async Task<ActionResult<ProductDto>> CreateProduct(ProductDto productDto)
    {
        try
        {
            productDto = CrossMethodUserClaimExtractor(productDto, HttpContext);

            var product = await _productService.CreateProductAsync(productDto);

            if (product == null)
            {
                return BadRequest("Product already exists");
            }

            var userWarehouseIdClaim  = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId");
            
            TriggerGetAllProducts(int.Parse(userWarehouseIdClaim!.Value));    

            return product;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [HttpPut("Update")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(ProductDto productDto)
    {
        try
        {
            productDto = CrossMethodUserClaimExtractor(productDto, HttpContext);

            var product = await _productService.UpdateProductAsync(productDto);

            if (product == null)
            {
                return BadRequest("Product does not exist");
            }

            var userWarehouseIdClaim  = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId");

            TriggerGetAllProducts(int.Parse(userWarehouseIdClaim!.Value));
            
            return product;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("Delete/{sku}")]
    public async Task<ActionResult<bool>> DeleteProduct(string sku)
    {
        try
        {
            var product = await _productService.DeleteProductAsync(sku);

            if (product == false)
            {
                return BadRequest("Product does not exist");
            }

            var userWarehouseIdClaim  = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId");

            TriggerGetAllProducts(int.Parse(userWarehouseIdClaim!.Value));

            return Ok("Product deleted");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    
    private async void TriggerGetAllProducts(int warehouseId)
    {
        var log = await _productService.GetProductsByWarehouseAsync(warehouseId);
        await _hubContext.Clients.Group(warehouseId.ToString() + " InventoryManagement").SendAsync("ProductListUpdate", log);
    }

    private ProductDto CrossMethodUserClaimExtractor(ProductDto dto, HttpContext httpContext)
    {
        var userWarehouseIdClaim  = int.Parse(httpContext.User.Claims.FirstOrDefault(x => x.Type == "warehouseId").Value!);

        dto.WarehouseId = userWarehouseIdClaim;
        

        return dto;
    }



}