using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BusTicketManagement.Application.Interfaces.Repositories;
using BusTicketManagement.Domain.Entities;
using BusTicketManagement.Infrastructure.Data;

namespace BusTicketManagement.Infrastructure.Repositories
{
    public class RouteRepository : BaseRepository<Route>, IRouteRepository
    {
        public RouteRepository(IDbConnectionFactory connectionFactory) 
            : base(connectionFactory, "Routes")
        {
        }

        public async Task<Route> GetByRouteCodeAsync(string routeCode)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Routes WHERE RouteCode = @RouteCode", conn))
                {
                    cmd.Parameters.AddWithValue("@RouteCode", routeCode);
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

        public async Task<Route> GetBySourceDestinationAsync(string source, string destination)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Routes WHERE SourceCity = @Source AND DestinationCity = @Destination AND IsActive = 1", conn))
                {
                    cmd.Parameters.AddWithValue("@Source", source);
                    cmd.Parameters.AddWithValue("@Destination", destination);
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

        public override async Task<int> AddAsync(Route entity)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Routes (SourceCity, DestinationCity, Distance, Duration, BaseFare, RouteCode, IsActive, CreatedAt, UpdatedAt) " +
                    "VALUES (@SourceCity, @DestinationCity, @Distance, @Duration, @BaseFare, @RouteCode, @IsActive, @CreatedAt, @UpdatedAt); " +
                    "SELECT SCOPE_IDENTITY();", conn))
                {
                    cmd.Parameters.AddWithValue("@SourceCity", entity.SourceCity);
                    cmd.Parameters.AddWithValue("@DestinationCity", entity.DestinationCity);
                    cmd.Parameters.AddWithValue("@Distance", entity.Distance);
                    cmd.Parameters.AddWithValue("@Duration", entity.Duration);
                    cmd.Parameters.AddWithValue("@BaseFare", entity.BaseFare);
                    cmd.Parameters.AddWithValue("@RouteCode", entity.RouteCode);
                    cmd.Parameters.AddWithValue("@IsActive", true);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public override async Task<bool> UpdateAsync(Route entity)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Routes SET SourceCity=@SourceCity, DestinationCity=@DestinationCity, " +
                    "Distance=@Distance, Duration=@Duration, BaseFare=@BaseFare, IsActive=@IsActive, UpdatedAt=@UpdatedAt " +
                    "WHERE RouteId=@RouteId", conn))
                {
                    cmd.Parameters.AddWithValue("@RouteId", entity.RouteId);
                    cmd.Parameters.AddWithValue("@SourceCity", entity.SourceCity);
                    cmd.Parameters.AddWithValue("@DestinationCity", entity.DestinationCity);
                    cmd.Parameters.AddWithValue("@Distance", entity.Distance);
                    cmd.Parameters.AddWithValue("@Duration", entity.Duration);
                    cmd.Parameters.AddWithValue("@BaseFare", entity.BaseFare);
                    cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                    await conn.OpenAsync();
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        protected override Route MapFromReader(SqlDataReader reader)
        {
            return new Route
            {
                RouteId = (int)reader["RouteId"],
                SourceCity = reader["SourceCity"].ToString(),
                DestinationCity = reader["DestinationCity"].ToString(),
                Distance = (decimal)reader["Distance"],
                Duration = (int)reader["Duration"],
                BaseFare = (decimal)reader["BaseFare"],
                RouteCode = reader["RouteCode"].ToString(),
                IsActive = (bool)reader["IsActive"],
                CreatedAt = (DateTime)reader["CreatedAt"],
                UpdatedAt = (DateTime)reader["UpdatedAt"]
            };
        }
    }
}
