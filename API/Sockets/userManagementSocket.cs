using System.Security.Claims;
using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;


namespace sockets; // Copy the methods from here


public class UserManagementSocket : Hub // Simple hub that automatically adds users to groups based on their warehouseId claim
{

    private readonly IEmployeeService _service;

    public UserManagementSocket(IEmployeeService service)
    {
        _service = service;
    }

    public override async Task OnConnectedAsync()
    {

        try
        {
            var user = Context.User;

            if (user!.Identity!.IsAuthenticated) // Ensures that the user is authenticated
            {
                var role = user.FindFirst(ClaimTypes.Role)?.Value; // Usermanagement socket is only for admins
                if (role != "admin")
                {
                    await base.OnDisconnectedAsync(exception: new Exception("Unauthorized"));
                    return;
                }

                var warehouseId = user.FindFirst("warehouseId")?.Value; // Authorizes the user based on their warehouseId claim

                if (warehouseId != null)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, warehouseId + " UserMangement");
                    Console.WriteLine($"Client {Context.ConnectionId} connected to group {warehouseId} UserMangement in user management.");
                }
                else
                {
                    await base.OnDisconnectedAsync(exception: new Exception("No valid warehouseID claim found"));
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
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, warehouseId + " UserMangement");
                    Console.WriteLine($"Client {Context.ConnectionId} disconnected from group {warehouseId} UserMangement.");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in OnDisconnectedAsync: {e.Message}");
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task RequestUsers()
    {
        var user = Context.User;

        if (user.Identity.IsAuthenticated) // Ensures that the user is authenticated
        {
            var role = user.FindFirst(ClaimTypes.Role)?.Value; // Usermanagement socket is only for admins
            if (role != "sales" && role != "admin")
            {
                await base.OnDisconnectedAsync(exception: new Exception("Unauthorized"));
                return;
            }

            var warehouseId = user.FindFirst("warehouseId")?.Value;

            var list = await _service.GetEmployeesByWarehouseId(int.Parse(warehouseId!));

            await Clients.Group(warehouseId + " UserMangement").SendAsync("UserListUpdate", list);
        }
    }


}

