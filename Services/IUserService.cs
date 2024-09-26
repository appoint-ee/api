using api.Controllers.Models;

namespace api.Services;

public interface IUserService
{
    GetUserByUserNameResponse? Get(string userName);
    Task<long?> GetId(string userName);
    Task<bool> Exists(long id);
    Task<bool> UpdateAvailabilityHours(long id, List<UpdateAvailabilityHoursRequest> availabilityHours);
}