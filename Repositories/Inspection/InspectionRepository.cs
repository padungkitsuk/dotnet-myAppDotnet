using Dapper;
using System.Data;
using MyBackend.Models.Paged;
using MyBackend.Data;
using MyBackend.Models.Inspection;
using MyBackend.Repositories.Sequence;
using MyBackend.Models.Vehicle;

namespace MyBackend.Repositories.Inspection;

public class InspectionRepository : IInspectionRepository
{
    private readonly DbConnectionFactory _context;
    private readonly ILogger<InspectionRepository> _logger;
    private readonly ISequenceRepository _seq;
    public InspectionRepository(DbConnectionFactory context, ILogger<InspectionRepository> logger, ISequenceRepository seq)
    {
        _context = context;
        _logger = logger;
        _seq = seq;
    }

    public async Task<InspectionTransaction?> GetByIdAsync(string id)
    {
        const string sql = @"SELECT job_id, ref_no, job_create_by, job_create_date, job_owner, [source], agent_code, bu_code, policy_no, policy_effective_date, customer_type, customer_first_name, customer_last_name, customer_phone, payment_info, fleet_status, fleet_id, car_type, car_red_license, car_plate_no, car_province, car_brand, car_model, car_sub_model, chassis_number, appointment_status, no_survey_status, no_survey_code, no_survey_desc, job_desc
        FROM inspection_transaction 
        WHERE job_id = @JobId";
        using var db = _context.CreateConnection();
        return await db.QueryFirstOrDefaultAsync<InspectionTransaction>(sql, new { JobId = id });
    }

    public async Task<IEnumerable<InspectionTransaction>> GetAllAsync()
    {
        const string sql = @"SELECT job_id, ref_no, job_create_by, job_create_date, job_owner, [source], agent_code, bu_code, policy_no, policy_effective_date, customer_type, customer_first_name, customer_last_name, customer_phone, payment_info, fleet_status, fleet_id, car_type, car_red_license, car_plate_no, car_province, car_brand, car_model, car_sub_model, chassis_number, appointment_status, no_survey_status, no_survey_code, no_survey_desc, job_desc 
        FROM inspection_transaction 
        ORDER BY job_id";
        using var db = _context.CreateConnection();
        return await db.QueryAsync<InspectionTransaction>(sql); // QueryAsync จะเปิดและปิด connection ให้เราเองเมื่อใช้ร่วมกับ using var db
    }

    public async Task<string> CreateAsync(InspectionTransaction d)
    {
        string newJobId = await _seq.GetNextSequenceValue();
        d.JobId = newJobId;

        using var db = _context.CreateConnection();
        db.Open();
        using var trans = db.BeginTransaction();

        try
        {
            const string sqlInsert = @"
            INSERT INTO inspection_transaction 
            (job_id, ref_no, job_create_by, job_create_date, job_owner, [source], agent_code, bu_code, 
             policy_no, policy_effective_date, customer_type, customer_first_name, customer_last_name, 
             customer_phone, payment_info, fleet_status, fleet_id, car_type, car_red_license, 
             car_plate_no, car_province, car_brand, car_model, car_sub_model, chassis_number, 
             appointment_status, no_survey_status, no_survey_code, no_survey_desc, job_desc) 
            VALUES 
            (@jobId, @refNo, @jobCreateBy, GETDATE(), @jobOwner, @source, @agentCode, @buCode, 
             @policyNo, @policyEffectiveDate, @customerType, @customerFirstName, @customerLastName, 
             @customerPhone, @paymentInfo, @fleetStatus, @fleetId, @carType, @carRedLicense, 
             @carPlateNo, @carProvince, @carBrand, @carModel, @carSubModel, @chassisNumber, 
             @appointmentStatus, @noSurveyStatus, @noSurveyCode, @noSurveyDesc, @jobDesc);";

            await db.ExecuteAsync(sqlInsert, d, transaction: trans);

            trans.Commit();
            return newJobId;
        }
        catch (Exception ex)
        {
            trans.Rollback();
            _logger.LogError(ex, "Insert Inspection Failed JobId: {JobId}", d.JobId);
            throw;
        }
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
        var allData = await GetAllAsync();
        var totalItems = allData.Count();
        var pagedData = allData
        .OrderBy(t => t.JobId)
        .ThenByDescending(t => t.JobCreateDate)
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<InspectionTransaction>
        {
            TotalRow = totalItems,
            PageNo = pageNo,
            PageSize = pageSize,
            Data = pagedData
        };
    }

    public async Task<List<VehicleInfo>> GetCarInfo(IEnumerable<string> carPlateNos)
    {
        // Dapper จะจัดการแปลง @carPlateNos เป็น ('ทะเบียน1', 'ทะเบียน2', ...) ให้เอง
        const string sql = @"
        SELECT job_id, car_plate_no, car_province 
        FROM inspection_transaction 
        WHERE car_plate_no IN @carPlateNos";

        using var db = _context.CreateConnection();

        // ส่ง carPlateNos เข้าไปตรงๆ Dapper จะจัดการที่เหลือให้
        var result = await db.QueryAsync<VehicleInfo>(sql, new { carPlateNos });
        return result.ToList();
    }

    public async Task<string> GetSeq()
    {
        return await _seq.GetNextSequenceValue();
    }
}