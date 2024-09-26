using api.Models;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class SlotsController : ApiControllerBase
{
    private readonly IMeetingService _meetingService;
    private readonly ISlotService _slotService;
    private readonly IUserService _userService;

    public SlotsController(
        IMeetingService meetingService,
        ISlotService slotService,
        IUserService userService)
    {
        _meetingService = meetingService ?? throw new ArgumentNullException(nameof(meetingService));
        _slotService = slotService ?? throw new ArgumentNullException(nameof(slotService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [HttpGet("time")] 
    public async Task<ActionResult<List<TimeSlot>>> GetTimeSlots([FromQuery] string userName, DateTime start, DateTime end)
    {
        var userId  = await _userService.GetId(userName);

        if (userId is null)
        {
            return BadRequest();
        }
        
        var meetings = await _meetingService.Get(userId.Value, start, end);

        var timeSlots = _slotService.GenerateTimeSlots(userId.Value, start, end, meetings);

        return Ok(timeSlots);
    }
    
    [HttpGet("day")]
    public async Task<ActionResult<List<DaySlot>>> GetDaySlots([FromQuery] string userName, DateTime start, DateTime end)
    {
        var userId  = await _userService.GetId(userName);

        if (userId is null)
        {
            return BadRequest();
        }
        
        var meetings = await _meetingService.Get(userId.Value, start, end);

        var daySlots = _slotService.GenerateDaySlots(userId.Value, start, end, meetings);

        return Ok(daySlots);
    }
}