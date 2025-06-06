using api.Controllers.Models;
using api.Services.Dtos;

namespace api.Services;

public interface IMeetingService
{
    void Create(CreateMeetingRequest request);
    Task<GetMeetingDetailResponse?> Get(Guid id);
    Task<List<GetMeetingResponse>?> Get(long userId, DateTime timeMin, DateTime timeMax);
}