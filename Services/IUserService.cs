using api.Controllers.Models;
using api.Services.Dtos;

namespace api.Services;

public interface IUserService
{
    Task<IEnumerable<GetUserResponse>> GetAll();
    Task<GetUserResponse> GetById(long id);
    Task<GetUserResponse?> GetByEmail(string email);
    Task<string?> GetProfileNameByUserEmail(string email);
    Task<bool> Exists(string profileName, long userId);
    Task<CreateUserResponse?> Create(CreateUserRequest userRequest);
    Task<bool> Update(long id, UpdateUserRequest userRequest);
    Task<List<GetWeeklyHoursResponse>> GetWeeklyHours(long id);
    Task<bool> UpdateWeeklyHours(long id, List<UpdateWeeklyHoursRequest> weeklyHours);
    Task<List<GetDateSpecificHoursResponse>> GetDateSpecificHours(long id);
    Task<bool> UpsertDateSpecificHour(long id, UpsertDateSpecificHourRequest newDateSpecificHour);
    Task<bool> DeleteDateSpecificHour(long id, Guid dateSpecificHourId);
}