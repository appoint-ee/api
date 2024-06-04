using api.Controllers.Models;
using api.Models;

namespace api.Services;

public class SlotService : ISlotService
{
    public List<TimeSlot> GenerateTimeSlots(DateTime startDateTime, DateTime endDateTime, IList<GetMeetingResponse>? meetings)
    { 
         var timeSlots = new List<TimeSlot>();
         var startWorkingTime = new TimeSpan(9, 0, 0);
         var endWorkingTime = new TimeSpan(17, 0, 0);
         var currentDateTime = startDateTime;
         
         while (currentDateTime < endDateTime)
         {
             var nextDateTime = currentDateTime.AddHours(1);

             var isWorkingDay = currentDateTime.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday;

             var isWorkingHours = currentDateTime.TimeOfDay >= startWorkingTime 
                                  && currentDateTime.TimeOfDay < endWorkingTime;

             if (!isWorkingDay || !isWorkingHours)
             {
                 currentDateTime = nextDateTime;
                 continue;
             }

             var isBooked = meetings != null && meetings.Any(m =>
                 m.StartTime < currentDateTime && currentDateTime < m.EndTime       
                 || currentDateTime < m.StartTime &&  m.EndTime < nextDateTime       
                 || currentDateTime == m.StartTime &&  m.EndTime == nextDateTime       
                 || m.StartTime < nextDateTime && nextDateTime < m.EndTime);        

             var status = isBooked ? "booked" : "available";
                 
             timeSlots.Add(new TimeSlot
             {
                 StartTime = currentDateTime,
                 EndTime = nextDateTime,
                 Status = status
             });

             currentDateTime = nextDateTime;
         }
         
         return timeSlots;
    }

    public List<DaySlot> GenerateDaySlots(DateTime startDateTime, DateTime endDateTime, IList<GetMeetingResponse>? meetings)
    { 
        var daySlots = new List<DaySlot>();
        
        var timeSlots = GenerateTimeSlots(startDateTime, endDateTime, meetings);

        var currentDate = startDateTime.Date;

        while (currentDate < endDateTime.Date)
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