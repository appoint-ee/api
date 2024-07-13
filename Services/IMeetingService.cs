using api.Controllers.Models;

namespace api.Services;

public interface IMeetingService
{
    void Create(CreateMeetingRequest request);
    Task<List<GetMeetingResponse>?> Get(string userName, DateTime timeMin, DateTime timeMax);
}