using System.Globalization;
using api.Controllers.Models;
using api.Data;
using api.Data.Entities;
using api.Data.Enums;
using api.Extensions;
using api.Models;
using api.Services.Dtos;
using Microsoft.EntityFrameworkCore;
using static System.Enum;

namespace api.Services;

public class MeetingService : IMeetingService
{
    private readonly DataContext _dataContext;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IGoogleEventService _googleEventService;
    
    public MeetingService(
        DataContext dataContext,
        IGoogleAuthService googleAuthService,
        IGoogleEventService googleEventService)
    {
        _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        _googleAuthService = googleAuthService ?? throw new ArgumentNullException(nameof(googleAuthService));
        _googleEventService = googleEventService ?? throw new ArgumentNullException(nameof(googleEventService));
    }

    public void Create(CreateMeetingRequest request)
    {
        var hostExist = _dataContext.Users.Any(u => u.Id == request.HostId);

        if (!hostExist)
        {
            throw new Exception("Host not found!");
        }
        
        var service = _dataContext.Services
            .Include(s => s.UserServices)
            .SingleOrDefault(u => u.Id == request.ServiceId);

        if (service == null)
        {
            throw new Exception("Service not found!");
        }

        var host = service
            .UserServices
            .FirstOrDefault(us => us.UserId == request.HostId
                && us.ServiceId == request.ServiceId);
        
        if (host == null)
        {
            throw new Exception("The selected host does not offer this service!");
        }

        var meeting = new Meeting()
        {
            Title = service.Name,
            Description = request.Description,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            ServiceId = request.ServiceId,
            Duration = host.Duration,
            Price = host.Price,
            CreatedAt = DateTime.Now
        };
        
        _dataContext.Meetings.Add(meeting);

        var attendees = new List<MeetingAttendee>()
        {
            new()
            {
                UserId = request.HostId,
                IsHost = true,
                CreatedAt = DateTime.Now,
                Meeting = meeting
            },
            new()
            { 
                UserId = request.CreatorId, 
                IsHost = false, 
                CreatedAt = DateTime.Now, 
                Meeting = meeting
            }
        };
 
        _dataContext.MeetingAttendees.AddRange(attendees);
        _dataContext.SaveChanges();
    }
    
