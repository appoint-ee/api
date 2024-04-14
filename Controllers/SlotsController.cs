using System.Net;
using System.Text.Json;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class SlotsController : ApiControllerBase
{
    private readonly IEventService _eventService;
    private readonly ISlotService _slotService;

    public SlotsController(
        IEventService eventService,
        ISlotService slotService)
    {
        _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
        _slotService = slotService ?? throw new ArgumentNullException(nameof(slotService));
    }
    
    [HttpGet("time")] 
    public async Task<ActionResult<List<TimeSlot>>> GetTimeSlots([FromQuery] DateTime start, DateTime end)
    {
        var accessToken = GetAccessToken();
        
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Unauthorized: Access token is invalid or missing.");
        }
        
        var response = await _eventService.Get(accessToken);

        if (response.StatusCode != HttpStatusCode.OK
            || response.Content == null)
        {
            return StatusCode((int)response.StatusCode);
        }

        var jsonDocument = JsonDocument.Parse(response.Content);
        if (!jsonDocument.RootElement.TryGetProperty("items", out var eventsJson))
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        
        var events = JsonSerializer.Deserialize<List<Event>>(eventsJson.ToString());

        var timeSlots = _slotService.GenerateTimeSlots(start, end, events);

       return Ok(timeSlots);
    }

    [HttpGet("day")]
    public async Task<ActionResult<List<DaySlot>>> GetDaySlots([FromQuery] DateTime start, DateTime end)
    {
        var accessToken = GetAccessToken();
        
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Unauthorized: Access token is invalid or missing.");
        }
        
        var response = await _eventService.Get(accessToken);

        if (response.StatusCode != HttpStatusCode.OK
            || response.Content == null)
        {
            return StatusCode((int)response.StatusCode);
        }

        var jsonDocument = JsonDocument.Parse(response.Content);
        if (!jsonDocument.RootElement.TryGetProperty("items", out var eventsJson))
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        
        var events = JsonSerializer.Deserialize<List<Event>>(eventsJson.ToString());

        var daySlots = _slotService.GenerateDaySlots(start, end, events);

        return Ok(daySlots);
    }
}