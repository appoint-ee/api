using api.Controllers.Models;
using api.Data;
using api.Data.Entities;
using api.Models;

namespace api.Services;

public class SlotService : ISlotService
{
    private readonly DataContext _context;

    public SlotService(DataContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public List<TimeSlot>? GenerateTimeSlots(long userId, DateTime start, DateTime end, GetMeetingResponse[]? meetings)
    {
        var timeSlots = new List<TimeSlot>();
        var timeIncrement = TimeSpan.FromHours(1);

        var weeklyHours = _context.WeeklyHours.Where(x => x.UserId == userId).ToArray();
        var dateSpecificHours = _context.DateSpecificHours.Where(x => x.UserId == userId).ToArray();

        if (!weeklyHours.Any())
        {
            return null;
        }
        
        while (start.Date <= end.Date)        
        {
            var weeklyHoursForIterationDay = weeklyHours.Where(x => x.DayOfWeek == (int)start.DayOfWeek).ToList();

            foreach (var weeklyHourItem in weeklyHoursForIterationDay)
            {
                var currentIteration = new DateTime(start.Year, start.Month, start.Day, weeklyHourItem.StartTime.Hour, 0, 0);
                var endIteration = new DateTime(start.Year, start.Month, start.Day, weeklyHourItem.EndTime.Hour, 0, 0);
                var dateSpecificHoursForIterationDay = dateSpecificHours.Where(x => x.SpecificDate == DateOnly.FromDateTime(start.Date)).ToArray();

                for (; currentIteration < endIteration; currentIteration = currentIteration.Add(timeIncrement))
                {
                    var nextIteration = currentIteration.Add(timeIncrement);
                    
                    var isDateSpecificHour = HasAnyDateSpecificHourWithinTimeRange(
                        dateSpecificHoursForIterationDay,
                        TimeOnly.FromDateTime(currentIteration),
                        TimeOnly.FromDateTime(currentIteration));

                    if (isDateSpecificHour)
                    {
                        continue;
                    }
                    
                    var isBooked = HasAnyMeetingWithinTimeRange(meetings, 
                        currentIteration, 
                        nextIteration);        
                    
                    timeSlots.Add(
                        new TimeSlot()
                        {
                            StartTime = currentIteration,
                            EndTime = nextIteration,
                            Status = isBooked 
                                ? "booked" 
                                : "available"
                        });
                }
            }

            start = start.AddDays(1);
        }

        return timeSlots;
    }

    private static bool HasAnyDateSpecificHourWithinTimeRange(DateSpecificHour[]? dateSpecificHoursForTheDay, TimeOnly rangeStart, TimeOnly rangeEnd)
    {
        return dateSpecificHoursForTheDay != null && dateSpecificHoursForTheDay.Any(x
            => rangeStart <= x.StartTime && x.StartTime <= rangeEnd
               || rangeStart <= x.EndTime && x.EndTime <= rangeEnd
               || x.StartTime <= rangeStart && rangeEnd <= x.EndTime);
    }

    private static bool HasAnyMeetingWithinTimeRange(IList<GetMeetingResponse>? meetings, DateTime rangeStart, DateTime rangeEnd)
    {
        rangeEnd = rangeEnd.AddTicks(-1);

        return meetings != null && meetings.Any(m 
            => rangeStart <= m.StartTime && m.StartTime <= rangeEnd        
               || rangeStart <= m.EndTime && m.EndTime <= rangeEnd  
               || m.StartTime <= rangeStart && rangeEnd <= m.EndTime);
    }

    public List<DaySlot>? GenerateDaySlots(long userId, DateTime start, DateTime end, GetMeetingResponse[]? meetings)
    { 
        var daySlots = new List<DaySlot>();
        
        var timeSlots = GenerateTimeSlots(userId, start, end, meetings);

        if (timeSlots == null)
        {
            return null;
        }
        
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