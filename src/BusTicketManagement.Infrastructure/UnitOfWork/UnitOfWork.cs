using System.Threading.Tasks;
using BusTicketManagement.Application.Interfaces;
using BusTicketManagement.Application.Interfaces.Repositories;
using BusTicketManagement.Infrastructure.Data;
using BusTicketManagement.Infrastructure.Repositories;

namespace BusTicketManagement.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public IUserRepository Users { get; private set; }
        public IBusRepository Buses { get; private set; }
        public IRouteRepository Routes { get; private set; }
        public IScheduleRepository Schedules { get; private set; }
        public IBookingRepository Bookings { get; private set; }
        public IPaymentRepository Payments { get; private set; }
        public ISeatRepository Seats { get; private set; }

        public UnitOfWork(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            InitializeRepositories();
        }

        private void InitializeRepositories()
        {
            Users = new UserRepository(_connectionFactory);
            Buses = new BusRepository(_connectionFactory);
            Routes = new RouteRepository(_connectionFactory);
            Schedules = new ScheduleRepository(_connectionFactory);
            Bookings = new BookingRepository(_connectionFactory);
            Payments = new PaymentRepository(_connectionFactory);
            Seats = new SeatRepository(_connectionFactory);
        }

        public async Task<bool> SaveChangesAsync()
        {
            // ADO.NET executes queries immediately, no SaveChanges needed
            return await Task.FromResult(true);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
