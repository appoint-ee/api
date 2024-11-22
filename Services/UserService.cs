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
    
    public GetProfileDetailsResponse? GetProfileDetails(string profileName)
    {
        return _context.Profiles
               .Where(profile => profile.ProfileName == profileName)
               .Include(p => p.Users)
                    .ThenInclude(u => u.UserServices)
                    .ThenInclude(us => us.Service)
               .Select(profile => new GetProfileDetailsResponse()
               {
                   Id = profile.Id,
                   UserName = profile.ProfileName,
                   EmailAddress = profile.EmailAddress,
                   PhotoUrl = profile.PhotoUrl,
                   Address = profile.Address,
                   CountryCode = profile.CountryCode,
                   LangCode = profile.LangCode,
                   PreferredTimeZone = profile.PreferredTimeZone,
                   IsOrg = profile.IsOrg,
                   Services = GetServices(profile).ToList()
               }).FirstOrDefault();
    }

    private static IEnumerable<ServiceDto> GetServices(Profile profile)
    {
        return profile.Users
            .SelectMany(u => u.UserServices.Select(us => new
            {
                ServiceId = us.Service.Id,
                Service = new ServiceDto
                {
                    Id = us.Service.Id,
                    Name = us.Service.Name,
                    DefaultDuration = us.Service.DefaultDuration,
                    DefaultPrice = us.Service.DefaultPrice
                },
                Host = new HostDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhotoUrl = u.PhotoUrl,
                    Duration = us.Duration,
                    Price = us.Price
                }
            }))
            .GroupBy(x => x.ServiceId)
            .Select(g => new ServiceDto
            {
                Id = g.Key,
                Name = g.First().Service.Name,
                DefaultDuration = g.First().Service.DefaultDuration,
                DefaultPrice = g.First().Service.DefaultPrice,
                Hosts = g.Select(x => x.Host)
                    .DistinctBy(h => h.Id)
                    .ToList()
            });
    }

    public async Task<long?> GetUserId(string profileName)
    {
        var profile = await _context.Profiles.SingleOrDefaultAsync(x => x.ProfileName == profileName);
        return profile?.Users.SingleOrDefault()?.Id;
    }

    public async Task<bool> Exists(string profileName, long userId)
    {
        var profile = await _context.Profiles.Include(x => x.Users).SingleOrDefaultAsync(x => x.ProfileName == profileName);
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
}