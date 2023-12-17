using System.Security.Claims;
using Application.Helpers;
using Application.Dtos;
using Application.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.OpenApi.Any;

namespace sockets;


public class InventorySocket : Hub // Simple hub that automatically adds users to groups based on their warehouseId claim
{
    private readonly IMapper _mapper;
    private readonly IProductLocationService _productLocationService;

    private readonly IProductService _productService;

    private readonly IBrandService _brandService;

    private readonly ITypeService _typeService;

    private readonly ILocationService _locationService;



    public InventorySocket(
        IProductLocationService productLocationService,
        IProductService productService,
        IBrandService brandService,
        ITypeService typeService,
        ILocationService locationService,
        IMapper mapper
        )
    {
        _productLocationService = productLocationService;
        _productService = productService;
        _brandService = brandService;
        _typeService = typeService;
        _locationService = locationService;
        _mapper = mapper;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        {
            try
            {
                var user = Context.User ?? throw new ApplicationException("User is null");

                if (user.Identity.IsAuthenticated) // Ensures that the user is authenticated
                {
                    var warehouseId = user.FindFirst("warehouseId")?.Value; // Authorizes the user based on their warehouseId claim

                    if (warehouseId != null)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, warehouseId + " InventoryManagement");
                        Console.WriteLine($"Client {Context.ConnectionId} connected to group {warehouseId} in inventory management.");
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in OnConnectedAsync: {e.Message}");
            }
        }

    }


    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var user = Context.User ?? throw new ApplicationException("User is null");

            if (user!.Identity!.IsAuthenticated)
            {
                var warehouseId = user.FindFirst("warehouseId")?.Value;

                if (warehouseId != null)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, warehouseId + " InventoryManagement");
                    Console.WriteLine($"Client {Context.ConnectionId} disconnected from group {warehouseId} in inventory.");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in OnDisconnectedAsync: {e.Message}");
        }

        await base.OnDisconnectedAsync(exception);
    }


    public async Task Request(RequestDto message) // Handles requests from the client
    {
        Console.WriteLine(message.RequestType);
        var user = Context.User ?? throw new ApplicationException("User is null");


        if (user!.Identity!.IsAuthenticated) // Ensures that the user is authenticated
        {

            var warehouseId = int.Parse(user.FindFirst("warehouseId")?.Value);

            List<object> list = null;

            switch (message.RequestType)
            {
                case EndpointEnum.Product: // 1
                    var productList = await _productService.GetProductsByWarehouseAsync(warehouseId);
                    list = _mapper.Map<List<object>>(productList);
                    break;
                case EndpointEnum.ProductLocation: // 2
                    var productLocationList = await _productLocationService.GetProductLocationsByWarehouseAsync(warehouseId);
                    list = _mapper.Map<List<object>>(productLocationList);
                    break;
                case EndpointEnum.Location: // 3
                    var locationList = await _locationService.GetLocationsByWarehouseAsync(warehouseId);
                    list = _mapper.Map<List<object>>(locationList);
                    break;
                case EndpointEnum.Brand: // 4
                    var brandList = await _brandService.GetBrandsByWarehouseAsync(warehouseId);
                    list = _mapper.Map<List<object>>(brandList);
                    break;
                case EndpointEnum.Type: // 5
                    var typeList = await _typeService.GetTypesByWarehouseAsync(warehouseId);
                    list = _mapper.Map<List<object>>(typeList);
                    break;

                default:
                    Console.WriteLine("Error in Request: No endpoint found");
                    break;

            }

            if (list == null)
            {
                Console.WriteLine("Error in Request: List is null");
                return;
            }

            await Clients.Caller.SendAsync(message.RequestType.ToString() + "UpdateList", list);
        }

    }

}

