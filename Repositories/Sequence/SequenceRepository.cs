using System.Text.Encodings.Web;
using System.Text.Json;
using Dapper;
using MyBackend.Data;
using MyBackend.Models.Inspection;

namespace MyBackend.Repositories.Sequence;

public class SequenceRepository : ISequenceRepository
{
    private readonly DbConnectionFactory _context;
    private readonly ILogger<SequenceRepository> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new() {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true
    };

    public SequenceRepository(DbConnectionFactory context, ILogger<SequenceRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string> GetNextSequenceValue()
    {
        const string sql = @"SELECT TOP 1 LEFT(job_id, 4) AS job_id FROM inspection_transaction ORDER BY job_id DESC;";
        using var db = _context.CreateConnection();

        // 1. ดึงข้อมูลล่าสุด
        var result = await db.QueryFirstOrDefaultAsync<InspectionTransaction>(sql);
        _logger.LogInformation("nextVal: {Json} {Time}", JsonSerializer.Serialize(result, _jsonOptions), DateTime.Now);
        int currentYear = DateTime.Now.Year;

        // ตรวจสอบกรณีที่ตารางยังไม่มีข้อมูล (result เป็น null)
        if (result != null && !string.IsNullOrEmpty(result.jobId))
        {
            int lastUsedYear = Convert.ToInt32(result.jobId);

            if (currentYear > lastUsedYear)
            {
                // 2. ใช้ .ExecuteAsync แทน .ExecuteRaw
                await db.ExecuteAsync("ALTER SEQUENCE runningJobId RESTART WITH 1;");
            }
        }

        // 3. ดึงค่า Sequence (ใช้ ExecuteScalarAsync สำหรับ Async method)
        var nextVal = await db.ExecuteScalarAsync<int>("SELECT NEXT VALUE FOR runningJobId;");

        _logger.LogInformation("nextVal: {nextVal} {Time}", nextVal, DateTime.Now);
        // ส่งคืนค่าเป็น string ตาม Signature ของ Method (เช่น "1" หรือจัด Format ตามต้องการ)
        return $"{currentYear}{nextVal:D6}";;
    }


}