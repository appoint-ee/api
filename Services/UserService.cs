using api.Controllers.Models;
using api.Data;
using api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class UserService : IUserService
{
    private readonly DataContext _context;

    public UserService(DataContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public GetUserByUserNameResponse? Get(string userName)
    {
        return _context.Users
            .Where(u => u.UserName == userName)
            .Select(u => new GetUserByUserNameResponse()
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    EmailAddress = u.EmailAddress,
                    PhotoUrl = u.PhotoUrl,
                    Status = u.Status,
                    Address = u.Address,
                    CountryCode = u.CountryCode,
                    LangCode = u.LangCode,
                    PreferredTimeZone = u.PreferredTimeZone
                })
            .FirstOrDefault();
    }

    public async Task<long?> GetId(string userName)
    {
        var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == userName);
        return user?.Id;
    }

    public async Task<bool> Exists(long id)
    {
        return await _context.Users
            .AnyAsync(u => u.Id == id);
    }

    public async Task<bool> UpdateAvailabilityHours(long id, List<UpdateAvailabilityHoursRequest> availabilityHours)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var existingAvailability = _context.AvailabilityHours
                .Where(a => a.UserId == id);
            
            _context.AvailabilityHours.RemoveRange(existingAvailability);

            foreach (var availability in availabilityHours)
            {
                var newAvailability = new AvailabilityHour()
                {
                    UserId = id,
                    DayOfWeek = availability.DayOfWeek,
                    StartTime = availability.StartTime,
                    EndTime = availability.EndTime,
                    CreatedAt = DateTime.Now,
                };

                _context.AvailabilityHours.Add(newAvailability);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            // TODO: log ex
            return false;
        }
    }
}