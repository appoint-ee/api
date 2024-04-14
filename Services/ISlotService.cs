using api.Models;

namespace api.Services;

public interface ISlotService
{
    List<TimeSlot> GenerateTimeSlots(DateTime startDateTime, DateTime endDateTime, IList<Event>? events);
    List<DaySlot> GenerateDaySlots(DateTime startDateTime, DateTime endDateTime, IList<Event>? events);
}