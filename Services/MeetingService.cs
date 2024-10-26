using System.Globalization;
using api.Controllers.Models;
using api.Data;
using api.Data.Entities;
using api.Data.Enums;
using api.Extensions;
using api.Models;
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
        var anyParticipant = _dataContext.Users.Any(u => u.Id == request.ParticipantId);

        if (!anyParticipant)
        {
            throw new Exception("Participant not found!");
        }

        var meeting = new Meeting()
        {
            Title = request.Title,
            Description = request.Description,
            ExternalId = request.ExternalId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            ServiceId = request.ServiceId,
            CreatedAt = DateTime.Now
        };
        
        _dataContext.Meetings.Add(meeting);

        var attendees = new List<MeetingAttendee>()
        {
            new()
            {
                UserId = request.ParticipantId,
                IsCreator = true,
                CreatedAt = DateTime.Now,
                Meeting = meeting
            }
        };
        
        if (!string.IsNullOrWhiteSpace(request.CreatorEmailAddress))
        {
            var user = _dataContext.Users.SingleOrDefault(x => x.EmailAddress == request.CreatorEmailAddress);

            if (user == null)
            {
                user = new User()
                {
                    EmailAddress = request.CreatorEmailAddress,
                    FirstName = request.CreatorFirstName,
                    LastName = request.CreatorLastName,
                    Status = "Created",
                    CreatedAt = DateTime.Now
                };

                _dataContext.Users.Add(user);
            }
            
            attendees.Add(
                new()
                { 
                    User = user, 
                    IsCreator = true, 
                    CreatedAt = DateTime.Now, 
                    Meeting = meeting
                });
        }
        
        _dataContext.MeetingAttendees.AddRange(attendees);
        _dataContext.SaveChanges();
    }

    public async Task<List<GetMeetingResponse>?> Get(long userId, DateTime timeMin, DateTime timeMax)
    {
        await SyncWithGoogleCalendar(userId); 

        var meetings = _dataContext.MeetingAttendees
            .Where(x => x.UserId == userId)
            .Select(x => x.Meeting)
            .Where(m => m.StartTime < timeMin && timeMin < m.EndTime
                        || timeMin < m.StartTime && m.EndTime < timeMax
                        || m.StartTime < timeMax && timeMax < m.EndTime)
            .Select( m => new GetMeetingResponse()
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                StartTime = m.StartTime,
                EndTime = m.EndTime,
                ServiceName = m.Service.Name,
                ServiceDuration = m.Service.Duration,
                ServicePrice = m.Service.Price
            }).ToList();

        return meetings;
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
                        IsCreator = true, 
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