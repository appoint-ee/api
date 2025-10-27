using api.Controllers.Models;
using api.Data;
using api.Data.Entities;
using api.Services.Dtos;
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
            IsOnline = request.IsOnline,
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
            .FirstOrDefault();
        
        if (profile == null) return null;
        var (services, hosts) = GetServicesAndHosts(profile);
        
        if (services.Count == 0 || hosts.Count == 0) return null;
        var hostIds = hosts.Select(x => x.Id).ToList();
        var weeklyHours = GetWeeklyHours(hostIds);
        var dateSpecificHours = GetDateSpecificHours(hostIds);

        return new GetProfileDetailsResponse() 
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
            IsOnline = profile.IsOnline, 
            Hosts = hosts, 
            Services = services,
            WeeklyHours = weeklyHours,
            DateSpecificHours = dateSpecificHours
        };
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

    private static (List<ServiceDto> Services, List<HostDto> Hosts) GetServicesAndHosts(Profile profile)
    {
        if (profile?.Users == null || profile.Users.Count == 0)
        {
            return (new List<ServiceDto>(), new List<HostDto>());
        }
        
        var flattenedUserServices = profile.Users
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
            }));

        if (flattenedUserServices == null)
        {
            return (new List<ServiceDto>(), new List<HostDto>());
        }

        var services =  flattenedUserServices.GroupBy(x => x.ServiceId)
            .Select(g =>
                {
                    var service = g.First().Service;

                    return new ServiceDto()
                    {
                        Id = g.Key,
                        Name = service.Name,
                        Hosts = g.Select(x => x.Host)
                            .DistinctBy(h => h.Id)
                            .ToList()
                    };
                }).ToList();

        var hosts = flattenedUserServices.GroupBy(x => x.HostId)
            .Select(g =>
            {
                var host = g.First().Host;
                
                return new HostDto
                {
                    Id = g.Key,
                    FirstName = host.FirstName,
                    LastName = host.LastName,
                    PhotoUrl = host.PhotoUrl,
                    Status = host.Status,
                    Services = g.Select(x => x.Service)
                        .DistinctBy(h => h.Id)
                        .ToList()
                };
            }).ToList();

        return (services, hosts);
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