using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class PeopleController : ApiControllerBase
{
    private const string ApiUrl = "https://people.googleapis.com/v1/people/me";

    private readonly IConfiguration _configuration;
    private readonly IRestClient _restClient;
    
    public PeopleController(
        IConfiguration configuration,
        IRestClient restClient)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }
    
    [HttpGet]
    public async Task<ActionResult> GetUserInfo()
    {
        var accessToken = GetAccessToken();
        
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Unauthorized: Access token is invalid or missing.");
        }
        
        var request = new RestRequest(ApiUrl);
        request.AddQueryParameter ("key", _configuration["GoogleAPI:Key"]);
        request.AddQueryParameter ("personFields", "emailAddresses,birthdays");
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        request.AddHeader ("Accept", "application/json");
        
        var response = await _restClient.ExecuteGetAsync(request);

        if (response.StatusCode == HttpStatusCode.OK 
            && !string.IsNullOrEmpty(response.Content))
        {
            var jsonDocument = JsonDocument.Parse(response.Content); 
            return Ok(jsonDocument);
        }
        
        return StatusCode((int)response.StatusCode);
    }
}