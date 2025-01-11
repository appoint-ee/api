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

    public async Task Create(CreateUserRequest request)
    {
        _context.Users.Add(
            new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailAddress = request.Email,
                PhotoUrl = request.PhotoUrl,
                Status = "Created",
                CreatedAt = default,
                UpdatedAt = default
            });
        
        await _context.SaveChangesAsync();
    }

    public async Task<long?> GetUserId(string profileName)
    {
        var profile = await _context.Profiles.SingleOrDefaultAsync(x => x.ProfileName == profileName);
        return profile?.Users.SingleOrDefault()?.Id;
    }

    public async Task<bool> Exists(string profileName, long userId)
    {
        var profile = await _context.Profiles.Include(x => x.Users)
            .SingleOrDefaultAsync(x => x.ProfileName == profileName);
        return profile?.Users.Any(u => u.Id == userId) ?? false;
    }

    public async Task<bool> UpdateWeeklyHours(long id, List<UpdateWeeklyHoursRequest> weeklyHours)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var existingWeeklyHours = _context.WeeklyHours
                .Where(a => a.UserId == id);
            
            _context.WeeklyHours.RemoveRange(existingWeeklyHours);

            foreach (var weeklyHour in weeklyHours)
            {
                _context.WeeklyHours.Add
                (
                    new WeeklyHour()
                    {
                        UserId = id,
                        DayOfWeek = weeklyHour.DayOfWeek,
                        StartTime = weeklyHour.StartTime,
                        EndTime = weeklyHour.EndTime,
                        CreatedAt = DateTime.Now,
                    });
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

    public async Task<bool> UpsertDateSpecificHour(long id, UpsertDateSpecificHourRequest newDateSpecificHour)
    {
        var existingDateSpecificHours = _context.DateSpecificHours
            .Where(x => x.UserId == id );

        var rangeStart = newDateSpecificHour.StartTime;
        var rangeEnd = newDateSpecificHour.EndTime;

        var anyOverlapping = await existingDateSpecificHours.AnyAsync(x => x.Id != newDateSpecificHour.Id 
                                                     && x.SpecificDate == newDateSpecificHour.SpecificDate
                                                     && (rangeStart <= x.StartTime && x.StartTime <= rangeEnd
                                                        || rangeStart <= x.EndTime && x.EndTime <= rangeEnd
                                                        || x.StartTime <= rangeStart && rangeEnd <= x.EndTime));

        if (anyOverlapping)
        {
            return false;
        }

        var dateSpecificHour = existingDateSpecificHours.FirstOrDefault(x => x.Id == newDateSpecificHour.Id);

        if (dateSpecificHour == null)
        {
            dateSpecificHour = new DateSpecificHour
            {
                UserId = id,
                SpecificDate = newDateSpecificHour.SpecificDate,
                StartTime = newDateSpecificHour.StartTime,
                EndTime = newDateSpecificHour.EndTime
            };
                
            _context.DateSpecificHours.Add(dateSpecificHour);
        }
        else
        {
            dateSpecificHour.SpecificDate = newDateSpecificHour.SpecificDate;
            dateSpecificHour.StartTime = newDateSpecificHour.StartTime;
            dateSpecificHour.EndTime = newDateSpecificHour.EndTime;
        }

        await _context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> DeleteDateSpecificHour(long id, Guid dateSpecificHourId)
    {
        var dateSpecificHour = await _context.DateSpecificHours
            .SingleOrDefaultAsync(x => x.UserId == id 
                && x.Id == dateSpecificHourId);

        if (dateSpecificHour == null)
        {
            return false;
        }
        
        _context.Remove(dateSpecificHour);
        await _context.SaveChangesAsync();   
            
        return true;

    }
}