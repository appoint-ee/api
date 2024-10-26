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

    public SlotsController(
        IMeetingService meetingService,
        ISlotService slotService)
    {
        _meetingService = meetingService ?? throw new ArgumentNullException(nameof(meetingService));
        _slotService = slotService ?? throw new ArgumentNullException(nameof(slotService));
    }

    [HttpGet("time")] 
    public async Task<ActionResult<List<TimeSlot>>> GetTimeSlots([FromQuery] long userId, DateTime start, DateTime end)
    {
        var meetings = await _meetingService.Get(userId, start, end);

        var timeSlots = _slotService.GenerateTimeSlots(userId, start, end, meetings);

        return Ok(timeSlots);
    }
    
    [HttpGet("day")]
    public async Task<ActionResult<List<DaySlot>>> GetDaySlots([FromQuery] long userId, DateTime start, DateTime end)
    {
        var meetings = await _meetingService.Get(userId, start, end);

        var daySlots = _slotService.GenerateDaySlots(userId, start, end, meetings);

        return Ok(daySlots);
    }
}