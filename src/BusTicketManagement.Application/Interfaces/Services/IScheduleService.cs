using System.Collections.Generic;
using System.Threading.Tasks;
using BusTicketManagement.Application.DTOs.Schedule;

namespace BusTicketManagement.Application.Interfaces.Services
{
    public interface IScheduleService
    {
        Task<ScheduleDto> GetScheduleByIdAsync(int scheduleId);
        Task<IEnumerable<ScheduleDto>> SearchSchedulesAsync(SearchScheduleDto dto);
        Task<int> CreateScheduleAsync(CreateScheduleDto dto);
        Task<bool> UpdateScheduleAsync(UpdateScheduleDto dto);
        Task<bool> DeleteScheduleAsync(int scheduleId);
    }
}
