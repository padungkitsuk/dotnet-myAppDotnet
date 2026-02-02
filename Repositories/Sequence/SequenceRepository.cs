using System.Text.Encodings.Web;
using System.Text.Json;
using Dapper;
using MyBackend.Data;
using MyBackend.Models.Sequence;

namespace MyBackend.Repositories.Sequence;

public class SequenceRepository : ISequenceRepository
{
    private readonly DbConnectionFactory _context;
    private readonly ILogger<SequenceRepository> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        //, WriteIndented = true
    };

    public SequenceRepository(DbConnectionFactory context, ILogger<SequenceRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string> GetNextSequenceValue()
    {
        using var db = _context.CreateConnection();
        int currentYear = DateTime.Now.Year;

        // ใช้ Transaction ระดับ SQL เพื่อป้องกันการ Restart Sequence ซ้ำซ้อน
        const string sql = @"
        SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
        BEGIN TRANSACTION;
            DECLARE @NextVal INT;
            DECLARE @LastYear INT = (SELECT TOP 1 [year] FROM running_job WITH (UPDLOCK, HOLDLOCK) ORDER BY [year] DESC);

            IF @LastYear IS NULL OR @currentYear > @LastYear
            BEGIN
                -- รีเซ็ต Sequence เมื่อขึ้นปีใหม่
                EXEC('ALTER SEQUENCE runningJobId RESTART WITH 1');
                SELECT @NextVal = NEXT VALUE FOR runningJobId;
                INSERT INTO running_job ([year], job_count) VALUES (@currentYear, @NextVal);
            END
            ELSE
            BEGIN
                SELECT @NextVal = NEXT VALUE FOR runningJobId;
                UPDATE running_job SET job_count = @NextVal WHERE [year] = @currentYear;
            END

            SELECT @NextVal;
        COMMIT TRANSACTION;";

        try
        {
            // connect DB return NextVal
            int nextVal = await db.ExecuteScalarAsync<int>(sql, new { currentYear });

            //_logger.LogInformation("Generated JobId: {Year}{NextVal:D6}", currentYear, nextVal);
            string jobId = $"{currentYear}/{nextVal}";
            _logger.LogInformation("Generated JobId: {JobId}", jobId);

            //return $"{currentYear}{nextVal:D6}";
            return jobId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sequence for year {Year}", currentYear);
            throw;
        }
    }


    public async Task<string> GetNextFleetValue()
    {
        using var db = _context.CreateConnection();
        const string sql = "SELECT NEXT VALUE FOR runningFleetId;";

        try
        {
            int nextVal = await db.ExecuteScalarAsync<int>(sql);
            _logger.LogInformation("Generated fleetId: F{NextVal}", nextVal);
            return $"F{nextVal}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating fleet");
            throw;
        }
    }


}