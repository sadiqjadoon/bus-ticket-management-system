using System.Collections.Generic;
using System.Threading.Tasks;
using BusTicketManagement.Application.DTOs.Auth;
using BusTicketManagement.Application.DTOs.Admin;

namespace BusTicketManagement.Application.Interfaces.Services
{
    public interface IAdminService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync(int pageNumber, int pageSize, string searchTerm);
        Task<bool> AssignRoleToUserAsync(AssignRoleDto dto);
        Task<bool> AssignPermissionToRoleAsync(AssignPermissionDto dto);
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync();
    }
}
