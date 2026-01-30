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

        // One Query for Performance
        const string sql = @"
        DECLARE @NextVal INT;
        DECLARE @LastYear INT = (SELECT TOP 1 [year] FROM running_job ORDER BY [year] DESC);

        -- 1. check for Reset Sequence
        IF @LastYear IS NULL OR @currentYear > @LastYear
        BEGIN
            EXEC('ALTER SEQUENCE runningJobId RESTART WITH 1');
            SELECT @NextVal = NEXT VALUE FOR runningJobId;
            INSERT INTO running_job ([year], job_count) VALUES (@currentYear, @NextVal);
        END
        ELSE
        BEGIN
            -- 2. case nornal
            SELECT @NextVal = NEXT VALUE FOR runningJobId;
            UPDATE running_job SET job_count = @NextVal WHERE [year] = @currentYear;
        END

        -- return
        SELECT @NextVal;";

        try
        {
            // connect DB return NextVal
            int nextVal = await db.ExecuteScalarAsync<int>(sql, new { currentYear });

            //_logger.LogInformation("Generated JobId: {Year}{NextVal:D6}", currentYear, nextVal);
            _logger.LogInformation("Generated JobId: {Year}/{NextVal}", currentYear, nextVal);

            //return $"{currentYear}{nextVal:D6}";
            return $"{currentYear}/{nextVal}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sequence for year {Year}", currentYear);
            throw;
        }
    }


}