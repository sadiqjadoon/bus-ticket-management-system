using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BusTicketManagement.Application.Interfaces.Repositories;
using BusTicketManagement.Domain.Entities;
using BusTicketManagement.Infrastructure.Data;

namespace BusTicketManagement.Infrastructure.Repositories
{
    public class BusRepository : BaseRepository<Bus>, IBusRepository
    {
        public BusRepository(IDbConnectionFactory connectionFactory) 
            : base(connectionFactory, "Buses")
        {
        }

        public async Task<Bus> GetByBusNoAsync(string busNo)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Buses WHERE BusNo = @BusNo", conn))
                {
                    cmd.Parameters.AddWithValue("@BusNo", busNo);
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

        public async Task<IEnumerable<Bus>> GetActiveBusesAsync()
        {
            List<Bus> buses = new List<Bus>();
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Buses WHERE IsActive = 1", conn))
                {
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            buses.Add(MapFromReader(reader));
                        }
                    }
                }
            }
            return buses;
        }

        public override async Task<int> AddAsync(Bus entity)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Buses (BusNo, BusType, Capacity, RegistrationNo, ManufacturerName, ModelName, YearOfManufacture, Owner, IsActive, CreatedAt, UpdatedAt) " +
                    "VALUES (@BusNo, @BusType, @Capacity, @RegistrationNo, @ManufacturerName, @ModelName, @YearOfManufacture, @Owner, @IsActive, @CreatedAt, @UpdatedAt); " +
                    "SELECT SCOPE_IDENTITY();", conn))
                {
                    cmd.Parameters.AddWithValue("@BusNo", entity.BusNo);
                    cmd.Parameters.AddWithValue("@BusType", entity.BusType);
                    cmd.Parameters.AddWithValue("@Capacity", entity.Capacity);
                    cmd.Parameters.AddWithValue("@RegistrationNo", entity.RegistrationNo);
                    cmd.Parameters.AddWithValue("@ManufacturerName", entity.ManufacturerName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModelName", entity.ModelName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@YearOfManufacture", entity.YearOfManufacture ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Owner", entity.Owner ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                    await conn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public override async Task<bool> UpdateAsync(Bus entity)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Buses SET BusNo=@BusNo, BusType=@BusType, Capacity=@Capacity, " +
                    "RegistrationNo=@RegistrationNo, ManufacturerName=@ManufacturerName, ModelName=@ModelName, " +
                    "YearOfManufacture=@YearOfManufacture, Owner=@Owner, IsActive=@IsActive, UpdatedAt=@UpdatedAt " +
                    "WHERE BusId=@BusId", conn))
                {
                    cmd.Parameters.AddWithValue("@BusId", entity.BusId);
                    cmd.Parameters.AddWithValue("@BusNo", entity.BusNo);
                    cmd.Parameters.AddWithValue("@BusType", entity.BusType);
                    cmd.Parameters.AddWithValue("@Capacity", entity.Capacity);
                    cmd.Parameters.AddWithValue("@RegistrationNo", entity.RegistrationNo);
                    cmd.Parameters.AddWithValue("@ManufacturerName", entity.ManufacturerName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModelName", entity.ModelName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@YearOfManufacture", entity.YearOfManufacture ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Owner", entity.Owner ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", entity.IsActive);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                    await conn.OpenAsync();
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        protected override Bus MapFromReader(SqlDataReader reader)
        {
            return new Bus
            {
                BusId = (int)reader["BusId"],
                BusNo = reader["BusNo"].ToString(),
                BusType = reader["BusType"].ToString(),
                Capacity = (int)reader["Capacity"],
                RegistrationNo = reader["RegistrationNo"].ToString(),
                ManufacturerName = reader["ManufacturerName"] != DBNull.Value ? reader["ManufacturerName"].ToString() : null,
                ModelName = reader["ModelName"] != DBNull.Value ? reader["ModelName"].ToString() : null,
                YearOfManufacture = reader["YearOfManufacture"] != DBNull.Value ? (int)reader["YearOfManufacture"] : (int?)null,
                Owner = reader["Owner"] != DBNull.Value ? reader["Owner"].ToString() : null,
                IsActive = (bool)reader["IsActive"],
                CreatedAt = (DateTime)reader["CreatedAt"],
                UpdatedAt = (DateTime)reader["UpdatedAt"]
            };
        }
    }
}
