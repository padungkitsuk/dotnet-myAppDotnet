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

    public async Task<List<MasterDropdown>> GetAgentList()
    {
        const string sql = @"
        select 
        code, 
        desc_th as [label]
        from master_agent
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
        SELECT code, label FROM (
            SELECT 
                state_code AS code, 
                state_desc AS label,
                1 AS group_priority,
                seq
            FROM master_job_state
            WHERE active_status = 1 AND group_code = @groupCode 
            UNION ALL
            SELECT 
                state_code AS code, 
                state_desc AS label,
                2 AS group_priority,
                seq
            FROM master_job_state 
            WHERE active_status = 1 AND group_code = '00' AND state_code = 'cancel'
        ) AS CombinedResult
        ORDER BY group_priority, seq;
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

    public async Task<List<MasterDropdown>> GetRegionList()
    {
        const string sql = @"
        select  
        mss.region_code as code,
        mr.desc_th as label
        from master_servey_service mss 
        left join master_region mr on mss.region_code = mr.code  
        where mss.active_status  = 1
        group by mss.region_code,mr.desc_th
        order by min(mr.seq)
        ";

        using var db = _context.CreateConnection();

        var result = await db.QueryAsync<MasterDropdown>(sql);
        return [.. result];
    }

    public async Task<List<MasterDropdown>> GetProvinceCodeList()
    {
        const string sql = @"
        select  
        mss.province_code as code,
        mp.desc_th as label
        from master_servey_service mss 
        left join master_province mp on mss.province_code  = mp.code2 
        where mss.active_status  = 1
        group by mss.province_code,mp.desc_th
        order by min(mp.seq)
        ";

        using var db = _context.CreateConnection();

        var result = await db.QueryAsync<MasterDropdown>(sql);
        return [.. result];
    }

    public async Task<List<MasterDropdown>> GetDistrictList()
    {
        const string sql = @"
        select  
        mss.district_code  as code,
        md.desc_th as label
        from master_servey_service mss 
        left join master_district md on mss.district_code   = md.code  
        where mss.active_status  = 1
        group by mss.district_code,md.desc_th
        order by min(md.seq)
        ";

        using var db = _context.CreateConnection();

        var result = await db.QueryAsync<MasterDropdown>(sql);
        return [.. result];
    }

    public async Task<List<MasterDropdownPrice>> GetServeyServiceList(string regionCode, string provinceCode, string districtCode)
    {
        const string sql = @"
        select 
        mss.price ,
        concat('ราคา ', mss.price ,' บาท (', mss.company_code ,') ', msc.desc_th , ' ', msc.contact ) label,
        mss.company_code as code
        from master_servey_service mss
        left join master_servey_company msc on mss.company_code = msc.code
        where mss.active_status = 1
        and mss.region_code = @regionCode
        and mss.province_code = @provinceCode
        and mss.district_code = @districtCode
        order by mss.price,msc.seq 
        ";

        using var db = _context.CreateConnection();

        var result = await db.QueryAsync<MasterDropdownPrice>(sql, new { regionCode, provinceCode, districtCode });
        return [.. result];
    }



}