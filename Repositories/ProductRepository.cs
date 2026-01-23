using Dapper;
// using Npgsql;
using MyBackend.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace MyBackend.Repositories;

public class ProductRepository : IProductRepository {
    private readonly string _connectionString;

    public ProductRepository(IConfiguration configuration) {
        // ดึง Connection String จาก appsettings.json
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    // สร้าง Connection สำหรับ PostgreSQL
    //private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
    // 1. เปลี่ยนตัวเชื่อมต่อเป็น SqlConnection สำหรับ SQL Server
    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<Product?> GetByIdAsync(int id){
        const string sql = "SELECT * FROM products WHERE id = @Id";
        using var db = CreateConnection();
        // @Id เป็นการใช้ Parameter เพื่อป้องกัน SQL Injection
        return await db.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Product>> GetAllAsync(){
        const string sql = "SELECT id, name, price FROM Products ORDER BY id";
        using var db = CreateConnection();
        return await db.QueryAsync<Product>(sql);
    }

    public async Task<int> CreateAsync(Product product){
        const string sql = @"INSERT INTO products (name, price) 
                             VALUES (@name, @price);
                             SELECT CAST(SCOPE_IDENTITY() as int);";
        using var db = CreateConnection();
        return await db.ExecuteScalarAsync<int>(sql, product);
    }

    public async Task<bool> UpdateAsync(Product product){
        const string sql = @"UPDATE products 
                            SET name = @name, 
                                price = @price 
                            WHERE id = @id";

        using var db = CreateConnection();
        int rowsAffected = await db.ExecuteAsync(sql, product);
        return rowsAffected > 0;
    }
}