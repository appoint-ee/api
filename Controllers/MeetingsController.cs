using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using api.Controllers.Models;
using api.Services;
using api.Services.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class MeetingsController : ApiControllerBase
{
    private readonly IMeetingService _meetingService;
    private readonly IUserService _userService;

    public MeetingsController(
        IMeetingService meetingService,
        IUserService userService)
    {
        _meetingService = meetingService ?? throw new ArgumentNullException(nameof(meetingService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateMeetingRequest request)
    {
        var authorizedEmail = User.FindFirstValue(ClaimTypes.Email);
        
        // TODO: Move this to fluent validation
        if (authorizedEmail == null
            && (request.CreatorEmailAddress == null 
                || request.CreatorFirstName == null
                || request.CreatorLastName == null
                || request.CreatorPhoneNumber == null))
        { 
            return BadRequest();
        }
        
        if(authorizedEmail == null)
        {
            var newUser = new CreateUserRequest
            {
                FirstName = request.CreatorFirstName,
                LastName = request.CreatorLastName,
                EmailAddress = request.CreatorEmailAddress,
                PhoneNumber = request.CreatorPhoneNumber,
            };
            
            // TODO: Return a specific error for unauthorized users with an existing account.
            var createdUser = await _userService.Create(newUser);

            if (createdUser == null)
            {
                return BadRequest();
            }
            
            request.CreatorId = createdUser.Id;
        }
        else 
        {
            var authorizedUser = await _userService.GetByEmail(authorizedEmail);
            
            request.CreatorId = authorizedUser.Id;
            request.CreatorEmailAddress = authorizedUser.EmailAddress;
        }

        _meetingService.Create(request);

        return Ok();
    }
    
    [HttpGet]
    public async Task<ActionResult<List<GetMeetingResponse>?>> Get([FromQuery] DateTime? start, DateTime? end)
    {
        var authorizedEmail = User.FindFirstValue(ClaimTypes.Email);

        start ??= DateTime.Now.AddMonths(-1);
        end ??= DateTime.Now;

        if (authorizedEmail == null)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        var authorizedUser = await _userService.GetByEmail(authorizedEmail);

        var meeting = await _meetingService.Get(authorizedUser.Id, start.Value, end.Value);

        if (meeting == null)
        {
            return NotFound();
        }
        
        return Ok(meeting);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetMeetingDetailResponse?>> Get(Guid id)
    {
        var meeting = await _meetingService.Get(id);

        if (meeting == null)
        {
            return NotFound();
        }
        
        return Ok(meeting);
    }
}