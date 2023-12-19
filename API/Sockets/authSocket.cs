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


public class AuthSocket : Hub // Simple hub that automatically adds users to groups based on their warehouseId claim
{
    private readonly IEmployeeService _employeeService;

    public AuthSocket(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
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
                    var id = user.FindFirst("id")?.Value; // Authorizes the user based on their warehouseId claim

                    if (id != null)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, id +" AuthManagement");
                        Console.WriteLine($"Client {Context.ConnectionId} connected to group {id + " AuthManagement"}");
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
                var id = user.FindFirst("id")?.Value;

                if (id != null)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, id + " AuthManagement");
                    Console.WriteLine($"Client {Context.ConnectionId} disconnected from group {id + " AuthManagement"}");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in OnDisconnectedAsync: {e.Message}");
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task RequestMe()
    {
        var user = Context.User;

        if (user.Identity.IsAuthenticated) // Ensures that the user is authenticated
        {
            var id = int.Parse(user.FindFirst("id")?.Value);

            var thisUser = await _employeeService.GetEmployeeById(id);

            await Clients.Group(id + " AuthManagement").SendAsync("UserUpdate", thisUser);
        }
    }
}