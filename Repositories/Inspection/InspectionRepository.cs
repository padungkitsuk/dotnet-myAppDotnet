using Dapper;
using System.Data;
using MyBackend.Models.Paged;
using MyBackend.Data;
using MyBackend.Models.Inspection;

namespace MyBackend.Repositories.Inspection;

public class InspectionRepository : IInspectionRepository
{
    private readonly DbConnectionFactory _context;
    public InspectionRepository(DbConnectionFactory context)
    {
        _context = context;
    }

    public async Task<InspectionTransaction?> GetByIdAsync(string job_id)
    {
        const string sql = "SELECT * FROM inspection_transaction WHERE job_id = @Id";
        using var db = _context.CreateConnection();
        // @Id เป็นการใช้ Parameter เพื่อป้องกัน SQL Injection
        return await db.QueryFirstOrDefaultAsync<InspectionTransaction>(sql, new { jobId = job_id });
    }

    public async Task<IEnumerable<InspectionTransaction>> GetAllAsync()
    {
        const string sql = @"SELECT job_id, ref_no, job_create_by, job_create_date, job_owner, [source], agent_code, bu_code, policy_no, policy_effective_date, customer_type, customer_first_name, customer_last_name, customer_phone, payment_info, fleet_status, fleet_id, car_type, car_red_license, car_plate_no, car_province, car_brand, car_model, car_sub_model, chassis_number, appointment_status, no_survey_status, no_survey_code, no_survey_desc, job_desc 
        FROM inspection_transaction 
        ORDER BY job_id";
        using var db = _context.CreateConnection();
        return await db.QueryAsync<InspectionTransaction>(sql);
    }

    public async Task<int> CreateAsync(InspectionTransaction d)
    {
        const string sql = @"INSERT INTO products (name, price) 
                             VALUES (@name, @price);
                             SELECT CAST(SCOPE_IDENTITY() as int);";
        using var db = _context.CreateConnection();
        return await db.ExecuteScalarAsync<int>(sql, d);
    }

    public async Task<bool> UpdateAsync(InspectionTransaction d)
    {
        const string sql = @"UPDATE products 
                            SET name = @name, 
                                price = @price 
                            WHERE id = @id";

        using var db = _context.CreateConnection();
        int rowsAffected = await db.ExecuteAsync(sql, d);
        return rowsAffected > 0;
    }

    public async Task<PagedResult<InspectionTransaction>> GetPagedAsync(int pageNo, int pageSize)
    {
        var allData = await GetAllAsync(); // ใช้ Method เดิมที่คุณเขียนไว้อ่านไฟล์ JSON

        var totalItems = allData.Count();

        // คำนวณการข้ามและการดึงข้อมูล
        var pagedData = allData
        .OrderBy(t => t.jobId)
        .ThenByDescending(t => t.jobCreateDate)
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<InspectionTransaction>
        {
            TotalItems = totalItems,
            PageNo = pageNo,
            PageSize = pageSize,
            Data = pagedData
        };
    }
}