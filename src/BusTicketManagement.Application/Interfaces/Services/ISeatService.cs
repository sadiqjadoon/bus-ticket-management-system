using System.Collections.Generic;
using System.Threading.Tasks;
using BusTicketManagement.Application.DTOs.Seat;

namespace BusTicketManagement.Application.Interfaces.Services
{
    public interface ISeatService
    {
        Task<ScheduleSeatsDto> GetScheduleSeatsAsync(int scheduleId);
        Task<SeatDto> GetSeatByIdAsync(int seatId);
    }
}
