using api.Models;
using RestSharp;

namespace api.Services;

public interface IGoogleEventService 
{
    Task<RestResponse> Create(string accessToken, Event calendarEvent);
    Task<RestResponse> Get(string accessToken, string? startTime = null, string? endTime = null);
    Task<RestResponse> GetById(string accessToken, string id);
    Task<RestResponse> Update(string accessToken, string id, Event calendarEvent);
    Task<RestResponse> Delete(string accessToken, string id);
}