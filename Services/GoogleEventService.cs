using api.Extensions;
using api.Models;
using RestSharp;

namespace api.Services;

public class GoogleEventService : IGoogleEventService
{
    private const string ApiUrl = "https://www.googleapis.com/calendar/v3/calendars/primary/events";

    private readonly IConfiguration _configuration;
    private readonly IRestClient _restClient;

    public GoogleEventService(
        IConfiguration configuration,
        IRestClient restClient)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public async Task<RestResponse> Create(string accessToken, Event calendarEvent)
    {
        var request = new RestRequest(ApiUrl, Method.Post);
        
        request.AddQueryParameter("key", _configuration["GoogleAPI:Key"]);
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type","application/json");
        request.AddParameter("application/json", calendarEvent.SerializeWithCamelCase(), ParameterType.RequestBody);

        var response = await _restClient.ExecuteAsync(request);
        
        return response;    
    }

    public async Task<RestResponse> Get(string accessToken)
    {
        var request = new RestRequest(ApiUrl);
        
        request.AddQueryParameter("key", _configuration["GoogleAPI:Key"]);
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        request.AddHeader("Accept", "application/json");

        var response = await _restClient.ExecuteAsync(request);
        
        return response;
    }
    
    public async Task<RestResponse> GetById(string accessToken, string id)
    {
        var request = new RestRequest($"{ApiUrl}/{id}");
        
        request.AddQueryParameter("key", _configuration["GoogleAPI:Key"]);
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        request.AddHeader("Accept", "application/json");

        var response = await _restClient.ExecuteAsync(request);
        
        return response;
    }
    
    public async Task<RestResponse> Update(string accessToken, string id, Event calendarEvent)
    {
        var request = new RestRequest($"{ApiUrl}/{id}", Method.Patch);
        
        request.AddQueryParameter("key", _configuration["GoogleAPI:Key"]);
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type","application/json");
        request.AddParameter("application/json", calendarEvent.SerializeWithCamelCase(), ParameterType.RequestBody);

        var response = await _restClient.ExecuteAsync(request);
        
        return response;
    }

    public async Task<RestResponse> Delete(string accessToken, string id)
    {
        var request = new RestRequest($"{ApiUrl}/{id}", Method.Delete);
        
        request.AddQueryParameter("key", _configuration["GoogleAPI:Key"]);
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        request.AddHeader("Accept", "application/json");

        var response = await _restClient.ExecuteAsync(request);
        
        return response;
    }
}