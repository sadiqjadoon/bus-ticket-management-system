using System.Collections.Generic;
using System.Threading.Tasks;
using BusTicketManagement.Application.DTOs.Route;

namespace BusTicketManagement.Application.Interfaces.Services
{
    public interface IRouteService
    {
        Task<RouteDto> GetRouteByIdAsync(int routeId);
        Task<IEnumerable<RouteDto>> GetAllRoutesAsync(int pageNumber, int pageSize);
        Task<int> CreateRouteAsync(CreateRouteDto dto);
        Task<bool> UpdateRouteAsync(UpdateRouteDto dto);
        Task<bool> DeleteRouteAsync(int routeId);
    }
}
