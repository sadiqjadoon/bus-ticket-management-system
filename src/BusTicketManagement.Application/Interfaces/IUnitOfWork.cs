using System.Threading.Tasks;
using BusTicketManagement.Application.Interfaces.Repositories;

namespace BusTicketManagement.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IBusRepository Buses { get; }
        IRouteRepository Routes { get; }
        IScheduleRepository Schedules { get; }
        IBookingRepository Bookings { get; }
        IPaymentRepository Payments { get; }
        ISeatRepository Seats { get; }

        Task<bool> SaveChangesAsync();
        void Dispose();
    }
}
