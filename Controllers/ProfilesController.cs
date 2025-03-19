using System.Security.Claims;
using System.Text.Json;
using api.Controllers.Models;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProfilesController : ApiControllerBase
{
    private readonly IUserService _userService;
    private readonly IProfileService _profileService;
    
    public ProfilesController(
        IUserService userService,
        IProfileService profileService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
    }
    
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateProfileRequest request)
    {
        await _profileService.Create(request);

        return Ok();
    }
    
    [HttpGet("me")]             
    public async Task<ActionResult<GetProfileDetailsResponse>?> GetProfilePrivateDetails()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        
        if(email == null)
        {
            return NotFound();
        }
        
        var profileName = await _userService.GetProfileNameByUserEmail(email);

        if(profileName == null)
        {
            return NotFound();
        }
        
        var profileDetails = _profileService.GetDetails(profileName);

        if (profileDetails == null)
        {
            return NotFound();
        }

        return profileDetails;
    }
    
    [AllowAnonymous]
    [HttpGet("{profileName}")]
    public ActionResult<GetProfileDetailsResponse>? GetProfilePublicDetails([FromRoute] string profileName)
    {
        var profileDetails = _profileService.GetDetails(profileName);

        if (profileDetails == null)
        {
            return NotFound();
        }

        return profileDetails;
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(long id, [FromBody] JsonElement jsonElement)
    {
        // https://github.com/dotnet/aspnetcore/issues/24333
        var patch = JsonConvert.DeserializeObject<JsonPatchDocument>(jsonElement.GetRawText());

        if (patch == null)
        {
            return BadRequest();
        }
        
        var success = await _profileService.Patch(id, patch);
        if (!success) 
        { 
            return NotFound();
        }
        
        return NoContent(); 
    }
    
    [HttpGet("{profileName}/users/{userId}/weekly-hours")]
    public async Task<ActionResult> GetWeeklyHours([FromRoute] string profileName, long userId)
    {
        var exist = await _userService.Exists(profileName, userId);

        if (!exist)
        {
            return NotFound();
        }

        var weeklyHours = await _userService.GetWeeklyHours(userId);

        return Ok(weeklyHours);
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
    
    [HttpGet("{profileName}/users/{userId}/date-specific-hours")]
    public async Task<ActionResult> GetDateSpecificHours([FromRoute] string profileName, long userId)
    {
        var exist = await _userService.Exists(profileName, userId);

        if (!exist)
        {
            return NotFound();
        }

        var dateSpecificHours = await _userService.GetDateSpecificHours(userId);

        return Ok(dateSpecificHours);
    }
    
    [HttpPut("{profileName}/users/{userId}/date-specific-hour")]
    public async Task<ActionResult> UpsertDateSpecificHour([FromRoute] string profileName, long userId, [FromBody] UpsertDateSpecificHourRequest request)
    {
        var exist = await _userService.Exists(profileName, userId);

        if (!exist)
        {
            return NotFound();
        }

        var isSuccess = await _userService.UpsertDateSpecificHour(userId, request);

        return isSuccess ? Ok() : StatusCode(500);
    }
    
    [HttpDelete("{profileName}/users/{userId}/date-specific-hour")]
    public async Task<ActionResult> DeleteDateSpecificHour([FromRoute] string profileName, long userId, [FromBody] Guid id)
    {
        var exist = await _userService.Exists(profileName, userId);

        if (!exist)
        {
            return NotFound();
        }

        var isSuccess = await _userService.DeleteDateSpecificHour(userId, id);

        return isSuccess ? Ok() : StatusCode(500);
    }
}