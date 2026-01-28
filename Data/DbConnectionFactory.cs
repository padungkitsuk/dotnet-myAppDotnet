
using System.Data;
using Microsoft.Data.SqlClient;

namespace MyBackend.Data
{
    public class DbConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")!;
        }

        // สร้าง Connection ใหม่ส่งออกไป
        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}