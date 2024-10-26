using api.Controllers.Models;

namespace api.Services;

public interface IUserService
{
    GetProfileDetailsResponse? GetProfileDetails(string userName);
    Task<long?> GetUserId(string profileName);
    Task<bool> Exists(string profileName, long userId);
    Task<bool> UpdateAvailabilityHours(long id, List<UpdateAvailabilityHoursRequest> availabilityHours);
}