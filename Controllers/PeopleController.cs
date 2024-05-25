using System.Net;
using System.Text.Json;
using api.Controllers.Models;
using api.Data;
using api.Data.Entities;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestSharp;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class PeopleController : ApiControllerBase
{
    private const string ApiUrl = "https://people.googleapis.com/v1/people/me";

    private readonly DataContext _context;
    private readonly IConfiguration _configuration;
    private readonly IRestClient _restClient;
    
    public PeopleController(
        DataContext context,
        IConfiguration configuration,
        IRestClient restClient)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }
    
    [HttpGet]
    public async Task<ActionResult<GetUserInfoResponse>> GetUserInfo()
    {
        var accessToken = GetAccessToken();
        
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Unauthorized: Access token is invalid or missing.");
        }
        
        var request = new RestRequest(ApiUrl);
        request.AddQueryParameter ("key", _configuration["GoogleAPI:Key"]);
        request.AddQueryParameter ("personFields", "emailAddresses,names,phoneNumbers,addresses,locales,photos");
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        request.AddHeader ("Accept", "application/json");
        
        var response = await _restClient.ExecuteGetAsync(request);

        if (response.StatusCode == HttpStatusCode.OK 
            && !string.IsNullOrEmpty(response.Content))
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var apiResponse = JsonSerializer.Deserialize<GetGoogleApiPeopleResponse>(response.Content, options);

            if (apiResponse == null)
            {
                return NotFound();
            }
            
            // TODO: Google responses return arrays - if we use this, we could prioritize those with primary:true
            return Ok(new GetUserInfoResponse()
            {
                Locale = apiResponse.Locales?.FirstOrDefault()?.Value,
                Name = apiResponse.Names?.FirstOrDefault()?.DisplayName,
                PhotoUrl = apiResponse.Photos?.FirstOrDefault()?.Url,
                Address = apiResponse.Addresses?.FirstOrDefault()?.FormattedValue,
                EmailAddress = apiResponse.EmailAddresses?.FirstOrDefault()?.Value,
                PhoneNumber = apiResponse.PhoneNumbers?.FirstOrDefault()?.CanonicalForm
            });
        }
        
        return StatusCode((int)response.StatusCode);
    }

    [HttpGet("{userName}")]
    public async Task<User?> GetPersonByUserName(string userName)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
    }
}