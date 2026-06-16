namespace BusTicketManagement.Application.DTOs.Admin
{
    public class AssignRoleDto
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }

    public class AssignPermissionDto
    {
        public string RoleId { get; set; }
        public int PermissionId { get; set; }
    }

    public class RoleDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class PermissionDto
    {
        public int PermissionId { get; set; }
        public string PermissionCode { get; set; }
        public string PermissionName { get; set; }
        public string Description { get; set; }
    }
}
