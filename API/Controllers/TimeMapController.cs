using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TimeMapController : ControllerBase
{
    private readonly ITimeMapService _timeMapService;

    public TimeMapController(ITimeMapService timeMapService)
    {
        _timeMapService = timeMapService;
    }

    [HttpPost("signin/{employeeId}")]
    public IActionResult SignIn(int employeeId)
    {
        var result = _timeMapService.SignIn(employeeId);

        return Ok(result);
    }

    [HttpPost("signout/{employeeId}")]
    public IActionResult SignOut(int employeeId)
    {
        var result = _timeMapService.SignOut(employeeId);

        return Ok(result);
    }
}