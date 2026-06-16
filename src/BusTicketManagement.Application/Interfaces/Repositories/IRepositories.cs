using System.Collections.Generic;
using System.Threading.Tasks;
using BusTicketManagement.Domain.Entities;

namespace BusTicketManagement.Application.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUserNameAsync(string userName);
        Task<User> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName, int pageNumber, int pageSize);
    }

    public interface IBusRepository : IRepository<Bus>
    {
        Task<Bus> GetByBusNoAsync(string busNo);
        Task<IEnumerable<Bus>> GetActiveBusesAsync();
    }

    public interface IRouteRepository : IRepository<Route>
    {
        Task<Route> GetByRouteCodeAsync(string routeCode);
        Task<Route> GetBySourceDestinationAsync(string source, string destination);
    }

    public interface IScheduleRepository : IRepository<Schedule>
    {
        Task<IEnumerable<Schedule>> GetSchedulesByRouteAsync(int routeId, int pageNumber, int pageSize);
        Task<IEnumerable<Schedule>> SearchSchedulesAsync(string sourceCity, string destinationCity, System.DateTime travelDate);
    }

    public interface IBookingRepository : IRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetUserBookingsAsync(string userId, int pageNumber, int pageSize);
        Task<Booking> GetByBookingCodeAsync(string bookingCode);
    }

    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<Payment> GetByBookingIdAsync(int bookingId);
        Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status);
    }

    public interface ISeatRepository : IRepository<Seat>
    {
        Task<IEnumerable<Seat>> GetScheduleSeatsAsync(int scheduleId);
        Task<Seat> GetBySeatNumberAsync(int scheduleId, int seatNumber);
        Task<int> GetAvailableSeatsCountAsync(int scheduleId);
    }
}
