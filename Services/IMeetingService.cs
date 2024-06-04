using api.Controllers.Models;

namespace api.Services;

public interface IMeetingService
{
    void Create(CreateMeetingRequest request);
    List<GetMeetingResponse>? Get(string userName, DateTime start, DateTime end);
}