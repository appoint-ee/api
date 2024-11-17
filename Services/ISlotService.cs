using api.Controllers.Models;
using api.Models;

namespace api.Services;

public interface ISlotService
{
    List<TimeSlot>? GenerateTimeSlots(long userId, DateTime startDateTime, DateTime endDateTime, GetMeetingResponse[]? events);
    List<DaySlot>? GenerateDaySlots(long userId, DateTime startDateTime, DateTime endDateTime, GetMeetingResponse[]? events);
}