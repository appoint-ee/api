using api.Controllers.Models;
using api.Data;
using api.Models;

namespace api.Services;

public class SlotService : ISlotService
{
    private readonly DataContext _context;

    public SlotService(DataContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public List<TimeSlot> GenerateTimeSlots(long userId, DateTime start, DateTime end, IList<GetMeetingResponse>? meetings)
    {
        var timeSlots = new List<TimeSlot>();
        var timeIncrement = TimeSpan.FromHours(1);

        var userAvailabilityHours = _context.AvailabilityHours.Where(x => x.UserId == userId).ToList();
        
        while (start.Date <= end.Date)        
        {
            var dailyAvailabilityHours = userAvailabilityHours.Where(x => x.DayOfWeek == (int)start.DayOfWeek).ToList();

            foreach (var dailyAvailabilityHour in dailyAvailabilityHours)
            {
                var currentIteration = new DateTime(start.Year, start.Month, start.Day, dailyAvailabilityHour.StartTime.Hour, 0, 0);
                var endIteration = new DateTime(start.Year, start.Month, start.Day, dailyAvailabilityHour.EndTime.Hour, 0, 0);
                
                for (; currentIteration < endIteration; currentIteration = currentIteration.Add(timeIncrement))
                {
                    var nextIteration = currentIteration.Add(timeIncrement);
                    
                    var isBooked = meetings != null && meetings.Any(m =>
                        m.StartTime < currentIteration && currentIteration < m.EndTime       
                        || currentIteration < m.StartTime &&  m.EndTime < nextIteration       
                        || currentIteration == m.StartTime &&  m.EndTime == nextIteration       
                        || m.StartTime < nextIteration && nextIteration < m.EndTime);        

                    var status = isBooked ? "booked" : "available";
                    
                    timeSlots.Add(
                        new TimeSlot()
                        {
                            StartTime = currentIteration,
                            EndTime = nextIteration,
                            Status = status
                        });
                }
            }

            start = start.AddDays(1);
        }

        return timeSlots;
    }

    public List<DaySlot> GenerateDaySlots(long userId, DateTime start, DateTime end, IList<GetMeetingResponse>? meetings)
    { 
        var daySlots = new List<DaySlot>();
        
        var timeSlots = GenerateTimeSlots(userId, start, end, meetings);

        var currentDate = start.Date;

        while (currentDate < end.Date)
        {
            var currentDayTimeSlots =
                timeSlots.Where(x => x.StartTime.Date == currentDate || x.EndTime.Date == currentDate).ToArray();
            
            var anyAvailable = currentDayTimeSlots.Any(x => x.Status == "available");
            var anyBooked = currentDayTimeSlots.Any(x => x.Status == "booked");

            var status = anyAvailable ? "available" : (anyBooked ? "booked" : "Not Available");

            daySlots.Add(new DaySlot()
            {
                DateTime = currentDate,
                Status = status
            });
            
            currentDate = currentDate.AddDays(1);
        }

        return daySlots;
    }
}