using api.Controllers.Models;
using api.Data;
using api.Data.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class ProfileService : IProfileService
{
    private readonly DataContext _context;

    public ProfileService(DataContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task Create(CreateProfileRequest request)
    {
        var profile = new Profile
        {
            ProfileName = request.ProfileName,
            EmailAddress = request.EmailAddress,
            PhotoUrl = request.PhotoUrl,
            Address = request.Address,
            CountryCode = request.CountryCode,
            LangCode = request.LangCode,
            PreferredTimeZone = request.PreferredTimeZone,
            IsOrg = request.IsOrg,
            CreatedAt = default,
            UpdatedAt = default
        };
        
        _context.Profiles.Add(profile);
        
        await _context.SaveChangesAsync();

        var users = _context.Users.Where(x => request.UserIds.Contains(x.Id));

        foreach (var user in users)
        {
            user.Status = "Admin"; // TODO:
            user.ProfileId = profile.Id;
        }
        
        await _context.SaveChangesAsync();
    }

    public GetProfileDetailsResponse? GetDetails(string name)
    {
        var profile = _context.Profiles
            .Where(profile => profile.ProfileName == name)
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
                Hosts = GetHosts(profile),
                Services = GetServices(profile).ToList(),
            }).FirstOrDefault();

        if (profile != null)
        {
            var hostIds = profile.Hosts.Select(x => x.Id).ToList();
            profile.WeeklyHours = GetWeeklyHours(hostIds);
            profile.DateSpecificHours = GetDateSpecificHours(hostIds);
        }

        return profile;
    }

    public async Task<bool> Patch(long id, JsonPatchDocument? patchDoc)
    {
        var profile = GetById(id);

        if (profile == null)
        {
            return false;
        }

        patchDoc.ApplyTo(profile);

        await _context.SaveChangesAsync();
        return true;
    }

    private static List<ServiceDto> GetServices(Profile profile)
    {
        return profile.Users
            .SelectMany(u => u.UserServices)
            .GroupBy(us => us.Service.Id)
            .Select(g => new ServiceDto
            {
                Id = g.Key,
                Name = g.First().Service.Name,
                Duration = g.First().Service.DefaultDuration,
                Price = g.First().Service.DefaultPrice
            })
            .ToList();
    }
    
    private static List<HostDto> GetHosts(Profile profile)
    {
        return profile.Users
            .SelectMany(u => u.UserServices.Select(us => new
            {
                ServiceId = us.Service.Id,
                Service = new ServiceDto
                {
                    Id = us.Service.Id,
                    Name = us.Service.Name,
                    Duration = us.Duration,
                    Price = us.Price
                },
                HostId = u.Id,
                Host = new HostDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhotoUrl = u.PhotoUrl,
                    Status = u.Status
                }
            }))
            .GroupBy(x => x.HostId)
            .Select(g => new HostDto()
            {
                Id = g.Key,
                FirstName = g.First().Host.FirstName,
                LastName = g.First().Host.LastName,
                Services = g.Select(x => x.Service)
                    .DistinctBy(h => h.Id)
                    .ToList()
            }).ToList();
    }
    
    private List<WeeklyHourDto> GetWeeklyHours(List<long> userIds)
    {
        var weeklyHours = _context.WeeklyHours
            .Where(x => userIds.Contains(x.UserId))
            .Select(w => new WeeklyHourDto()
            {
                Id = w.Id,
                DayOfWeek = w.DayOfWeek,
                StartTime = w.StartTime,
                EndTime = w.EndTime,
                UserId = w.UserId
            }).ToList();

        return weeklyHours;
    }
    
    private List<DateSpecificHourDto> GetDateSpecificHours(List<long> userIds)
    {
        var dateSpecificHours = _context.DateSpecificHours
            .Where(x => userIds.Contains(x.UserId))
            .Select(d => new DateSpecificHourDto()
            { 
                Id = d.Id, 
                SpecificDate = d.SpecificDate,
                StartTime = d.StartTime,
                EndTime = d.EndTime,
                UserId = d.UserId
            }).ToList();

        return dateSpecificHours;
    }
    
    private Profile? GetById(long id)
    {
        return _context.Profiles
            .FirstOrDefault(profile => profile.Id == id);
    }
}