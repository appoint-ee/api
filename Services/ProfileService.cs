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
        _context.Profiles.Add(
            new Profile
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
            });
        
        await _context.SaveChangesAsync();
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
    
    private Profile? GetById(long id)
    {
        return _context.Profiles
            .FirstOrDefault(profile => profile.Id == id);
    }
}