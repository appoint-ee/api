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
    public ActionResult<List<TimeSlot>> GetTimeSlots([FromQuery] string userName, DateTime start, DateTime end)
    {
        var meetings = _meetingService.Get(userName, start, end);

        var timeSlots = _slotService.GenerateTimeSlots(start, end, meetings);

        return Ok(timeSlots);
    }
    
    [HttpGet("day")]
    public ActionResult<List<DaySlot>> GetDaySlots([FromQuery] string userName, DateTime start, DateTime end)
    {
        var meetings = _meetingService.Get(userName, start, end);

        var daySlots = _slotService.GenerateDaySlots(start, end, meetings);

        return Ok(daySlots);
    }
}