using api.Controllers.Models;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProfilesController : ApiControllerBase
{
    private readonly IUserService _userService;
    
    public ProfilesController(
        IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [HttpGet("{profileName}")]
    public ActionResult<GetProfileDetailsResponse>? GetDetail([FromRoute] string profileName)
    {
        var profileDetails = _userService.GetProfileDetails(profileName);

        if (profileDetails == null)
        {
            return NotFound();
        }

        return profileDetails;
    }

    [HttpPut("{profileName}/users/{userId}/weekly-hours")]
    public async Task<ActionResult> UpdateWeeklyHours([FromRoute] string profileName, long userId, [FromBody] List<UpdateWeeklyHoursRequest> request)
    {
        var exist = await _userService.Exists(profileName, userId);

        if (!exist)
        {
            return NotFound();
        }

        var isSuccess = await _userService.UpdateWeeklyHours(userId, request);

        return isSuccess ? Ok() : StatusCode(500);
    }
}