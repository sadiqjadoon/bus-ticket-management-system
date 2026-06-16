using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BusTicketManagement.Application.Interfaces.Repositories;
using BusTicketManagement.Domain.Entities;
using BusTicketManagement.Infrastructure.Data;

namespace BusTicketManagement.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IDbConnectionFactory connectionFactory) 
            : base(connectionFactory, "AspNetUsers")
        {
        }

        public async Task<User> GetByUserNameAsync(string userName)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM AspNetUsers WHERE UserName = @UserName AND IsActive = 1", conn))
                {
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM AspNetUsers WHERE Email = @Email AND IsActive = 1", conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapFromReader(reader);
                        }
                    }
                }
            }
            return null;
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName, int pageNumber, int pageSize)
        {
            List<User> users = new List<User>();
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetUsersByRole", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RoleName", roleName);
                    cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);
                    
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(MapFromReader(reader));
                        }
                    }
                }
            }
            return users;
        }

        protected override User MapFromReader(SqlDataReader reader)
        {
            return new User
            {
                Id = reader["Id"].ToString(),
                UserName = reader["UserName"].ToString(),
                Email = reader["Email"].ToString(),
                FirstName = reader["FirstName"].ToString(),
                LastName = reader["LastName"].ToString(),
                IsActive = (bool)reader["IsActive"],
                CreatedAt = (DateTime)reader["CreatedAt"],
                UpdatedAt = (DateTime)reader["UpdatedAt"],
                LastLoginAt = reader["LastLoginAt"] != DBNull.Value ? (DateTime)reader["LastLoginAt"] : (DateTime?)null
            };
        }
    }
}
