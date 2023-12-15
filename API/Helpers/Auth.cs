using System.Security.Claims;
using Application.IServices;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class EnsureUserExistsAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Service location to retrieve IEmployeeService
        var employeeService = context.HttpContext.RequestServices.GetRequiredService<IEmployeeService>();

        var user = await employeeService.GetEmployeeById(int.Parse(context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "id")!.Value));
        if (user == null)
        {
            context.Result = new NotFoundObjectResult("User not found.");
        }
    }
}

