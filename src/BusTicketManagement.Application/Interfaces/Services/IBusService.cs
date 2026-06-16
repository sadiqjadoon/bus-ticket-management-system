using System.Collections.Generic;
using System.Threading.Tasks;
using BusTicketManagement.Application.DTOs.Bus;

namespace BusTicketManagement.Application.Interfaces.Services
{
    public interface IBusService
    {
        Task<BusDto> GetBusByIdAsync(int busId);
        Task<IEnumerable<BusDto>> GetAllBusesAsync(int pageNumber, int pageSize);
        Task<int> CreateBusAsync(CreateBusDto dto);
        Task<bool> UpdateBusAsync(UpdateBusDto dto);
        Task<bool> DeleteBusAsync(int busId);
    }
}
