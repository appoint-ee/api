using api.Controllers.Models;
using api.Data;
using api.Data.Entities;

namespace api.Services;

public class MeetingService : IMeetingService
{
    private readonly DataContext _dataContext;

    public MeetingService(
        DataContext dataContext)
    {
        _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
    }

    public void Create(CreateMeetingRequest request)
    {
        var anyParticipant = _dataContext.Users.Any(u => u.Id == request.ParticipantId);

        if (!anyParticipant)
        {
            throw new Exception("Participant not found!");
        }

        var user = _dataContext.Users.SingleOrDefault(x => x.EmailAddress == request.CreatorEmailAddress);

        if (user == null)
        {
            user = new User()
            {
                EmailAddress = request.CreatorEmailAddress,
                FirstName = request.CreatorFirstName,
                LastName = request.CreatorLastName,
                UserName = $"{request.CreatorFirstName} {request.CreatorLastName}", // TODO: uniqueness check
                Status = "Created",
                CreatedAt = DateTime.UtcNow
            };

            _dataContext.Users.Add(user);
        }

        var meeting = new Meeting()
        {
            Title = request.Title,
            Description = request.Description,
            Status = "Created",
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            CreatedAt = DateTime.UtcNow
        };
        
        _dataContext.Meetings.Add(meeting);

        var attendees = new List<MeetingAttendee>()
        {
            new()
            {
                User = user,
                IsCreator = true,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                Meeting = meeting
            },
            new()
            {
                UserId = request.ParticipantId,
                IsCreator = true,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                Meeting = meeting
            }
        };
        
        _dataContext.MeetingAttendees.AddRange(attendees);
        _dataContext.SaveChanges();
    }

    public List<GetMeetingResponse>? Get(string userName, DateTime start, DateTime end)
    {
        var user = _dataContext.Users.SingleOrDefault(x => x.UserName == userName);

        if (user == null)
        {
            return null;
        }

        var meetings = _dataContext.MeetingAttendees
            .Where(x => x.UserId == user.Id)
            .Select(x => x.Meeting)
            .Where(m => m.StartTime < start && start < m.EndTime
                        || start < m.StartTime && m.EndTime < end
                        || m.StartTime < end && end < m.EndTime)
            .Select( m => new GetMeetingResponse()
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                Status = m.Status,
                StartTime = m.StartTime,
                EndTime = m.EndTime
            }).ToList();

        return meetings;
    }
}