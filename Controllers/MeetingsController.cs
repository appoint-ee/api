using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using api.Controllers.Models;
using api.Data.Enums;
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
        
        if (IsMissingCreatorInfo(request, authorizedEmail))
        {
            return BadRequest();
        }
        
        if(authorizedEmail == null)
        {
            var userInfo = await _userService.GetByEmail(request.CreatorEmailAddress);

            if (userInfo == null)
            {
                var createdUser = await CreateUser(request);

                if (createdUser == null)
                {
                    return BadRequest();
                }
                
                request.CreatorId = createdUser.Id;
            }
            else if (DoesUserInfoMismatch(request, userInfo))
            {
                return BadRequest("A user with this email address already exists. Please check your information or log in.");
            }
            else
            {
                AssignUserToRequest(userInfo, request);
            }
        }
        else 
        {
            var authorizedUser = await _userService.GetByEmail(authorizedEmail);

            if (authorizedUser == null)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            AssignUserToRequest(authorizedUser, request);
        }

        _meetingService.Create(request);

        return Ok();
    }
    
    [HttpGet]
    public async Task<ActionResult<List<GetMeetingResponse>?>> GetAll([FromQuery] GetMeetingRequest request)
    {
        var authorizedEmail = User.FindFirstValue(ClaimTypes.Email);

        if (authorizedEmail == null)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        var authorizedUser = await _userService.GetByEmail(authorizedEmail);

        if (authorizedUser == null)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        
        request.UserId = authorizedUser.Id;
        
        var meeting = await _meetingService.GetAll(request);

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
    
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult> Patch(Guid id, [FromBody] UpdateMeetingRequest request)
    {
        var authorizedEmail = User.FindFirstValue(ClaimTypes.Email);

        if(authorizedEmail == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        var authorizedUser = await _userService.GetByEmail(authorizedEmail);
        
        if (authorizedUser == null)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        request.AttendeeId = authorizedUser.Id;
        var success = await _meetingService.Update(id, request);

        if (!success)
        {
            return NotFound();
        }
        
        return Ok();
    }
    
    // TODO: Move this to fluent validation
    private static bool IsMissingCreatorInfo(CreateMeetingRequest request, string? authorizedEmail)
    {
        return authorizedEmail == null
               && (request.CreatorEmailAddress == null
                   || request.CreatorFirstName == null
                   || request.CreatorLastName == null
                   || request.CreatorPhoneNumber == null);
    }
    
    private async Task<CreateUserResponse?> CreateUser(CreateMeetingRequest request)
    {
        var newUser = new CreateUserRequest
        {
            FirstName = request.CreatorFirstName,
            LastName = request.CreatorLastName,
            EmailAddress = request.CreatorEmailAddress,
            PhoneNumber = request.CreatorPhoneNumber,
        };

        var createdUser = await _userService.Create(newUser);
        return createdUser;
    }
    
    private static bool DoesUserInfoMismatch(CreateMeetingRequest request, GetUserResponse userInfo)
    {
        return userInfo.FirstName != request.CreatorFirstName
               || userInfo.LastName != request.CreatorLastName
               || userInfo.PhoneNumber != request.CreatorPhoneNumber;
    }
    
    private static void AssignUserToRequest(GetUserResponse user, CreateMeetingRequest request)
    {
        request.CreatorId = user.Id;
        request.CreatorEmailAddress = user.EmailAddress;
    }
}