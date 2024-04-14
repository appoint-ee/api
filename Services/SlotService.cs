using api.Models;

namespace api.Services;

public class SlotService : ISlotService
{
    public List<TimeSlot> GenerateTimeSlots(DateTime startDateTime, DateTime endDateTime, IList<Event>? events)
    { 
         var timeSlots = new List<TimeSlot>();
         var startWorkingTime = new TimeSpan(9, 0, 0);
         var endWorkingTime = new TimeSpan(17, 0, 0);
         var currentDateTime = startDateTime;
         
         while (currentDateTime < endDateTime)
         {
             var isWorkingHours = currentDateTime.TimeOfDay >= startWorkingTime 
                                  && currentDateTime.TimeOfDay < endWorkingTime;

             if (isWorkingHours)
             {
                 var nextDateTime = currentDateTime.AddHours(1);

                 var isBooked = events != null && events.Any(e =>
                     e.Start.DateTime < currentDateTime && currentDateTime < e.End.DateTime       
                     || currentDateTime < e.Start.DateTime &&  e.End.DateTime < nextDateTime       
                     || e.Start.DateTime < nextDateTime && nextDateTime < e.End.DateTime);        

                 var status = isBooked ? "booked" : "available";
                 
                 timeSlots.Add(new TimeSlot
                 {
                     StartTime = currentDateTime,
                     EndTime = nextDateTime,
                     Status = status
                 });

                 currentDateTime = nextDateTime;
             }
             else
             {
                 timeSlots.Add(new TimeSlot
                 {
                     StartTime = currentDateTime,
                     EndTime = currentDateTime.AddHours(1),
                     Status = "not available"
                 });

                 currentDateTime = currentDateTime.AddHours(1);
             }
         }
         
         return timeSlots;
    }

    public List<DaySlot> GenerateDaySlots(DateTime startDateTime, DateTime endDateTime, IList<Event>? events)
    { 
        var daySlots = new List<DaySlot>();
        
        var timeSlots = GenerateTimeSlots(startDateTime, endDateTime, events);

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