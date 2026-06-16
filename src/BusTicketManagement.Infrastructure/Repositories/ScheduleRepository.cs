using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BusTicketManagement.Application.Interfaces.Repositories;
using BusTicketManagement.Domain.Entities;
using BusTicketManagement.Infrastructure.Data;

namespace BusTicketManagement.Infrastructure.Repositories
{
    public class ScheduleRepository : BaseRepository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(IDbConnectionFactory connectionFactory)
            : base(connectionFactory, "Schedules")
        {
        }

        public async Task<IEnumerable<Schedule>> GetSchedulesByRouteAsync(int routeId, int pageNumber, int pageSize)
        {
            List<Schedule> schedules = new List<Schedule>();
            int offset = (pageNumber - 1) * pageSize;

            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Schedules WHERE RouteId = @RouteId AND ScheduleStatus = 'Scheduled' " +
                    "ORDER BY DepartureTime DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", conn))
                {
                    cmd.Parameters.AddWithValue("@RouteId", routeId);
                    cmd.Parameters.AddWithValue("@Offset", offset);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            schedules.Add(MapFromReader(reader));
                        }
                    }
                }
            }
            return schedules;
        }

        public async Task<IEnumerable<Schedule>> SearchSchedulesAsync(string sourceCity, string destinationCity, DateTime travelDate)
        {
            List<Schedule> schedules = new List<Schedule>();
            var travelDateOnly = travelDate.Date;

            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "EXEC sp_GetSchedulesForRoute @SourceCity, @DestinationCity, @TravelDate", conn))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.AddWithValue("@SourceCity", sourceCity);
                    cmd.Parameters.AddWithValue("@DestinationCity", destinationCity);
                    cmd.Parameters.AddWithValue("@TravelDate", travelDateOnly);

                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            schedules.Add(MapFromReader(reader));
                        }
                    }
                }
            }
            return schedules;
        }

        public override async Task<int> AddAsync(Schedule entity)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Schedules (BusId, RouteId, DepartureTime, ArrivalTime, AvailableSeats, TotalSeats, Fare, ScheduleStatus, CreatedAt, UpdatedAt) " +
                    "VALUES (@BusId, @RouteId, @DepartureTime, @ArrivalTime, @AvailableSeats, @TotalSeats, @Fare, @ScheduleStatus, @CreatedAt, @UpdatedAt); " +
                    "SELECT SCOPE_IDENTITY();", conn))
                {
                    cmd.Parameters.AddWithValue("@BusId", entity.BusId);
                    cmd.Parameters.AddWithValue("@RouteId", entity.RouteId);
                    cmd.Parameters.AddWithValue("@DepartureTime", entity.DepartureTime);
                    cmd.Parameters.AddWithValue("@ArrivalTime", entity.ArrivalTime);
                    cmd.Parameters.AddWithValue("@AvailableSeats", entity.AvailableSeats);
                    cmd.Parameters.AddWithValue("@TotalSeats", entity.TotalSeats);
                    cmd.Parameters.AddWithValue("@Fare", entity.Fare);
                    cmd.Parameters.AddWithValue("@ScheduleStatus", "Scheduled");
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public override async Task<bool> UpdateAsync(Schedule entity)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Schedules SET DepartureTime=@DepartureTime, ArrivalTime=@ArrivalTime, " +
                    "AvailableSeats=@AvailableSeats, Fare=@Fare, ScheduleStatus=@ScheduleStatus, UpdatedAt=@UpdatedAt " +
                    "WHERE ScheduleId=@ScheduleId", conn))
                {
                    cmd.Parameters.AddWithValue("@ScheduleId", entity.ScheduleId);
                    cmd.Parameters.AddWithValue("@DepartureTime", entity.DepartureTime);
                    cmd.Parameters.AddWithValue("@ArrivalTime", entity.ArrivalTime);
                    cmd.Parameters.AddWithValue("@AvailableSeats", entity.AvailableSeats);
                    cmd.Parameters.AddWithValue("@Fare", entity.Fare);
                    cmd.Parameters.AddWithValue("@ScheduleStatus", entity.ScheduleStatus);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                    await conn.OpenAsync();
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        protected override Schedule MapFromReader(SqlDataReader reader)
        {
            return new Schedule
            {
                ScheduleId = (int)reader["ScheduleId"],
                BusId = (int)reader["BusId"],
                RouteId = (int)reader["RouteId"],
                DepartureTime = (DateTime)reader["DepartureTime"],
                ArrivalTime = (DateTime)reader["ArrivalTime"],
                AvailableSeats = (int)reader["AvailableSeats"],
                TotalSeats = (int)reader["TotalSeats"],
                Fare = (decimal)reader["Fare"],
                ScheduleStatus = reader["ScheduleStatus"].ToString(),
                CreatedAt = (DateTime)reader["CreatedAt"],
                UpdatedAt = (DateTime)reader["UpdatedAt"]
            };
        }
    }
}
