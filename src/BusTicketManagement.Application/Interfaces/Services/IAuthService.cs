using System.Collections.Generic;
using System.Threading.Tasks;
using BusTicketManagement.Application.DTOs.Auth;

namespace BusTicketManagement.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<List<string>> GetUserRolesAsync(string userId);
        Task<List<string>> GetUserPermissionsAsync(string userId);
    }
}
