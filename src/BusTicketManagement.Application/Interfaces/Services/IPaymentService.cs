using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusTicketManagement.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(int bookingId, string paymentMethod);
        Task<bool> RefundPaymentAsync(int paymentId);
    }
}
