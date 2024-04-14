using System.Net;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using api.Services;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController : ApiControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(
        IEventService eventService)
    {
        _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] Event calendarEvent)
    {
        var accessToken = GetAccessToken();

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return Unauthorized("Unauthorized: Access token is invalid or missing.");
        }
        
        var response = await _eventService.Create(accessToken, calendarEvent);

        return StatusCode((int)response.StatusCode);
    }
    
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var accessToken = GetAccessToken();
        
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return Unauthorized("Unauthorized: Access token is invalid or missing.");
        }

        var response = await _eventService.Get(accessToken);

        if (response.StatusCode != HttpStatusCode.OK
            || string.IsNullOrWhiteSpace(response.Content))
        {
            return StatusCode((int)response.StatusCode);
        }

        var jsonDocument = JsonDocument.Parse(response.Content).RootElement;

        if (!jsonDocument.TryGetProperty("items", out var events))
        {
            return StatusCode((int)response.StatusCode);
        }
        
        return Ok(events);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult> GetById([FromRoute] string id)
    {
        var accessToken = GetAccessToken();
        
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return Unauthorized("Unauthorized: Access token is invalid or missing.");
        }

        var response = await _eventService.GetById(accessToken, id);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            return Content(response.Content, "application/json");
        }
        
        return StatusCode((int)response.StatusCode);
    }
    
    [HttpPatch("{id}")]
    public async Task<ActionResult> Update([FromRoute] string id, [FromBody] Event calendarEvent)
    {
        var accessToken = GetAccessToken();

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return Unauthorized("Unauthorized: Access token is invalid or missing.");
        }
        
        var response = await _eventService.Update(accessToken, id, calendarEvent);

        return StatusCode((int)response.StatusCode);
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] string id)
    {
        var accessToken = GetAccessToken();
        
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return Unauthorized("Unauthorized: Access token is invalid or missing.");
        }

        var response = await _eventService.Delete(accessToken, id);
        
        return StatusCode((int)response.StatusCode);
    }
}