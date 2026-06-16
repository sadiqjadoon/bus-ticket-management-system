using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BusTicketManagement.Application.Interfaces.Repositories;
using BusTicketManagement.Domain.Entities;
using BusTicketManagement.Infrastructure.Data;

namespace BusTicketManagement.Infrastructure.Repositories
{
    public class SeatRepository : BaseRepository<Seat>, ISeatRepository
    {
        public SeatRepository(IDbConnectionFactory connectionFactory)
            : base(connectionFactory, "Seats")
        {
        }

        public async Task<IEnumerable<Seat>> GetScheduleSeatsAsync(int scheduleId)
        {
            List<Seat> seats = new List<Seat>();
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Seats WHERE ScheduleId = @ScheduleId ORDER BY SeatNumber", conn))
                {
                    cmd.Parameters.AddWithValue("@ScheduleId", scheduleId);
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            seats.Add(MapFromReader(reader));
                        }
                    }
                }
            }
            return seats;
        }

        public async Task<Seat> GetBySeatNumberAsync(int scheduleId, int seatNumber)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Seats WHERE ScheduleId = @ScheduleId AND SeatNumber = @SeatNumber", conn))
                {
                    cmd.Parameters.AddWithValue("@ScheduleId", scheduleId);
                    cmd.Parameters.AddWithValue("@SeatNumber", seatNumber);
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

        public async Task<int> GetAvailableSeatsCountAsync(int scheduleId)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Seats WHERE ScheduleId = @ScheduleId AND SeatStatus = 'Available'", conn))
                {
                    cmd.Parameters.AddWithValue("@ScheduleId", scheduleId);
                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public override async Task<int> AddAsync(Seat entity)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Seats (ScheduleId, SeatNumber, SeatStatus, CreatedAt, UpdatedAt) " +
                    "VALUES (@ScheduleId, @SeatNumber, @SeatStatus, @CreatedAt, @UpdatedAt); " +
                    "SELECT SCOPE_IDENTITY();", conn))
                {
                    cmd.Parameters.AddWithValue("@ScheduleId", entity.ScheduleId);
                    cmd.Parameters.AddWithValue("@SeatNumber", entity.SeatNumber);
                    cmd.Parameters.AddWithValue("@SeatStatus", entity.SeatStatus);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public override async Task<bool> UpdateAsync(Seat entity)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Seats SET SeatStatus=@SeatStatus, UpdatedAt=@UpdatedAt WHERE SeatId=@SeatId", conn))
                {
                    cmd.Parameters.AddWithValue("@SeatId", entity.SeatId);
                    cmd.Parameters.AddWithValue("@SeatStatus", entity.SeatStatus);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                    await conn.OpenAsync();
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        protected override Seat MapFromReader(SqlDataReader reader)
        {
            return new Seat
            {
                SeatId = (int)reader["SeatId"],
                ScheduleId = (int)reader["ScheduleId"],
                SeatNumber = (int)reader["SeatNumber"],
                SeatStatus = reader["SeatStatus"].ToString(),
                CreatedAt = (DateTime)reader["CreatedAt"],
                UpdatedAt = (DateTime)reader["UpdatedAt"]
            };
        }
    }
}
