using api.Controllers.Models;

namespace api.Services;

public interface IUserService
{
    Task Create(CreateUserRequest request);
    Task<string?> GetProfileNameByUserEmail(string email);
    Task<bool> Exists(string profileName, long userId);
    Task<List<GetWeeklyHoursResponse>> GetWeeklyHours(long id);
    Task<bool> UpdateWeeklyHours(long id, List<UpdateWeeklyHoursRequest> weeklyHours);
    Task<List<GetDateSpecificHoursResponse>> GetDateSpecificHours(long id);
    Task<bool> UpsertDateSpecificHour(long id, UpsertDateSpecificHourRequest newDateSpecificHour);
    Task<bool> DeleteDateSpecificHour(long id, Guid dateSpecificHourId);
}