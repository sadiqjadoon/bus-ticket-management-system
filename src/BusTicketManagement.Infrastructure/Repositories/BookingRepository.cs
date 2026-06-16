using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BusTicketManagement.Application.Interfaces.Repositories;
using BusTicketManagement.Domain.Entities;
using BusTicketManagement.Infrastructure.Data;

namespace BusTicketManagement.Infrastructure.Repositories
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        public BookingRepository(IDbConnectionFactory connectionFactory)
            : base(connectionFactory, "Bookings")
        {
        }

        public async Task<IEnumerable<Booking>> GetUserBookingsAsync(string userId, int pageNumber, int pageSize)
        {
            List<Booking> bookings = new List<Booking>();
            int offset = (pageNumber - 1) * pageSize;

            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Bookings WHERE UserId = @UserId " +
                    "ORDER BY BookingDate DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Offset", offset);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            bookings.Add(MapFromReader(reader));
                        }
                    }
                }
            }
            return bookings;
        }

        public async Task<Booking> GetByBookingCodeAsync(string bookingCode)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Bookings WHERE BookingCode = @BookingCode", conn))
                {
                    cmd.Parameters.AddWithValue("@BookingCode", bookingCode);
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

        public override async Task<int> AddAsync(Booking entity)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Bookings (BookingCode, UserId, ScheduleId, SeatId, PassengerName, PassengerEmail, PassengerPhone, BookingStatus, BookingDate, CreatedAt, UpdatedAt) " +
                    "VALUES (@BookingCode, @UserId, @ScheduleId, @SeatId, @PassengerName, @PassengerEmail, @PassengerPhone, @BookingStatus, @BookingDate, @CreatedAt, @UpdatedAt); " +
                    "SELECT SCOPE_IDENTITY();", conn))
                {
                    cmd.Parameters.AddWithValue("@BookingCode", entity.BookingCode);
                    cmd.Parameters.AddWithValue("@UserId", entity.UserId);
                    cmd.Parameters.AddWithValue("@ScheduleId", entity.ScheduleId);
                    cmd.Parameters.AddWithValue("@SeatId", entity.SeatId);
                    cmd.Parameters.AddWithValue("@PassengerName", entity.PassengerName);
                    cmd.Parameters.AddWithValue("@PassengerEmail", entity.PassengerEmail ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PassengerPhone", entity.PassengerPhone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BookingStatus", "Pending");
                    cmd.Parameters.AddWithValue("@BookingDate", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public override async Task<bool> UpdateAsync(Booking entity)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Bookings SET BookingStatus=@BookingStatus, CancellationDate=@CancellationDate, CancellationReason=@CancellationReason, UpdatedAt=@UpdatedAt " +
                    "WHERE BookingId=@BookingId", conn))
                {
                    cmd.Parameters.AddWithValue("@BookingId", entity.BookingId);
                    cmd.Parameters.AddWithValue("@BookingStatus", entity.BookingStatus);
                    cmd.Parameters.AddWithValue("@CancellationDate", entity.CancellationDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CancellationReason", entity.CancellationReason ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                    await conn.OpenAsync();
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        protected override Booking MapFromReader(SqlDataReader reader)
        {
            return new Booking
            {
                BookingId = (int)reader["BookingId"],
                BookingCode = reader["BookingCode"].ToString(),
                UserId = reader["UserId"].ToString(),
                ScheduleId = (int)reader["ScheduleId"],
                SeatId = (int)reader["SeatId"],
                PassengerName = reader["PassengerName"].ToString(),
                PassengerEmail = reader["PassengerEmail"] != DBNull.Value ? reader["PassengerEmail"].ToString() : null,
                PassengerPhone = reader["PassengerPhone"] != DBNull.Value ? reader["PassengerPhone"].ToString() : null,
                BookingStatus = reader["BookingStatus"].ToString(),
                BookingDate = (DateTime)reader["BookingDate"],
                CancellationDate = reader["CancellationDate"] != DBNull.Value ? (DateTime)reader["CancellationDate"] : (DateTime?)null,
                CancellationReason = reader["CancellationReason"] != DBNull.Value ? reader["CancellationReason"].ToString() : null,
                CreatedAt = (DateTime)reader["CreatedAt"],
                UpdatedAt = (DateTime)reader["UpdatedAt"]
            };
        }
    }
}
