using Dapper;
using System.Data;
using MyBackend.Models.Utils.Paged;
using MyBackend.Data;
using MyBackend.Models.Inspection;
using MyBackend.Repositories.Sequence;
using MyBackend.Models.Vehicle;
using System.Text;
using System.Data.Common;
using MyBackend.Models.Master;

namespace MyBackend.Repositories.Inspection;

public class MasterDataRepository : IMasterDataRepository
{
    private readonly DbConnectionFactory _context;
    private readonly ILogger<InspectionRepository> _logger;
    public MasterDataRepository(DbConnectionFactory context, ILogger<InspectionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<MasterDropdown>> GetSourceList()
    {
        const string sql = @"
        select 
        code, 
        desc_th as [label]
        from master_source
        where active_status = 1
        order by seq
        ";

        using var db = _context.CreateConnection();

        var result = await db.QueryAsync<MasterDropdown>(sql);
        return [.. result];
    }

    public async Task<List<MasterDropdown>> GetBUList()
    {
        const string sql = @"
        select 
        code, 
        desc_th as [label]
        from master_bu
        where active_status = 1
        order by seq
        ";

        using var db = _context.CreateConnection();

        var result = await db.QueryAsync<MasterDropdown>(sql);
        return [.. result];
    }

    public async Task<List<MasterDropdown>> GetReasonServeyList()
    {
        const string sql = @"
        select 
        code, 
        desc_th as [label]
        from master_reason_servey
        where active_status = 1
        order by seq
        ";

        using var db = _context.CreateConnection();

        var result = await db.QueryAsync<MasterDropdown>(sql);
        return [.. result];
    }

    public async Task<List<MasterDropdown>> GetProvinceList()
    {
        const string sql = @"
        select 
        code, 
        desc_th as [label]
        from master_province
        where active_status = 1
        order by seq
        ";

        using var db = _context.CreateConnection();

        var result = await db.QueryAsync<MasterDropdown>(sql);
        return [.. result];
    }

    public async Task<List<MasterDropdown>> GetCarBrandList()
    {
        const string sql = @"
        SELECT 
            car_brand as code,
            car_brand as label
        FROM master_car
        GROUP BY car_brand
        ORDER BY MIN(seq)
        ";

        using var db = _context.CreateConnection();

        var result = await db.QueryAsync<MasterDropdown>(sql);
        return [.. result];
    }

    public async Task<List<MasterDropdown>> GetCarModelList(string carBrand)
    {
        const string sql = @"
        SELECT 
            car_model as code,
            car_model as label
        FROM master_car
        WHERE car_brand = @carBrand
        GROUP BY car_model
        ORDER BY MIN(seq)
        ";

        using var db = _context.CreateConnection();

        var result = await db.QueryAsync<MasterDropdown>(sql, new { carBrand });
        return [.. result];
    }

    public async Task<List<MasterDropdown>> GetJobStateList(string groupCode)
    {
        const string sql = @"
        select 
            state_code as code, 
            state_desc as label
        from master_job_state
        where active_status = 1
        and group_code = @groupCode 
        order by seq
        ";

        using var db = _context.CreateConnection();

        var result = await db.QueryAsync<MasterDropdown>(sql, new { groupCode });
        return [.. result];
    }

    public async Task<List<MasterDropdown>> GetRemarkMethodList()
    {
        const string sql = @"
        select 
            code, 
            desc_th as label
        from master_remark_method
        where active_status = 1
        order by seq
        ";

        using var db = _context.CreateConnection();

        var result = await db.QueryAsync<MasterDropdown>(sql);
        return [.. result];
    }

}