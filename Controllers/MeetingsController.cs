using Microsoft.AspNetCore.Mvc;
using api.Controllers.Models;
using api.Services;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class MeetingsController : ApiControllerBase
{
    private readonly IMeetingService _meetingService;

    public MeetingsController(
        IMeetingService meetingService)
    {
        _meetingService = meetingService ?? throw new ArgumentNullException(nameof(meetingService));
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
        var meeting = await _meetingService.Get(userName, start, end);

        if (meeting == null)
        {
            return NotFound();
        }
        
        return Ok(meeting);
    }
}