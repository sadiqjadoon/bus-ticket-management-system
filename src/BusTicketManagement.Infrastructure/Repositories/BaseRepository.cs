using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BusTicketManagement.Application.Interfaces;

namespace BusTicketManagement.Infrastructure.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly IDbConnectionFactory _connectionFactory;
        protected readonly string _tableName;

        protected BaseRepository(IDbConnectionFactory connectionFactory, string tableName)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand($"SELECT * FROM {_tableName} WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
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

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            List<T> items = new List<T>();
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand($"SELECT * FROM {_tableName}", conn))
                {
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            items.Add(MapFromReader(reader));
                        }
                    }
                }
            }
            return items;
        }

        public virtual async Task<int> AddAsync(T entity)
        {
            throw new NotImplementedException("Use specific repository implementation");
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            throw new NotImplementedException("Use specific repository implementation");
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            using (SqlConnection conn = _connectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand($"DELETE FROM {_tableName} WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    await conn.OpenAsync();
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        protected abstract T MapFromReader(SqlDataReader reader);
    }
}
