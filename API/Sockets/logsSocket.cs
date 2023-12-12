using System.Security.Claims;
using Application.IServices;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace sockets;

public class LogsSocket : Hub
{
    private readonly IProductLocationService _service;

    public LogsSocket(IProductLocationService service)
    {
        _service = service;
    }
    public override async Task OnConnectedAsync()
    {


        await base.OnConnectedAsync();
        {
            var user = Context.User;

            try
            {

                if (user.Identity.IsAuthenticated) // Ensures that the user is authenticated
                {
                    var warehouseId = user.FindFirst("warehouseId").Value; // Authorizes the user based on their warehouseId claim

                    if (warehouseId != null)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, warehouseId);
                        Console.WriteLine($"Client {Context.ConnectionId} connected to group {warehouseId} in Logs.");
                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in OnConnectedAsync: {e.Message}");
            }
            
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");

            var list = await _service.GetLogsByWarehouseAsync(int.Parse(user.FindFirst("warehouseId").Value));

            await Clients.Groups("warehouseId").SendAsync("LogsListUpdate", list);

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
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, warehouseId);
                    Console.WriteLine($"Client {Context.ConnectionId} disconnected from group {warehouseId}.");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in OnDisconnectedAsync: {e.Message}");
        }

        await base.OnDisconnectedAsync(exception);
    }


}

