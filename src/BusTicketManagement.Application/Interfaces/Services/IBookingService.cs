using System.Collections.Generic;
using System.Threading.Tasks;
using BusTicketManagement.Application.DTOs.Booking;

namespace BusTicketManagement.Application.Interfaces.Services
{
    public interface IBookingService
    {
        Task<BookingDto> GetBookingByIdAsync(int bookingId);
        Task<IEnumerable<BookingDto>> GetUserBookingsAsync(string userId, int pageNumber, int pageSize);
        Task<BookingDto> CreateBookingAsync(CreateBookingDto dto, string userId);
        Task<bool> CancelBookingAsync(CancelBookingDto dto);
        Task<IEnumerable<BookingDto>> GetBookingsByScheduleAsync(int scheduleId);
    }
}
