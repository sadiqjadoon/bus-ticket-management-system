using System;
using System.Data.SqlClient;

namespace BusTicketManagement.Infrastructure.Data
{
    public interface IDbConnectionFactory
    {
        SqlConnection GetConnection();
    }

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
