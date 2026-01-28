using Dapper;
// using Npgsql;
using System.Data;
using Microsoft.Data.SqlClient;
using MyBackend.Models.Test;
using MyBackend.Models.Paged;
using MyBackend.Data;

namespace MyBackend.Repositories.Test;

public class ProductRepository : IProductRepository
{
    // private readonly string _connectionString;
    // public ProductRepository(IConfiguration configuration)
    // {
    //     // ดึง Connection String จาก appsettings.json
    //     _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    // }
    // // สร้าง Connection สำหรับ PostgreSQL
    // //private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
    // // 1. เปลี่ยนตัวเชื่อมต่อเป็น SqlConnection สำหรับ SQL Server
    // private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    private readonly DbConnectionFactory _context;

    // Inject DapperContext เข้ามาแทน
    public ProductRepository(DbConnectionFactory context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM products WHERE id = @Id";
        using var db = _context.CreateConnection();
        // @Id เป็นการใช้ Parameter เพื่อป้องกัน SQL Injection
        return await db.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        const string sql = "SELECT id, name, price FROM Products ORDER BY id";
        using var db = _context.CreateConnection();
        return await db.QueryAsync<Product>(sql);
    }

    public async Task<int> CreateAsync(Product product)
    {
        const string sql = @"INSERT INTO products (name, price) 
                             VALUES (@name, @price);
                             SELECT CAST(SCOPE_IDENTITY() as int);";
        using var db = _context.CreateConnection();
        return await db.ExecuteScalarAsync<int>(sql, product);
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        const string sql = @"UPDATE products 
                            SET name = @name, 
                                price = @price 
                            WHERE id = @id";

        using var db = _context.CreateConnection();
        int rowsAffected = await db.ExecuteAsync(sql, product);
        return rowsAffected > 0;
    }

    public async Task<PagedResult<Product>> GetPagedAsync(int pageNo, int pageSize)
    {
        var allData = await GetAllAsync(); // ใช้ Method เดิมที่คุณเขียนไว้อ่านไฟล์ JSON

        var totalItems = allData.Count();

        // คำนวณการข้ามและการดึงข้อมูล
        var pagedData = allData
        .OrderBy(t => t.id)
        .ThenByDescending(t => t.name)
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<Product>
        {
            TotalItems = totalItems,
            PageNo = pageNo,
            PageSize = pageSize,
            Data = pagedData
        };
    }
}