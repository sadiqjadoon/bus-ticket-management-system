using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BusTicketManagement.Application.Interfaces.Repositories;
using BusTicketManagement.Domain.Entities;
using BusTicketManagement.Infrastructure.Data;

namespace BusTicketManagement.Infrastructure.Repositories
{
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(IDbConnectionFactory connectionFactory)
            : base(connectionFactory, "Payments")
        {
        }

        public async Task<Payment> GetByBookingIdAsync(int bookingId)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Payments WHERE BookingId = @BookingId", conn))
                {
                    cmd.Parameters.AddWithValue("@BookingId", bookingId);
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

        public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status)
        {
            List<Payment> payments = new List<Payment>();
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Payments WHERE PaymentStatus = @Status", conn))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            payments.Add(MapFromReader(reader));
                        }
                    }
                }
            }
            return payments;
        }

        public override async Task<int> AddAsync(Payment entity)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Payments (BookingId, Amount, PaymentStatus, PaymentMethod, TransactionId, CreatedAt, UpdatedAt) " +
                    "VALUES (@BookingId, @Amount, @PaymentStatus, @PaymentMethod, @TransactionId, @CreatedAt, @UpdatedAt); " +
                    "SELECT SCOPE_IDENTITY();", conn))
                {
                    cmd.Parameters.AddWithValue("@BookingId", entity.BookingId);
                    cmd.Parameters.AddWithValue("@Amount", entity.Amount);
                    cmd.Parameters.AddWithValue("@PaymentStatus", entity.PaymentStatus);
                    cmd.Parameters.AddWithValue("@PaymentMethod", entity.PaymentMethod ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TransactionId", entity.TransactionId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public override async Task<bool> UpdateAsync(Payment entity)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Payments SET PaymentStatus=@PaymentStatus, PaymentMethod=@PaymentMethod, " +
                    "PaymentDate=@PaymentDate, RefundDate=@RefundDate, RefundReason=@RefundReason, UpdatedAt=@UpdatedAt " +
                    "WHERE PaymentId=@PaymentId", conn))
                {
                    cmd.Parameters.AddWithValue("@PaymentId", entity.PaymentId);
                    cmd.Parameters.AddWithValue("@PaymentStatus", entity.PaymentStatus);
                    cmd.Parameters.AddWithValue("@PaymentMethod", entity.PaymentMethod ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PaymentDate", entity.PaymentDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RefundDate", entity.RefundDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RefundReason", entity.RefundReason ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                    await conn.OpenAsync();
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        protected override Payment MapFromReader(SqlDataReader reader)
        {
            return new Payment
            {
                PaymentId = (int)reader["PaymentId"],
                BookingId = (int)reader["BookingId"],
                Amount = (decimal)reader["Amount"],
                PaymentStatus = reader["PaymentStatus"].ToString(),
                PaymentMethod = reader["PaymentMethod"] != DBNull.Value ? reader["PaymentMethod"].ToString() : null,
                TransactionId = reader["TransactionId"] != DBNull.Value ? reader["TransactionId"].ToString() : null,
                PaymentDate = reader["PaymentDate"] != DBNull.Value ? (DateTime)reader["PaymentDate"] : (DateTime?)null,
                RefundDate = reader["RefundDate"] != DBNull.Value ? (DateTime)reader["RefundDate"] : (DateTime?)null,
                RefundReason = reader["RefundReason"] != DBNull.Value ? reader["RefundReason"].ToString() : null,
                CreatedAt = (DateTime)reader["CreatedAt"],
                UpdatedAt = (DateTime)reader["UpdatedAt"]
            };
        }
    }
}
