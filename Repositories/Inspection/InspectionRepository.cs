using Dapper;
using System.Data;
using MyBackend.Models.Paged;
using MyBackend.Data;
using MyBackend.Models.Inspection;
using MyBackend.Repositories.Sequence;
using MyBackend.Models.Vehicle;
using System.Text;
using System.Data.Common;

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

    public async Task<PagedResult<IEnumerable<InspectionTransaction>>> GetPagedAsync(RequestDataInspection d)
    {

        var sql = new StringBuilder("SELECT job_id, ref_no, job_create_by, FORMAT(job_create_date,'yyyy-MM-dd HH:mm') job_create_date, job_owner, [source], agent_code, bu_code, policy_no, FORMAT(policy_effective_date,'yyyy-MM-dd') policy_effective_date, customer_type, customer_first_name, customer_last_name, customer_phone, payment_info, fleet_status, fleet_id, car_type, car_red_license, car_plate_no, car_province, car_brand, car_model, car_sub_model, chassis_number, appointment_status, no_survey_status, no_survey_code, no_survey_desc, job_status, job_desc FROM inspection_transaction WHERE 1=1 ");
        if (!string.IsNullOrEmpty(d.JobId))
        {
            sql.Append(" and job_id = '" + d.JobId + "' ");
        }

        sql.Append(" ORDER BY job_id ");
        using var db = _context.CreateConnection();

        var allData = await db.QueryAsync<InspectionTransaction>(sql.ToString());
        var totalItems = allData.Count();
        var pagedData = allData
        .OrderBy(t => t.JobId)
        .ThenByDescending(t => t.JobCreateDate)
            .Skip((d.PageNo - 1) * d.PageSize)
            .Take(d.PageSize)
            .ToList();

        return new PagedResult<IEnumerable<InspectionTransaction>>
        {
            TotalRow = totalItems,
            PageNo = d.PageNo,
            PageSize = d.PageSize,
            Data = pagedData
        };
    }

    public async Task<InspectionTransaction?> GetByIdAsync(RequestDataInspection d)
    {
        const string sql = @"SELECT job_id, ref_no, job_create_by, FORMAT(job_create_date,'yyyy-MM-dd HH:mm') job_create_date, job_owner, [source], agent_code, bu_code, policy_no, FORMAT(policy_effective_date,'yyyy-MM-dd') policy_effective_date, customer_type, customer_first_name, customer_last_name, customer_phone, payment_info, fleet_status, fleet_id, car_type, car_red_license, car_plate_no, car_province, car_brand, car_model, car_sub_model, chassis_number, appointment_status, no_survey_status, no_survey_code, no_survey_desc, job_status, job_desc
        FROM inspection_transaction 
        WHERE job_id = @JobId";
        using var db = _context.CreateConnection();
        return await db.QueryFirstOrDefaultAsync<InspectionTransaction>(sql, new { JobId = d.JobId });
    }

    public async Task<List<InspectionTransaction>> CreateAsync(List<InspectionTransaction> transactions, string? fleetStatus)
    {
        var responseList = new List<InspectionTransaction>();

        // ใช้ await using เพื่อประสิทธิภาพสูงสุดในการคืนค่า Resource
        await using var db = (DbConnection)_context.CreateConnection();
        await db.OpenAsync();
        await using var trans = await db.BeginTransactionAsync();

        try
        {
            const string sqlInsert = @"
            INSERT INTO inspection_transaction 
            (job_id, ref_no, job_create_by, job_create_date, job_owner, [source], agent_code, bu_code, 
             policy_no, policy_effective_date, customer_type, customer_first_name, customer_last_name, 
             customer_phone, payment_info, fleet_status, fleet_id, car_type, car_red_license, 
             car_plate_no, car_province, car_brand, car_model, car_sub_model, chassis_number, 
             appointment_status, no_survey_status, no_survey_code, no_survey_desc, job_status, job_desc) 
            VALUES 
            (@jobId, @refNo, @jobCreateBy, GETDATE(), @jobOwner, @source, @agentCode, @buCode, 
             @policyNo, @policyEffectiveDate, @customerType, @customerFirstName, @customerLastName, 
             @customerPhone, @paymentInfo, @fleetStatus, @fleetId, @carType, @carRedLicense, 
             @carPlateNo, @carProvince, @carBrand, @carModel, @carSubModel, @chassisNumber, 
             @appointmentStatus, @noSurveyStatus, @noSurveyCode, @noSurveyDesc, @jobStatus, @jobDesc);";

            string? newFleetId = (fleetStatus == "Y") ? await _seq.GetNextFleetValue() : null;

            foreach (var d in transactions)
            {
                // JobId
                string newJobId = await _seq.GetNextSequenceValue();

                d.JobId = newJobId;
                d.FleetId = newFleetId;
                d.FleetStatus = fleetStatus;

                // DB
                await db.ExecuteAsync(sqlInsert, d, transaction: trans);

                responseList.Add(new InspectionTransaction
                {
                    JobId = newJobId,
                    CarPlateNo = d.CarPlateNo,
                    CarProvince = d.CarProvince,
                    FleetId = newFleetId
                });
            }

            await trans.CommitAsync();
            return responseList;
        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            _logger.LogError(ex, "Insert Failed: {Message}. All changes rolled back.", ex.Message);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(InspectionTransaction d)
    {
        const string sqlUpdate = @" UPDATE inspection_transaction 
        SET 
        job_desc = @jobDesc,
        job_update_date = GETDATE() 
        WHERE 
        job_id = @jobId;";

        using var db = _context.CreateConnection();
        int rowsAffected = await db.ExecuteAsync(sqlUpdate, d);
        return rowsAffected > 0;
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