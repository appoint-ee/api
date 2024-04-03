using System.Net;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using api.Extensions;
using RestSharp;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController : Controller
{
    private const string ApiUrl = "https://www.googleapis.com/calendar/v3/calendars/primary/events";

    private readonly IConfiguration _configuration;
    private readonly IRestClient _restClient;

    public EventsController(
        IConfiguration configuration,
        IRestClient restClient)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    [HttpPost]
    public async Task<ActionResult> Create(Event calendarEvent)
    {
        var accessToken = GetAccessToken();

        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Unauthorized: Access token is invalid or missing.");
        }
        
        var request = new RestRequest(ApiUrl);
        request.AddQueryParameter ("key", _configuration["GoogleAPI:Key"]);
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        request.AddHeader ("Accept", "application/json");
        request.AddHeader("Content-Type","application/json");
        request.AddParameter("application/json", calendarEvent.SerializeWithCamelCase(), ParameterType.RequestBody);
        
        var response = await _restClient.ExecutePostAsync(request);

        return StatusCode((int)response.StatusCode);
    }
    
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var accessToken = GetAccessToken();
        
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Unauthorized: Access token is invalid or missing.");
        }
        
        var request = new RestRequest(ApiUrl);
        request.AddQueryParameter ("key", _configuration["GoogleAPI:Key"]);
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        request.AddHeader ("Accept", "application/json");
        
        var response = await _restClient.ExecuteGetAsync(request);

        if (response.StatusCode != System.Net.HttpStatusCode.OK
            || response.Content == null) return StatusCode((int)response.StatusCode);

        var jsonDocument = JsonDocument.Parse(response.Content);
        var root = jsonDocument.RootElement;
        if (!root.TryGetProperty("items", out var fieldValue))
            return StatusCode((int)response.StatusCode);
        
        var fieldValueString = fieldValue.ToString();
        return Ok(fieldValueString);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(string id)
    {
        var accessToken = GetAccessToken();
        
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Unauthorized: Access token is invalid or missing.");
        }
        
        var request = new RestRequest($"{ApiUrl}/{id}");
        request.AddQueryParameter ("key", _configuration["GoogleAPI:Key"]);
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        request.AddHeader ("Accept", "application/json");
        
        var response = await _restClient.ExecuteGetAsync(request);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            return Ok(response.Content);
        }
        
        return StatusCode((int)response.StatusCode);
    }
    
    [HttpPatch("{id}")]
    public async Task<ActionResult> Update(string id, Event calendarEvent)
    {
        var accessToken = GetAccessToken();

        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Unauthorized: Access token is invalid or missing.");
        }

        var request = new RestRequest($"{ApiUrl}/{id}", Method.Patch);
        request.AddQueryParameter ("key", _configuration["GoogleAPI:Key"]);
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        request.AddHeader ("Accept", "application/json");
        request.AddHeader("Content-Type","application/json");
        request.AddParameter("application/json", calendarEvent.SerializeWithCamelCase(), ParameterType.RequestBody);
        
        var response = await _restClient.ExecuteAsync(request);

        return StatusCode((int)response.StatusCode);
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var accessToken = GetAccessToken();
        
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Unauthorized: Access token is invalid or missing.");
        }
        
        var request = new RestRequest($"{ApiUrl}/{id}", Method.Delete);
        request.AddQueryParameter ("key", _configuration["GoogleAPI:Key"]);
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        request.AddHeader ("Accept", "application/json");
        
        var response = await _restClient.ExecuteAsync(request);
        
        return StatusCode((int)response.StatusCode);
    }
    
    private string? GetAccessToken()
    {
        var authorizationHeader = HttpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            return null;
        }

        return authorizationHeader["Bearer ".Length..];
    }
}