    public async Task<GetMeetingDetailResponse?> Get(Guid id)
    {
        var meeting = await _dataContext.Meetings
            .Include(m => m.Attendees)
            .Include(m => m.Service)
                .ThenInclude(s => s.UserServices)
            .Where(m => m.Id == id)
            .Select(m => new GetMeetingDetailResponse()
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                ServiceName = m.Service.Name,
                StartTime = m.StartTime,
                EndTime = m.EndTime,
                Duration = m.Service.UserServices
                    .FirstOrDefault(us => us.UserId == m.Attendees
                        .FirstOrDefault(ma=> ma.IsHost)!.UserId)!.Duration,
                Price = m.Service.UserServices
                    .FirstOrDefault(us => us.UserId == m.Attendees
                        .FirstOrDefault(ma => ma.IsHost)!.UserId)!.Price,
                Attendees = m.Attendees.Select(x=> new AttendeeDto()
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    IsHost = x.IsHost,
                    MeetingId = x.MeetingId,
                    Status = x.Status
                }).ToList()
            }).FirstOrDefaultAsync();
        
        return meeting;
    }

    public async Task<List<GetMeetingResponse>?> GetAll(GetMeetingRequest request)
    {
        // await SyncWithGoogleCalendar(request.UserId); 

        var query = _dataContext.MeetingAttendees
            .Where(x => x.UserId == request.UserId)
            .Select(x => x.Meeting)
            .Where(m => m.StartTime < request.StartDate && request.StartDate < m.EndTime
                        || request.StartDate < m.StartTime && m.EndTime < request.EndDate
                        || m.StartTime < request.EndDate && request.EndDate < m.EndTime);
            
        if (request.ServiceId is { Count: > 0 })
        {
            query = query.Where(m => request.ServiceId.Contains(m.ServiceId));
        }

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            query = query.Where(m => m.Title.Contains(request.Title));
        }
        
        var meetings = await query
            .Select( m => new GetMeetingResponse()
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                StartTime = m.StartTime,
                EndTime = m.EndTime,
                ServiceId = m.Service.Id,
                ServiceName = m.Service.Name,
                Duration = m.Duration,
                Price = m.Price
            }).ToListAsync();

        return meetings;
    }

    public async Task<bool> Update(Guid id, UpdateMeetingRequest request)
    {
        var meeting = await _dataContext.Meetings
            .Include(m => m.Attendees)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (meeting == null)
        {
            return false;
        }

        var isUserEligible = meeting.Attendees.Any(m => m.UserId == request.AttendeeId);
        if (!isUserEligible)
        {
            return false;
        }

        if (request.StartTime.HasValue
            && request.EndTime.HasValue)
        {
            meeting.StartTime = request.StartTime.Value;
            meeting.EndTime = request.EndTime.Value;
        }
        
        if (request.Status.HasValue)
        {
            var oneOnOneMeeting = meeting.Attendees.Count == 2;
            if (oneOnOneMeeting)
            {
                foreach (var attendee in meeting.Attendees)
                {
                    attendee.Status = request.Status.Value;
                }
            }
            else
            {
                var attendee = meeting.Attendees.FirstOrDefault(a => a.UserId == request.AttendeeId);
                attendee!.Status = request.Status.Value;
            }
        }

        await _dataContext.SaveChangesAsync();
        return true;
    }

    private async Task SyncWithGoogleCalendar(long userId)
    {
        var startTime = DateTime.Today.AddMonths(-1);
        var endTime = DateTime.Today.AddYears(1);
        
        var googleAuth = _googleAuthService.Get(userId); // TODO: Combine Get and Update methods

        if (googleAuth != null
            && !string.IsNullOrWhiteSpace(googleAuth.RefreshToken))
        {
            try
            {
                var accessToken = await _googleAuthService.Update(googleAuth.UserId, googleAuth.RefreshToken);

                var googleMeetings = await _googleEventService.Get(
                    accessToken, 
                    startTime.ToString("yyyy-MM-ddTHH:mm:sszzz"), 
                    endTime.ToString("yyyy-MM-ddTHH:mm:sszzz")); // TODO: return directly meetings instead

                if (!string.IsNullOrWhiteSpace(googleMeetings.Content))
                {
                    var apiResponse = googleMeetings.Content.DeserializeFromCamelCase<GoogleCalendarEvents>();

                    if (apiResponse != null && apiResponse.Items.Count > 0)
                    {
                        var items = apiResponse.Items.Where(x => x.Recurrence == null).ToList(); // TODO: Handle recurring events

                        await UpdateGoogleMeetings(googleAuth.UserId,
                            items, 
                            startTime.ToUniversalTime(), 
                            endTime.ToUniversalTime(),
                            apiResponse.TimeZone);
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: Log the exception
            }
        }
    }
    
    private async Task UpdateGoogleMeetings(long userId,  List<GoogleCalendarEventItem> googleEvents, DateTime timeMin, DateTime timeMax, string timeZone)
    {
        var meetings = _dataContext.MeetingAttendees
            .Where(x => x.UserId == userId)
            .Select(x => x.Meeting)
            .Where(m => m.StartTime <= timeMin && timeMin <= m.EndTime
                        || m.StartTime >= timeMin && timeMax >= m.EndTime
                        || m.StartTime <= timeMax && timeMax <= m.EndTime
                        || timeMin <= m.StartTime && m.EndTime <= timeMax)
            .ToList();

        foreach (var googleEvent in googleEvents)
        { 
            var startTime= GetGoogleEventTimes(googleEvent.Start, timeZone); 
            var endTime= GetGoogleEventTimes(googleEvent.End, timeZone);
            
            if (startTime is null
                || endTime is null) 
            { 
                continue;
            }
                
            var meeting = meetings.FirstOrDefault(x => x.ExternalId == googleEvent.Id);
            
            if (meeting == null)
            {
                var tryStatusParse = TryParse(googleEvent.Status, true, out AttendeeStatus attendeeStatus);
                
                if (!tryStatusParse) 
                { 
                    // TODO: Log exception
                }
                
                meeting = new Meeting() 
                {
                    Title = googleEvent.Summary, 
                    StartTime = startTime!.Value, 
                    EndTime = endTime!.Value, 
                    ExternalId = googleEvent.Id, 
                    Description = googleEvent.Description ?? "", 
                    CreatedAt = DateTime.Now
                };
                    
                var attendees = new List<MeetingAttendee>() 
                { 
                    new() 
                    { 
                        UserId = userId, 
                        IsHost = false, 
                        Status = attendeeStatus, 
                        CreatedAt = DateTime.Now, 
                        Meeting = meeting
                    } 
                };

                _dataContext.Meetings.Add(meeting); 
                _dataContext.MeetingAttendees.AddRange(attendees);
            }
            else if (meeting.StartTime != startTime!.Value 
                    || meeting.EndTime != endTime!.Value
                    || meeting.Title != googleEvent.Summary)
            {
                meeting.StartTime = startTime!.Value; 
                meeting.EndTime = endTime!.Value;
                meeting.Title = googleEvent.Summary; 
                meeting.Description = googleEvent.Description;
            }
        }

        await _dataContext.SaveChangesAsync();
    }

    private static DateTime? GetGoogleEventTimes(GoogleCalendarEventDateTime googleCalendarEventDateTime, string timeZone)
    {
        if (googleCalendarEventDateTime.DateTime.HasValue)
        {
            return TimeZoneInfo.ConvertTimeToUtc(googleCalendarEventDateTime.DateTime.Value);
        }

        if (!DateTime.TryParseExact(googleCalendarEventDateTime.Date,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedDateTime))
        {
            return null;
        }
        
        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        
        return TimeZoneInfo.ConvertTimeToUtc(parsedDateTime, timeZoneInfo);
    }
}