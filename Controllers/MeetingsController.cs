using Microsoft.AspNetCore.Mvc;
using api.Controllers.Models;
using api.Services;

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

    [HttpPost]
    public ActionResult Create([FromBody] CreateMeetingRequest request)
    {
        _meetingService.Create(request);

        return Ok();
    }
    
    [HttpGet]
    public async Task<ActionResult<List<GetMeetingResponse>?>> Get([FromQuery] string userName, DateTime start, DateTime end)
    {
        var userId  = await _userService.GetId(userName);

        if (userId is null)
        {
            return BadRequest();
        }
        
        var meeting = await _meetingService.Get(userId.Value, start, end);

        if (meeting == null)
        {
            return NotFound();
        }
        
        return Ok(meeting);
    }
}