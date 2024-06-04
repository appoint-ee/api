using api.Controllers.Models;
using api.Models;

namespace api.Services;

public interface ISlotService
{
    List<TimeSlot> GenerateTimeSlots(DateTime startDateTime, DateTime endDateTime, IList<GetMeetingResponse>? events);
    List<DaySlot> GenerateDaySlots(DateTime startDateTime, DateTime endDateTime, IList<GetMeetingResponse>? events);
}