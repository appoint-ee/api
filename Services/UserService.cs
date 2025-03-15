using api.Controllers.Models;
using api.Data;
using api.Data.Entities;
using api.Services.Dtos;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class UserService : IUserService
{
    private readonly DataContext _context;

    public UserService(DataContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<IEnumerable<GetUserResponse>> GetAll()
    {
        return await _context.Users
            .Select(u => new GetUserResponse()
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                EmailAddress = u.EmailAddress,
                PhotoUrl = u.PhotoUrl,
                City = u.City,
                Country = u.Country,
                PhoneNumber = u.PhoneNumber
                
            })
            .ToListAsync();
    }
    
    public async Task<GetUserResponse> GetById(long id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return null;

        return new GetUserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailAddress = user.EmailAddress,
            PhotoUrl = user.PhotoUrl,
            City = user.City,
            Country = user.Country,
            PhoneNumber = user.PhoneNumber
        };
    }
    
    public async Task<string?> GetProfileNameByUserEmail(string email)
    {
        var user = await _context.Users.Include(x => x.Profile).SingleOrDefaultAsync(x => x.EmailAddress == email);
        return user?.Profile?.ProfileName;
    }

    public async Task<bool> Exists(string profileName, long userId)
    {
        var profile = await _context.Profiles.Include(x => x.Users)
            .SingleOrDefaultAsync(x => x.ProfileName == profileName);
        return profile?.Users.Any(u => u.Id == userId) ?? false;
    }

    public async Task<CreateUserResponse?> Create(CreateUserRequest request)
    {
        var exist = await _context.Users.AnyAsync(x =>
            x.EmailAddress == request.Email);

        if (exist)
        {
            return null;
        }

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailAddress = request.Email,
            PhotoUrl = request.PhotoUrl,
            Status = "Created",
            City = request.City,
            Country = request.Country,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = default,
            UpdatedAt = default
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return new CreateUserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailAddress = user.EmailAddress,
            PhotoUrl = user.PhotoUrl,
            City = user.City,
            Country = user.Country,
            PhoneNumber = user.PhoneNumber
        };
    }

    public async Task<bool> Update(long id, UpdateUserRequest userRequest)
    {
        var existingUser = await _context.Users.Where(s => s.Id == id).FirstOrDefaultAsync();
        if (existingUser == null) return false;
           
        existingUser.FirstName = userRequest.FirstName;
        existingUser.LastName = userRequest.LastName;
        existingUser.EmailAddress = userRequest.EmailAddress;
        existingUser.PhotoUrl = userRequest.PhotoUrl;
        existingUser.City = userRequest.City;
        existingUser.Country = userRequest.Country;
        existingUser.PhoneNumber = userRequest.PhoneNumber;
        existingUser.UpdatedAt = DateTime.UtcNow;
          
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<GetWeeklyHoursResponse>> GetWeeklyHours(long id)
    {
        return await _context.WeeklyHours
            .Where(a => a.UserId == id)
            .Select(a => new GetWeeklyHoursResponse
            {
                Id = a.Id,
                UserId = a.UserId,
                DayOfWeek = a.DayOfWeek,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            })
            .ToListAsync();
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

    public async Task<List<GetDateSpecificHoursResponse>> GetDateSpecificHours(long id)
    {
        return await _context.DateSpecificHours
            .Where(a => a.UserId == id)
            .Select(a => new GetDateSpecificHoursResponse()
            {
                Id = a.Id,
                UserId = a.UserId,
                SpecificDate = a.SpecificDate,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            })
            .ToListAsync();    }

    public async Task<bool> UpsertDateSpecificHour(long id, UpsertDateSpecificHourRequest newDateSpecificHour)
    {
        var existingDateSpecificHours = _context.DateSpecificHours
            .Where(x => x.UserId == id );

        // TODO:
        // var rangeStart = newDateSpecificHour.StartTime;
        // var rangeEnd = newDateSpecificHour.EndTime;
        // var anyOverlapping = await existingDateSpecificHours.AnyAsync(x => x.Id != newDateSpecificHour.Id 
        //                                                                    && x.SpecificDate == newDateSpecificHour.SpecificDate
        //                                                                    && (rangeStart <= x.StartTime && x.StartTime <= rangeEnd
        //                                                                        || rangeStart <= x.EndTime && x.EndTime <= rangeEnd
        //                                                                        || x.StartTime <= rangeStart && rangeEnd <= x.EndTime));
        // if (anyOverlapping)
        // {
        //     return false;
        // }

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