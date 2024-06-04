using System.Net;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using api.Services;

namespace api.Controllers;

[Obsolete]
[ApiController]
[Route("[controller]")]
public class EventsController : ApiControllerBase
{
    private readonly IGoogleEventService _googleEventService;

    public EventsController(
        IGoogleEventService googleEventService)
    {
        _googleEventService = googleEventService ?? throw new ArgumentNullException(nameof(googleEventService));
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] Event calendarEvent)
    {
        var accessToken = GetAccessToken();

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return Unauthorized("Unauthorized: Access token is invalid or missing.");
        }
        
        var response = await _googleEventService.Create(accessToken, calendarEvent);

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

        var response = await _googleEventService.Get(accessToken);

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

        var response = await _googleEventService.GetById(accessToken, id);

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
        
        var response = await _googleEventService.Update(accessToken, id, calendarEvent);

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

        var response = await _googleEventService.Delete(accessToken, id);
        
        return StatusCode((int)response.StatusCode);
    }
}