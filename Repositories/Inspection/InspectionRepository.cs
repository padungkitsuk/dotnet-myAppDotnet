using Dapper;
using System.Data;
using MyBackend.Models.Utils.Paged;
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

    public async Task<InspectionTransaction> GetByIdAsync(RequestDataInspection d)
    {
        const string sql = @"SELECT job_id, ref_no, job_create_by, FORMAT(job_create_date,'yyyy-MM-dd HH:mm') job_create_date, job_owner, [source], agent_code, bu_code, policy_no, FORMAT(policy_effective_date,'yyyy-MM-dd') policy_effective_date, customer_type, customer_first_name, customer_last_name, customer_phone, payment_info, fleet_status, fleet_id, car_type, car_red_license, car_plate_no, car_province, car_brand, car_model, car_sub_model, chassis_number, appointment_status, no_survey_status, no_survey_code, no_survey_desc, job_status, job_desc
        FROM inspection_transaction 
        WHERE job_id = @JobId";
        using var db = _context.CreateConnection();
        var result = await db.QueryFirstOrDefaultAsync<InspectionTransaction>(sql, new { JobId = d.JobId });
        if (result == null) return new InspectionTransaction();

        return result;
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
             @appointmentStatus, @noSurveyStatus, @noSurveyCode, @noSurveyDesc, @jobStatus, @jobDesc);
             
            INSERT INTO inspection_transaction_history 
             (job_id, seq, create_date, create_by,    status,   job_status, job_desc) 
            VALUES
             (@JobId, 1,   GETDATE(),   @jobCreateBy, N'งานใหม่', '-',            '-');
             ";

            string? newFleetId = (fleetStatus == "Y") ? await _seq.GetNextFleetValue() : null;

            foreach (var d in transactions)
            {
                // JobId
                string newJobId = await _seq.GetNextSequenceValue();

                d.JobId = newJobId;
                d.RefNo = string.IsNullOrEmpty(d.RefNo) ? await _seq.GetNextRefNoValue() : d.RefNo;
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

    public async Task<bool> UpdateJobHistory(InspectionTransactionHistory d)
    {
        const string sql = @" 
        BEGIN TRY
            IF EXISTS (SELECT 1 FROM inspection_transaction WHERE job_id = @JobId)
            BEGIN
                BEGIN TRANSACTION;
                    
                    DECLARE @LastSeq INT;
                    SELECT @LastSeq = ISNULL(MAX(seq), 0) + 1 FROM inspection_transaction_history WHERE job_id = @JobId;

                    INSERT INTO inspection_transaction_history 
                    (job_id, seq,      create_date, create_by,  status,  job_status, job_desc) 
                    VALUES
                    (@JobId, @LastSeq, GETDATE(),   @CreateBy,  @Status, @JobStatus, @JobDesc);

                    UPDATE inspection_transaction SET job_update_date = GETDATE() WHERE job_id = @JobId;

                COMMIT TRANSACTION;
                SELECT 1; -- คืนค่าว่าทำงานสำเร็จ
            END
            ELSE
            BEGIN
                SELECT 0; -- คืนค่าว่าไม่พบ JobId (จะทำให้ rowsAffected เป็น 0)
            END
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
            THROW; 
        END CATCH;";

        using var db = _context.CreateConnection();
        try
        {
            if (string.IsNullOrEmpty(d.JobId)) return false;

            int result = await db.ExecuteScalarAsync<int>(sql, d);

            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for JobId: {JobId}", d.JobId);
            throw;
        }
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
        return [.. result];
    }

    public async Task<List<JobList>> GetJobListInfo(string fleetId)
    {
        const string sql = @"
        select job_id, car_plate_no  
        from inspection_transaction 
        where fleet_id = @fleetId";

        using var db = _context.CreateConnection();

        var result = await db.QueryAsync<JobList>(sql, new { fleetId });
        return [.. result];
    }

    public async Task<string> GetSeq()
    {
        return await _seq.GetNextSequenceValue();
    }

    public async Task<IEnumerable<InspectionTaskDetail>> GetTaskDetailAsync(string jobId)
    {

        var sql = new StringBuilder(@"
            SELECT 
                '1' as task,
                it01.task_desc,
                it01.task_complete_status,
                case when it01.task_complete_status = '002' 
                    then 'Complete' 
                    else 'In Progress' 
                end as task_complete_status_desc,
                case when it01.task_complete_status = '002' 
                    then format(it01.task_complete_date,'yyyy-MM-dd HH:mm') 
                    else null          
                end as  task_complete_date,
                format(it01.appointment_datetime,'yyyy-MM-dd HH:mm') as appointment_datetime,
                it01.task_status ,
                mjs.state_desc as task_status_desc,
                it01.task_detail ,
                null survey_date, null survey_company_code, null survey_company_type, null survey_location_region, null survey_location_province, null survey_location_district, null survey_price1, null survey_price2
            FROM inspection_task_001 it01 
            LEFT  JOIN  master_job_state mjs on mjs.group_code ='02' and mjs.state_code = it01.task_status
            WHERE it01.job_id = @jobId and round = '1' 
        UNION ALL 
            SELECT 
                '2' as task,
                it02.task_desc,
                it02.task_complete_status,
                case when it02.task_complete_status = '002' 
                    then 'Complete' 
                    else 'In Progress' 
                end as task_complete_status_desc,
                case when it02.task_complete_status = '002' 
                    then format(it02.task_complete_date,'yyyy-MM-dd HH:mm') 
                    else null          
                end as  task_complete_date,
                null ,
                it02.task_status ,
                mjs.state_desc as task_status_desc,
                it02.task_detail ,
                format(it02.survey_date,'yyyy-MM-dd HH:mm') survey_date, survey_company_code, survey_company_type, survey_location_region, survey_location_province, survey_location_district, survey_price1, survey_price2
            FROM inspection_task_002 it02 
            LEFT  JOIN  master_job_state mjs on mjs.group_code ='03' and mjs.state_code = it02.task_status
            WHERE it02.job_id = @jobId and round = '1'
        UNION ALL 
            SELECT 
                '3' as task,
                it03.task_desc,
                it03.task_complete_status,
                case when it03.task_complete_status = '002' 
                    then 'Complete' 
                    else 'In Progress' 
                end as task_complete_status_desc,
                case when it03.task_complete_status = '002' 
                    then format(it03.task_complete_date,'yyyy-MM-dd HH:mm') 
                    else null          
                end as  task_complete_date,
                null ,
                it03.task_status ,
                mjs.state_desc as task_status_desc,
                it03.task_detail ,
                null , null , null , null , null , null , null , null
            FROM inspection_task_003 it03 
            LEFT  JOIN  master_job_state mjs on mjs.group_code ='04' and mjs.state_code = it03.task_status
            WHERE it03.job_id = @jobId and round = '1'
        UNION ALL 
            SELECT 
                '4' as task,
                it04.task_desc,
                it04.task_complete_status,
                case when it04.task_complete_status = '002' 
                    then 'Complete' 
                    else 'In Progress' 
                end as task_complete_status_desc,
                case when it04.task_complete_status = '002' 
                    then format(it04.task_complete_date,'yyyy-MM-dd HH:mm') 
                    else null          
                end as  task_complete_date,
                null ,
                it04.task_status ,
                mjs.state_desc as task_status_desc,
                it04.task_detail ,
                null , null , null , null , null , null , null , null
            FROM inspection_task_004 it04 
            LEFT  JOIN  master_job_state mjs on mjs.group_code ='05' and mjs.state_code = it04.task_status
            WHERE it04.job_id = @jobId and round = '1'
        UNION ALL
            SELECT 
                '5' as task,
                it01.task_desc,
                it01.task_complete_status,
                case when it01.task_complete_status = '002' 
                    then 'Complete' 
                    else 'In Progress' 
                end as task_complete_status_desc,
                case when it01.task_complete_status = '002' 
                    then format(it01.task_complete_date,'yyyy-MM-dd HH:mm') 
                    else null          
                end as  task_complete_date,
                format(it01.appointment_datetime,'yyyy-MM-dd HH:mm') as appointment_datetime,
                it01.task_status ,
                mjs.state_desc as task_status_desc,
                it01.task_detail ,
                null , null , null , null , null , null , null , null
            FROM inspection_task_001 it01 
            LEFT  JOIN  master_job_state mjs on mjs.group_code ='02' and mjs.state_code = it01.task_status
            WHERE it01.job_id = @jobId and round = '2' 
        UNION ALL 
            SELECT 
                '6' as task,
                it02.task_desc,
                it02.task_complete_status,
                case when it02.task_complete_status = '002' 
                    then 'Complete' 
                    else 'In Progress' 
                end as task_complete_status_desc,
                case when it02.task_complete_status = '002' 
                    then format(it02.task_complete_date,'yyyy-MM-dd HH:mm') 
                    else null          
                end as  task_complete_date,
                null ,
                it02.task_status ,
                mjs.state_desc as task_status_desc,
                it02.task_detail ,
                null , null , null , null , null , null , null , null
            FROM inspection_task_002 it02 
            LEFT  JOIN  master_job_state mjs on mjs.group_code ='03' and mjs.state_code = it02.task_status
            WHERE it02.job_id = @jobId and round = '2'
        UNION ALL 
            SELECT 
                '7' as task,
                it03.task_desc,
                it03.task_complete_status,
                case when it03.task_complete_status = '002' 
                    then 'Complete' 
                    else 'In Progress' 
                end as task_complete_status_desc,
                case when it03.task_complete_status = '002' 
                    then format(it03.task_complete_date,'yyyy-MM-dd HH:mm') 
                    else null          
                end as  task_complete_date,
                null ,
                it03.task_status ,
                mjs.state_desc as task_status_desc,
                it03.task_detail ,
                null , null , null , null , null , null , null , null
            FROM inspection_task_003 it03 
            LEFT  JOIN  master_job_state mjs on mjs.group_code ='04' and mjs.state_code = it03.task_status
            WHERE it03.job_id = @jobId and round = '2'
        UNION ALL 
            SELECT 
                '8' as task,
                it04.task_desc,
                it04.task_complete_status,
                case when it04.task_complete_status = '002' 
                    then 'Complete' 
                    else 'In Progress' 
                end as task_complete_status_desc,
                case when it04.task_complete_status = '002' 
                    then format(it04.task_complete_date,'yyyy-MM-dd HH:mm') 
                    else null          
                end as  task_complete_date,
                null ,
                it04.task_status ,
                mjs.state_desc as task_status_desc,
                it04.task_detail ,
                null , null , null , null , null , null , null , null
            FROM inspection_task_004 it04 
            LEFT  JOIN  master_job_state mjs on mjs.group_code ='05' and mjs.state_code = it04.task_status
            WHERE it04.job_id = @jobId and round = '2'
        ");

        using var db = _context.CreateConnection();

        var allData = await db.QueryAsync<InspectionTaskDetail>(sql.ToString(), new { jobId });

        return allData;
    }

    public async Task<List<InspectionTransaction>> UpdateTask001(List<InspectionTaskRequest> tasks)
    {
        var responseList = new List<InspectionTransaction>();
        using var db = (DbConnection)_context.CreateConnection();
        await db.OpenAsync();
        using var trans = await db.BeginTransactionAsync();

        // SQL ไม่ต้องมี Transaction ซ้อน
        const string sql = @" 
        IF NOT EXISTS (SELECT 1 FROM inspection_task_001 WHERE job_id = @JobId AND round = @Round)
        BEGIN
            INSERT INTO inspection_task_001 
            (job_id, round, task_desc, task_complete_status, task_complete_date, task_complete_by, task_status, appointment_datetime, task_detail, task_create_date, task_create_by) 
            VALUES
            (@JobId, @Round, @TaskDesc, @TaskCompleteStatus, @TaskCompleteDate,  @TaskCompleteBy, @TaskStatus, @AppointmentDatetime, @TaskDetail, GETDATE(), @TaskCreateBy);
        END
        ELSE
        BEGIN
            UPDATE inspection_task_001 SET 
                task_complete_status = @TaskCompleteStatus, 
                task_complete_date = @TaskCompleteDate,
                task_complete_by = @TaskCompleteBy,
                task_status = @TaskStatus,
                appointment_datetime = @AppointmentDatetime,
                task_detail = @TaskDetail,
                task_update_date = GETDATE(),
                task_update_by = @TaskCreateBy
            WHERE job_id = @JobId AND round = @Round;
        END
        DECLARE @LastSeq INT, @TaskStatusDesc varchar(100);
        SELECT @LastSeq = ISNULL(MAX(seq), 0) + 1 FROM inspection_transaction_history WHERE job_id = @JobId;
		SELECT @TaskStatusDesc = mjs.state_desc FROM master_job_state mjs WHERE mjs.group_code ='02' and mjs.state_code = @TaskStatus;
        INSERT INTO inspection_transaction_history 
          (job_id, seq,      create_date, create_by,     status,    job_status, job_desc) 
        VALUES
          (@JobId, @LastSeq, GETDATE(),   @TaskCreateBy, @TaskDesc, @TaskStatusDesc, @TaskDetail);
        SELECT 1;
        ";

        try
        {
            foreach (var d in tasks)
            {
                // รันทีละตัวภายใต้ Transaction เดียวกัน
                int result = await db.ExecuteScalarAsync<int>(sql, d, transaction: trans);

                if (result > 0)
                {
                    responseList.Add(new InspectionTransaction { JobId = d.JobId });
                }
            }

            await trans.CommitAsync();
            return responseList;
        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            _logger.LogError(ex, "UpdateTask001 Repository Failed");
            throw;
        }
    }

    public async Task<List<InspectionTransaction>> UpdateTask002(List<InspectionTaskRequest> tasks)
    {
        var responseList = new List<InspectionTransaction>();
        using var db = (DbConnection)_context.CreateConnection();
        await db.OpenAsync();
        using var trans = await db.BeginTransactionAsync();

        // SQL ไม่ต้องมี Transaction ซ้อน
        const string sql = @" 
        IF NOT EXISTS (SELECT 1 FROM inspection_task_002 WHERE job_id = @JobId AND round = @Round)
        BEGIN
            INSERT INTO inspection_task_002 
            (job_id, round, task_desc, task_complete_status, task_status, task_detail, task_create_date, task_create_by,
            survey_date, survey_company_code, survey_company_type, survey_location_region, survey_location_province, survey_location_district, survey_price1, survey_price2) 
            VALUES
            (@JobId, @Round, @TaskDesc, @TaskCompleteStatus, @TaskStatus,    @TaskDetail, GETDATE(), @TaskCreateBy,
			@SurveyDate, @SurveyCompanyCode,  @SurveyCompanyType,  @SurveyLocationRegion,  @SurveyLocationProvince,  @SurveyLocationDistrict,  @SurveyPrice1,  @SurveyPrice2);
        END
        ELSE
        BEGIN
            UPDATE inspection_task_002 SET 
                task_complete_status = @TaskCompleteStatus, 
                task_status = @TaskStatus,
                task_detail = @TaskDetail,
                task_update_date = GETDATE(),
                task_update_by = @TaskCreateBy,
                survey_date = @SurveyDate, 
                survey_company_code = @SurveyCompanyCode, 
                survey_company_type = @SurveyCompanyType, 
                survey_location_region = @SurveyLocationRegion, 
                survey_location_province = @SurveyLocationProvince, 
                survey_location_district = @SurveyLocationDistrict, 
                survey_price1 = @SurveyPrice1, 
                survey_price2 = @SurveyPrice2
            WHERE job_id = @JobId AND round = @Round;
        END
        DECLARE @LastSeq INT, @TaskStatusDesc varchar(100);
        SELECT @LastSeq = ISNULL(MAX(seq), 0) + 1 FROM inspection_transaction_history WHERE job_id = @JobId;
		SELECT @TaskStatusDesc = mjs.state_desc FROM master_job_state mjs WHERE mjs.group_code ='03' and mjs.state_code = @TaskStatus;
        INSERT INTO inspection_transaction_history 
          (job_id, seq,      create_date, create_by,     status,    job_status, job_desc) 
        VALUES
          (@JobId, @LastSeq, GETDATE(),   @TaskCreateBy, @TaskDesc, @TaskStatusDesc, @TaskDetail);
        SELECT 1;
        ";

        try
        {
            foreach (var d in tasks)
            {
                // รันทีละตัวภายใต้ Transaction เดียวกัน
                int result = await db.ExecuteScalarAsync<int>(sql, d, transaction: trans);

                if (result > 0)
                {
                    responseList.Add(new InspectionTransaction { JobId = d.JobId });
                }
            }

            await trans.CommitAsync();
            return responseList;
        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            _logger.LogError(ex, "UpdateTask002 Repository Failed");
            throw;
        }
    }

    public async Task<List<InspectionTransaction>> UpdateTask003(List<InspectionTaskRequest> tasks)
    {
        var responseList = new List<InspectionTransaction>();
        using var db = (DbConnection)_context.CreateConnection();
        await db.OpenAsync();
        using var trans = await db.BeginTransactionAsync();

        const string sql = @" 
        IF NOT EXISTS (SELECT 1 FROM inspection_task_003 WHERE job_id = @JobId AND round = @Round)
        BEGIN
            INSERT INTO inspection_task_003 
            (job_id, round, task_desc, task_complete_status, task_status, task_detail, task_create_date, task_create_by) 
            VALUES
            (@JobId, @Round, @TaskDesc, @TaskCompleteStatus, @TaskStatus,    @TaskDetail, GETDATE(), @TaskCreateBy);
        END
        ELSE
        BEGIN
            UPDATE inspection_task_003 SET 
                task_complete_status = @TaskCompleteStatus, 
                task_status = @TaskStatus,
                task_detail = @TaskDetail,
                task_update_date = GETDATE(),
                task_update_by = @TaskCreateBy
            WHERE job_id = @JobId AND round = @Round;
        END
        DECLARE @LastSeq INT, @TaskStatusDesc varchar(100);
        SELECT @LastSeq = ISNULL(MAX(seq), 0) + 1 FROM inspection_transaction_history WHERE job_id = @JobId;
		SELECT @TaskStatusDesc = mjs.state_desc FROM master_job_state mjs WHERE mjs.group_code ='04' and mjs.state_code = @TaskStatus;
        INSERT INTO inspection_transaction_history 
          (job_id, seq,      create_date, create_by,     status,    job_status, job_desc) 
        VALUES
          (@JobId, @LastSeq, GETDATE(),   @TaskCreateBy, @TaskDesc, @TaskStatusDesc, @TaskDetail);
        SELECT 1;
        ";

        try
        {
            foreach (var d in tasks)
            {
                // รันทีละตัวภายใต้ Transaction เดียวกัน
                int result = await db.ExecuteScalarAsync<int>(sql, d, transaction: trans);

                if (result > 0)
                {
                    responseList.Add(new InspectionTransaction { JobId = d.JobId });
                }
            }

            await trans.CommitAsync();
            return responseList;
        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            _logger.LogError(ex, "UpdateTask003 Repository Failed");
            throw;
        }
    }

    public async Task<List<InspectionTransaction>> UpdateTask004(List<InspectionTaskRequest> tasks)
    {
        var responseList = new List<InspectionTransaction>();
        using var db = (DbConnection)_context.CreateConnection();
        await db.OpenAsync();
        using var trans = await db.BeginTransactionAsync();

        // SQL ไม่ต้องมี Transaction ซ้อน
        const string sql = @" 
        IF NOT EXISTS (SELECT 1 FROM inspection_task_004 WHERE job_id = @JobId AND round = @Round)
        BEGIN
            INSERT INTO inspection_task_004 
            (job_id, round, task_desc, task_complete_status, task_status, task_detail, task_create_date, task_create_by) 
            VALUES
            (@JobId, '1', @TaskDesc, @TaskCompleteStatus, @TaskStatus,    @TaskDetail, GETDATE(), @TaskCreateBy);
        END
        ELSE
        BEGIN
            UPDATE inspection_task_004 SET 
                task_complete_status = @TaskCompleteStatus, 
                task_status = @TaskStatus,
                task_detail = @TaskDetail,
                task_update_date = GETDATE(),
                task_update_by = @TaskCreateBy
            WHERE job_id = @JobId AND round = @Round;
        END
        DECLARE @LastSeq INT, @TaskStatusDesc varchar(100);
        SELECT @LastSeq = ISNULL(MAX(seq), 0) + 1 FROM inspection_transaction_history WHERE job_id = @JobId;
		SELECT @TaskStatusDesc = mjs.state_desc FROM master_job_state mjs WHERE mjs.group_code ='05' and mjs.state_code = @TaskStatus;
        INSERT INTO inspection_transaction_history 
          (job_id, seq,      create_date, create_by,     status,    job_status, job_desc) 
        VALUES
          (@JobId, @LastSeq, GETDATE(),   @TaskCreateBy, @TaskDesc, @TaskStatusDesc, @TaskDetail);
        SELECT 1;
        ";

        try
        {
            foreach (var d in tasks)
            {
                // รันทีละตัวภายใต้ Transaction เดียวกัน
                int result = await db.ExecuteScalarAsync<int>(sql, d, transaction: trans);

                if (result > 0)
                {
                    responseList.Add(new InspectionTransaction { JobId = d.JobId });
                }
            }

            await trans.CommitAsync();
            return responseList;
        }
        catch (Exception ex)
        {
            await trans.RollbackAsync();
            _logger.LogError(ex, "UpdateTask004 Repository Failed");
            throw;
        }
    }

    public async Task<IEnumerable<InspectionTransactionHistory>> GetJobHistory(string jobId)
    {

       const string sql = @"
            SELECT 
            format(create_date,'dd-MM-yyyy HH:mm:ss') create_date,
            create_by as user_id,
            create_by as user_name,
            status,
            job_status ,
            job_desc 
            FROM inspection_transaction_history  
            WHERE job_id = @jobId
            ORDER BY create_date desc
        ";

        using var db = _context.CreateConnection();

        var allData = await db.QueryAsync<InspectionTransactionHistory>(sql, new { jobId });

        return allData;
    }



}