using api.Controllers.Models;

namespace api.Services;

public interface IUserService
{
    GetProfileDetailsResponse? GetProfileDetails(string userName);
    Task<long?> GetUserId(string profileName);
    Task<bool> Exists(string profileName, long userId);
    Task<bool> UpdateWeeklyHours(long id, List<UpdateWeeklyHoursRequest> weeklyHours);
    Task<bool> UpsertDateSpecificHour(long id, UpsertDateSpecificHourRequest newDateSpecificHour);
    Task<bool> DeleteDateSpecificHour(long id, Guid dateSpecificHourId);
}