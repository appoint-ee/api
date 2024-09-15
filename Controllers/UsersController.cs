using api.Controllers.Models;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ApiControllerBase
{
    private readonly IUserService _userService;
    
    public UsersController(
        IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [HttpGet("{userName}")]
    public ActionResult<GetUserByUserNameResponse>? GetByUserName([FromRoute] string userName)
    {
        var user = _userService.Get(userName);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    [HttpPut("{id}/availability-hours")]
    public async Task<ActionResult> UpdateAvailabilityHours([FromRoute] long id, [FromBody] List<UpdateAvailabilityHoursRequest> request)
    {
        var exist = await _userService.Exists(id);

        if (!exist)
        {
            return NotFound();
        }

        var isSuccess = await _userService.UpdateAvailabilityHours(id, request);

        return isSuccess ? Ok() : StatusCode(500);
    }
}