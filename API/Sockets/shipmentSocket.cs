using System.Security.Claims;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace sockets;

public class ShipmentSocket : Hub
{
    private readonly IShipmentService _service;

    public ShipmentSocket(IShipmentService service)
    {
        _service = service;
    }
    public override async Task OnConnectedAsync()
    {

        try
        {
            var user = Context.User ?? throw new ApplicationException("User is null");

            if (user.Identity.IsAuthenticated) // Ensures that the user is authenticated
            {
                var role = user.FindFirst(ClaimTypes.Role)?.Value; // Usermanagement socket is only for admins
                if (role != "admin" && role != "sales")
                {
                    await base.OnDisconnectedAsync(exception: new Exception("Unauthorized"));
                    return;
                }

                var warehouseId = user.FindFirst("warehouseId")?.Value; // Authorizes the user based on their warehouseId claim

                if (warehouseId != null)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, warehouseId + " ShipmentManagement");
                    Console.WriteLine($"Client {Context.ConnectionId} connected to group {warehouseId} in Shipment.");
                }
            }



        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in OnConnectedAsync: {e.Message}");
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var user = Context.User;

            if (user!.Identity!.IsAuthenticated)
            {
                var warehouseId = user.FindFirst("warehouseId")?.Value;

                if (warehouseId != null)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, warehouseId + " ShipmentManagement");
                    Console.WriteLine($"Client {Context.ConnectionId} disconnected from group {warehouseId} shipment management.");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in OnDisconnectedAsync: {e.Message}");
        }

    }
    public async Task RequestShipment()
    {
        try
        {
            var user = Context.User;

            if (user.Identity.IsAuthenticated) // Ensures that the user is authenticated
            {
                var role = user.FindFirst(ClaimTypes.Role)?.Value; // Usermanagement socket is only for admins
                if (role != "admin" && role != "sales")
                {
                    await base.OnDisconnectedAsync(exception: new Exception("Unauthorized"));
                    return;
                }

                var warehouseId = user.FindFirst("warehouseId")?.Value;

                var list = await _service.GetShipmentsByWarehouseAsync(int.Parse(warehouseId!));

                await Clients.Group(warehouseId + " ShipmentManagement").SendAsync("ShipmentListUpdate", list);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in RequestShipment: {e.Message}");
        }

    }
}

