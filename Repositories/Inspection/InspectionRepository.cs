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

        var sql = new StringBuilder(@"
        SELECT 
	        it.job_id, it.ref_no, it.job_create_by, FORMAT(it.job_create_date,'yyyy-MM-dd HH:mm') job_create_date, it.job_owner, it.[source], it.agent_code, it.bu_code, 
	        it.policy_no, FORMAT(it.policy_effective_date,'yyyy-MM-dd') policy_effective_date, 
	        it.customer_type, it.customer_first_name, it.customer_last_name, it.customer_phone, it.payment_info, 
	        it.fleet_status, it.fleet_id, 
	        it.car_type, it.car_red_license, it.car_plate_no, 
	        it.car_province, 
	        mp.desc_th as car_province_desc, 
	        it.car_brand, it.car_model, it.car_sub_model, it.chassis_number, 
	        it.appointment_status, it.no_survey_status, it.no_survey_code, it.no_survey_desc,
	        it.cancel_status ,
	        case when it.cancel_status = 'cancel' then it.cancel_status else it.job_status end as job_status, 
	        mjs.state_desc as job_status_desc,
	        it.job_desc,
	        it.informer_first_name, it.informer_last_name, it.informer_phone, it.informer_emails 
        FROM inspection_transaction it
        LEFT JOIN master_province mp ON it.car_province = mp.code
        LEFT JOIN master_job_state mjs ON mjs.state_code = (case when it.cancel_status = 'cancel' then it.cancel_status else it.job_status end)
        WHERE 1=1  
        ");

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(d.JobOwner))
        {
            sql.Append(" AND it.job_owner = @JobOwner ");
            parameters.Add("JobOwner", d.JobOwner);
        }

        if (!string.IsNullOrEmpty(d.JobId))
        {
            sql.Append(" AND it.job_id = @JobId ");
            parameters.Add("JobId", d.JobId);
        }

        if (!string.IsNullOrEmpty(d.CustomerType))
        {
            sql.Append(" AND it.customer_type = @CustomerType ");
            parameters.Add("CustomerType", d.CustomerType);
        }

        

        // -- 1. เรียงตามปี -- 2. เรียงตามลำดับ -- 3. กรณีเป็นตัวอักษร
        sql.Append(@" ORDER BY 
	    TRY_CAST(
	        CASE WHEN CHARINDEX('/', it.job_id) > 0 
	             THEN LEFT(it.job_id, CHARINDEX('/', it.job_id) - 1) 
	             ELSE it.job_id 
	        END AS INT
	    ) DESC,
	    CASE WHEN CHARINDEX('/', it.job_id) > 0 
	         THEN LEFT(it.job_id, CHARINDEX('/', it.job_id) - 1) 
	         ELSE it.job_id 
	    END DESC,
	    TRY_CAST(
	        CASE WHEN CHARINDEX('/', it.job_id) > 0 
	             THEN SUBSTRING(it.job_id, CHARINDEX('/', it.job_id) + 1, LEN(it.job_id)) 
	             ELSE '0' 
	        END AS INT
	    ) DESC,
	    it.job_id DESC ");

        using var db = _context.CreateConnection();

        var allData = await db.QueryAsync<InspectionTransaction>(sql.ToString(), parameters);

        var totalItems = allData.Count();
        var pagedData = allData
        //.OrderBy(t => t.JobId)
        //.ThenByDescending(t => t.JobCreateDate)
            .Skip((d.PageNo - 1) * d.PageSize)
            .Take(d.PageSize)
            .ToList();

        return new PagedResult<IEnumerable<InspectionTransaction>>
        {
            Pagination = new Pagination()
            {
                TotalRow = totalItems,
                PageNo = d.PageNo,
                PageSize = d.PageSize,
            },
            Data = pagedData
        };
    }

    public async Task<InspectionTransaction> GetByIdAsync(RequestDataInspection d)
    {
        const string sql = @"
        SELECT 
	        it.job_id, it.ref_no, it.job_create_by, FORMAT(it.job_create_date,'yyyy-MM-dd HH:mm') job_create_date, it.job_owner, it.[source], it.agent_code, it.bu_code, 
	        it.policy_no, FORMAT(it.policy_effective_date,'yyyy-MM-dd') policy_effective_date, 
	        it.customer_type, it.customer_first_name, it.customer_last_name, it.customer_phone, it.payment_info, 
	        it.fleet_status, it.fleet_id, 
	        it.car_type, it.car_red_license, it.car_plate_no, 
	        it.car_province, 
	        mp.desc_th as car_province_desc, 
	        it.car_brand, it.car_model, it.car_sub_model, it.chassis_number, 
	        it.appointment_status, it.no_survey_status, it.no_survey_code, it.no_survey_desc,
	        it.cancel_status ,
	        case when it.cancel_status = 'cancel' then it.cancel_status else it.job_status end as job_status, 
	        mjs.state_desc as job_status_desc,
	        it.job_desc,
	        it.informer_first_name, it.informer_last_name, it.informer_phone, it.informer_emails 
        FROM inspection_transaction it
        LEFT JOIN master_province mp ON it.car_province = mp.code
        LEFT JOIN master_job_state mjs ON mjs.state_code = (case when it.cancel_status = 'cancel' then it.cancel_status else it.job_status end)
        WHERE it.job_id = @JobId
        ";
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
            (job_id, ref_no, job_create_by, job_create_date, [source], agent_code, bu_code, 
             policy_no, policy_effective_date, customer_type, customer_first_name, customer_last_name, 
             customer_phone, payment_info, fleet_status, fleet_id, car_type, car_red_license, 
             car_plate_no, car_province, car_brand, car_model, car_sub_model, chassis_number, 
             appointment_status, no_survey_status, no_survey_code, no_survey_desc, job_status, job_desc,
             informer_first_name, informer_last_name, informer_phone, informer_emails) 
            VALUES 
            (@JobId, @RefNo, @JobCreateBy,     GETDATE(),      @Source, @AgentCode, @BuCode, 
             @PolicyNo, @PolicyEffectiveDate, @CustomerType, @CustomerFirstName, @CustomerLastName, 
             @CustomerPhone, @PaymentInfo, @FleetStatus, @FleetId, @CarType, @CarRedLicense, 
             @CarPlateNo, @CarProvince, @CarBrand, @CarModel, @CarSubModel, @ChassisNumber, 
             @AppointmentStatus, @NoSurveyStatus, @NoSurveyCode, @NoSurveyDesc, @JobStatus, @JobDesc,
			 @InformerFirstName, @InformerLastName, @InformerPhone, @InformerEmails);

            INSERT INTO inspection_transaction_history 
             (job_id, seq, create_date, create_by,    code,      status,    job_status, job_desc) 
            VALUES
             (@JobId, 1,   GETDATE(),   @JobCreateBy, @TaskCode, @TaskDesc,       '-',      '-');
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
                    CarProvinceDesc = d.CarProvinceDesc,
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

    public async Task<List<VehicleInfo>> GetFleetInfo(IEnumerable<string> jobIds)
    {
        // Dapper แปลง @jobIds เป็น ('xxx1', 'xxx2', ...)
        const string sql = @"
        select
        count(job_id) as fleet_count,
        fleet_id
        from inspection_transaction
        where job_id in @jobIds
        group by fleet_id
        ";

        using var db = _context.CreateConnection();

        // ส่ง carPlateNos เข้าไปตรงๆ Dapper จะจัดการที่เหลือให้
        var result = await db.QueryAsync<VehicleInfo>(sql, new { jobIds });
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

    public async Task<List<JobList>> GetJobListInfoById(string jobId)
    {
        const string sql = @"
        DECLARE @fleetId varchar(20), @fleetCount int;
        select @fleetId = fleet_id from inspection_transaction where job_id = @jobId;
		select @fleetCount = count(fleet_id) from inspection_transaction where fleet_id = @fleetId;
		if(@fleetCount > 0)
		begin
	        select job_id, car_plate_no  
	        from inspection_transaction 
	        where fleet_id = @fleetId;
		end
		else
		begin
			select job_id, car_plate_no  
	        from inspection_transaction 
	        where job_id = @jobId;
		end
        ";

        using var db = _context.CreateConnection();

        var result = await db.QueryAsync<JobList>(sql, new { jobId });
        return [.. result];
    }

    public async Task<string> GetSeq()
    {
        return await _seq.GetNextSequenceValue();
    }

    public async Task<IEnumerable<InspectionTaskDetail>> GetDetailTaskAsync(string jobId)
    {

        var sql = new StringBuilder(@"
            WITH AllTasks AS (
                SELECT 1 as t_type, '02' as group_code, job_id, seq, task_desc, task_complete_status, task_complete_date, task_status, task_detail, appointment_datetime, 
                    NULL as survey_date, NULL as survey_company_code, NULL as survey_company_type, NULL as survey_location_region, NULL as survey_location_province, NULL as survey_location_district, NULL as survey_price1, NULL as survey_price2, 
                    NULL as verify_result_datetime , NULL as result_report , NULL as mile_number , NULL as car_modification , NULL as car_inspection_result , 
                    NULL as car_type , NULL as spare , NULL as gas, NULL as gas_number , NULL as gas_type , NULL as gas_price , NULL as modify_vehicle ,NULL as remark_code
                FROM inspection_task_001
                UNION ALL
                SELECT 2, '03', job_id, seq, task_desc, task_complete_status, task_complete_date, task_status, task_detail, NULL, 
                    survey_date, survey_company_code, survey_company_type, survey_location_region, survey_location_province, survey_location_district, survey_price1, survey_price2,
                    NULL, NULL, NULL, NULL, NULL,   
                    NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL
                FROM inspection_task_002
                UNION ALL
                SELECT 3, '04', job_id, seq, task_desc, task_complete_status, task_complete_date, task_status, task_detail, NULL, 
                    NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 
                    NULL, NULL, NULL, NULL, NULL,   
                    NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL 
                FROM inspection_task_003
                UNION ALL
                SELECT 4, '05', job_id, seq, task_desc, task_complete_status, task_complete_date, task_status, task_detail, appointment_datetime, 
                    NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 
                    verify_result_datetime ,result_report ,mile_number ,car_modification ,car_inspection_result ,
                    car_type ,spare ,gas, gas_number ,gas_type ,gas_price ,modify_vehicle ,remark_code 
                FROM inspection_task_004
            )
            SELECT 
                -- คำนวณลำดับ Task อัตโนมัติ (Task 1-4 สำหรับ seq 1, 5-8 สำหรับ seq 2, 9-12 สำหรับ seq 3)
                CAST(((t.seq - 1) * 4) + t_type AS VARCHAR(10)) as task,
                t.task_desc,
                t.task_status,
                mjs.state_desc as task_status_desc,
                t.task_complete_status,
                CASE WHEN t.task_complete_status = '002' THEN 'Complete' ELSE 'In Progress' END as task_complete_status_desc,
                CASE WHEN t.task_complete_status = '002' THEN FORMAT(t.task_complete_date, 'yyyy-MM-dd HH:mm') ELSE NULL END as task_complete_date,
                FORMAT(t.appointment_datetime, 'yyyy-MM-dd HH:mm') as appointment_datetime,
                t.task_detail,
                FORMAT(t.survey_date, 'yyyy-MM-dd HH:mm') as survey_date,
                t.survey_company_code, t.survey_company_type, t.survey_location_region, t.survey_location_province, t.survey_location_district, t.survey_price1, t.survey_price2,
                t.remark_code,
                it.bu_code,
                FORMAT(t.verify_result_datetime, 'yyyy-MM-dd HH:mm') as verify_result_datetime,
                t.result_report  ,
                t.mile_number  ,
                t.car_modification  ,
                t.car_inspection_result  ,
                t.car_type  ,
                t.spare  ,
                t.gas , 
                t.gas_number  ,
                t.gas_type  ,
                t.gas_price  ,
                t.modify_vehicle  ,
                t.remark_code 
            FROM AllTasks t
            LEFT JOIN master_job_state mjs ON mjs.state_code = t.task_status
            LEFT JOIN inspection_transaction it ON t.job_id = it.job_id
            WHERE t.job_id = @JobId 
            ORDER BY t.seq, t_type ;
        ");

        using var db = _context.CreateConnection();

        var allData = await db.QueryAsync<InspectionTaskDetail>(sql.ToString(), new { jobId });

        return allData;
    }

    public async Task<List<InspectionTaskResponse>> UpdateTask001(List<InspectionTaskRequest> tasks)
    {
        var responseList = new List<InspectionTaskResponse>();
        using var db = (DbConnection)_context.CreateConnection();
        await db.OpenAsync();
        using var trans = await db.BeginTransactionAsync();

        // SQL ไม่ต้องมี Transaction ซ้อน
        const string sql = @" 
        DECLARE @StepLog bit=0
        IF NOT EXISTS (SELECT 1 FROM inspection_task_001 WHERE job_id = @JobId AND seq = @TaskSeq)
        BEGIN
            INSERT INTO inspection_task_001 
            (job_id, seq, task_desc, task_complete_status, task_complete_date, task_complete_by, task_status, appointment_datetime, task_detail, task_create_date, task_create_by) 
            VALUES
            (@JobId, @TaskSeq, @TaskDesc, @TaskCompleteStatus, @TaskCompleteDate,  @TaskCompleteBy, @TaskStatus, @AppointmentDatetime, @TaskDetail, GETDATE(), @TaskCreateBy);
			UPDATE inspection_transaction SET 
				job_status = @TaskCode ,
				cancel_status = (CASE WHEN @TaskStatus = 'cancel' THEN @TaskStatus ELSE NULL END)
			WHERE job_id = @JobId;
			SET @StepLog = 1;
        END
        ELSE
        BEGIN
	        IF NOT EXISTS (SELECT 1 FROM inspection_task_001 WHERE job_id = @JobId AND seq = @TaskSeq AND task_complete_status = '002')
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
            WHERE job_id = @JobId AND seq = @TaskSeq;
			UPDATE inspection_transaction SET 
				job_status = @TaskCode ,
				cancel_status = (CASE WHEN @TaskStatus = 'cancel' THEN @TaskStatus ELSE NULL END)
			WHERE job_id = @JobId;
			SET @StepLog = 1;
			END
        END
        IF (@StepLog = 1)
        BEGIN
        DECLARE @LastSeq INT, @TaskStatusDesc varchar(100);
        SELECT @LastSeq = ISNULL(MAX(seq), 0) + 1 FROM inspection_transaction_history WHERE job_id = @JobId;
		SELECT @TaskStatusDesc = mjs.state_desc FROM master_job_state mjs WHERE mjs.group_code ='02' and mjs.state_code = @TaskStatus;
        INSERT INTO inspection_transaction_history 
          (job_id, seq,      create_date, create_by,     code,      status,    job_code,    job_status,      job_desc) 
        VALUES
          (@JobId, @LastSeq, GETDATE(),   @TaskCreateBy, @TaskCode, @TaskDesc, @TaskStatus, @TaskStatusDesc, @TaskDetail);
		END
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
                    responseList.Add(new InspectionTaskResponse { JobId = d.JobId });
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

    public async Task<List<InspectionTaskResponse>> UpdateTask002(List<InspectionTaskRequest> tasks)
    {
        var responseList = new List<InspectionTaskResponse>();
        using var db = (DbConnection)_context.CreateConnection();
        await db.OpenAsync();
        using var trans = await db.BeginTransactionAsync();

        // SQL ไม่ต้องมี Transaction ซ้อน
        const string sql = @" 
        DECLARE @StepLog bit=0
        IF NOT EXISTS (SELECT 1 FROM inspection_task_002 WHERE job_id = @JobId AND seq = @TaskSeq)
        BEGIN
            INSERT INTO inspection_task_002 
            (job_id, seq, task_desc, task_complete_status, task_complete_date, task_complete_by, task_status, task_detail, task_create_date, task_create_by,
            survey_date, survey_company_code, survey_company_type, survey_location_region, survey_location_province, survey_location_district, survey_price1, survey_price2) 
            VALUES
            (@JobId, @TaskSeq, @TaskDesc, @TaskCompleteStatus, @TaskCompleteDate,  @TaskCompleteBy, @TaskStatus,    @TaskDetail, GETDATE(), @TaskCreateBy,
			@SurveyDate, @SurveyCompanyCode,  @SurveyCompanyType,  @SurveyLocationRegion,  @SurveyLocationProvince,  @SurveyLocationDistrict,  @SurveyPrice1,  @SurveyPrice2);
			UPDATE inspection_transaction SET 
				job_status = @TaskCode ,
				cancel_status = (CASE WHEN @TaskStatus = 'cancel' THEN @TaskStatus ELSE NULL END)
			WHERE job_id = @JobId;
			SET @StepLog = 1;
        END
        ELSE
        BEGIN
	        IF NOT EXISTS (SELECT 1 FROM inspection_task_002 WHERE job_id = @JobId AND seq = @TaskSeq AND task_complete_status = '002')
	        BEGIN
            UPDATE inspection_task_002 SET 
                task_complete_status = @TaskCompleteStatus, 
                task_complete_date = @TaskCompleteDate,
                task_complete_by = @TaskCompleteBy,
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
            WHERE job_id = @JobId AND seq = @TaskSeq;
			UPDATE inspection_transaction SET 
				job_status = @TaskCode ,
				cancel_status = (CASE WHEN @TaskStatus = 'cancel' THEN @TaskStatus ELSE NULL END)
			WHERE job_id = @JobId;
			SET @StepLog = 1;
			END
        END
        IF (@StepLog = 1)
        BEGIN
        DECLARE @LastSeq INT, @TaskStatusDesc varchar(100);
        SELECT @LastSeq = ISNULL(MAX(seq), 0) + 1 FROM inspection_transaction_history WHERE job_id = @JobId;
		SELECT @TaskStatusDesc = mjs.state_desc FROM master_job_state mjs WHERE mjs.group_code ='03' and mjs.state_code = @TaskStatus;
        INSERT INTO inspection_transaction_history 
          (job_id, seq,      create_date, create_by,     code,      status,    job_code,    job_status,      job_desc) 
        VALUES
          (@JobId, @LastSeq, GETDATE(),   @TaskCreateBy, @TaskCode, @TaskDesc, @TaskStatus, @TaskStatusDesc, @TaskDetail);
		END
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
                    responseList.Add(new InspectionTaskResponse { JobId = d.JobId });
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

    public async Task<List<InspectionTaskResponse>> UpdateTask003(List<InspectionTaskRequest> tasks)
    {
        var responseList = new List<InspectionTaskResponse>();
        using var db = (DbConnection)_context.CreateConnection();
        await db.OpenAsync();
        using var trans = await db.BeginTransactionAsync();

        const string sql = @" 
        DECLARE @StepLog bit=0
        IF NOT EXISTS (SELECT 1 FROM inspection_task_003 WHERE job_id = @JobId AND seq = @TaskSeq)
        BEGIN
            INSERT INTO inspection_task_003 
            (job_id, seq, task_desc, task_complete_status, task_complete_date, task_complete_by, task_status, task_detail, task_create_date, task_create_by) 
            VALUES
            (@JobId, @TaskSeq, @TaskDesc, @TaskCompleteStatus, @TaskCompleteDate,  @TaskCompleteBy,  @TaskStatus,  @TaskDetail, GETDATE(),       @TaskCreateBy);
			UPDATE inspection_transaction SET 
				job_status = @TaskCode ,
				cancel_status = (CASE WHEN @TaskStatus = 'cancel' THEN @TaskStatus ELSE NULL END)
			WHERE job_id = @JobId;
			SET @StepLog = 1;
        END
        ELSE
        BEGIN
	        IF NOT EXISTS (SELECT 1 FROM inspection_task_003 WHERE job_id = @JobId AND seq = @TaskSeq AND task_complete_status = '002')
	        BEGIN
            UPDATE inspection_task_003 SET 
                task_complete_status = @TaskCompleteStatus, 
                task_complete_date = @TaskCompleteDate,
                task_complete_by = @TaskCompleteBy,
                task_status = @TaskStatus,
                task_detail = @TaskDetail,
                task_update_date = GETDATE(),
                task_update_by = @TaskCreateBy
            WHERE job_id = @JobId AND seq = @TaskSeq;
			UPDATE inspection_transaction SET 
				job_status = @TaskCode ,
				cancel_status = (CASE WHEN @TaskStatus = 'cancel' THEN @TaskStatus ELSE NULL END)
			WHERE job_id = @JobId;
			SET @StepLog = 1;
			END
        END
        IF (@StepLog = 1)
        BEGIN
        DECLARE @LastSeq INT, @TaskStatusDesc varchar(100);
        SELECT @LastSeq = ISNULL(MAX(seq), 0) + 1 FROM inspection_transaction_history WHERE job_id = @JobId;
		SELECT @TaskStatusDesc = mjs.state_desc FROM master_job_state mjs WHERE mjs.group_code ='04' and mjs.state_code = @TaskStatus;
        INSERT INTO inspection_transaction_history 
          (job_id, seq,      create_date, create_by,     code,      status,    job_code,    job_status,      job_desc) 
        VALUES
          (@JobId, @LastSeq, GETDATE(),   @TaskCreateBy, @TaskCode, @TaskDesc, @TaskStatus, @TaskStatusDesc, @TaskDetail);
		END
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
                    responseList.Add(new InspectionTaskResponse { JobId = d.JobId });
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

    public async Task<List<InspectionTaskResponse>> UpdateTask004(List<InspectionTaskRequest> tasks)
    {
        var responseList = new List<InspectionTaskResponse>();
        using var db = (DbConnection)_context.CreateConnection();
        await db.OpenAsync();
        using var trans = await db.BeginTransactionAsync();

        // SQL ไม่ต้องมี Transaction ซ้อน
        const string sql = @" 
        DECLARE @StepLog bit=0
        IF NOT EXISTS (SELECT 1 FROM inspection_task_004 WHERE job_id = @JobId AND seq = @TaskSeq)
        BEGIN
            INSERT INTO inspection_task_004 
            (job_id, seq, appointment_datetime, task_desc, task_complete_status, task_complete_date, task_complete_by, task_status, task_detail, task_create_date, task_create_by,
             result_report, verify_result_datetime, mile_number, inspection_datetime, car_modification, car_inspection_result, car_type,
			 spare,  gas,   gas_number, gas_type,   gas_price, modify_vehicle, remark_code) 
            VALUES
            (@JobId, @TaskSeq,  @AppointmentDatetime, @TaskDesc,  @TaskCompleteStatus,   @TaskCompleteDate,  @TaskCompleteBy, @TaskStatus,  @TaskDetail, GETDATE(), @TaskCreateBy,
			 @ResultReport, @VerifyResultDatetime,  @MileNumber, @InspectionDatetime,  @CarModification, @CarInspectionResult, @CarType,
			 @Spare, @Gas,   @GasNumber,  @GasType, @GasPrice,    @ModifyVehicle, @RemarkCode);
			UPDATE inspection_transaction SET 
				job_status = @TaskCode ,
				cancel_status = (CASE WHEN @TaskStatus = 'cancel' THEN @TaskStatus ELSE NULL END)
			WHERE job_id = @JobId;
			SET @StepLog = 1;
        END
        ELSE
        BEGIN
	        IF NOT EXISTS (SELECT 1 FROM inspection_task_004 WHERE job_id = @JobId AND seq = @TaskSeq AND task_complete_status = '002')
	        BEGIN
            UPDATE inspection_task_004 SET 
                appointment_datetime = @AppointmentDatetime,
                task_complete_status = @TaskCompleteStatus, 
                task_complete_date = @TaskCompleteDate,
                task_complete_by = @TaskCompleteBy,
                task_status = @TaskStatus,
                task_detail = @TaskDetail,
                task_update_date = GETDATE(),
                task_update_by = @TaskCreateBy,
                result_report = @ResultReport, 
                verify_result_datetime = @VerifyResultDatetime, 
                mile_number = @MileNumber, 
                inspection_datetime = @InspectionDatetime, 
                car_modification = @CarModification,
                car_inspection_result = @CarInspectionResult, 
                car_type = @CarType,
                spare = @Spare,  
                gas = @Gas,   
                gas_number = @GasNumber, 
                gas_type = @GasType,   
                gas_price = @GasPrice, 
                modify_vehicle = @ModifyVehicle,
                remark_code = @RemarkCode
            WHERE job_id = @JobId AND seq = @TaskSeq;
			UPDATE inspection_transaction SET 
				job_status = @TaskCode ,
				cancel_status = (CASE WHEN @TaskStatus = 'cancel' THEN @TaskStatus ELSE NULL END)
			WHERE job_id = @JobId;
			SET @StepLog = 1;
			END
        END  
        IF (@StepLog = 1)
        BEGIN
        DECLARE @LastSeq INT, @TaskStatusDesc varchar(100);
        SELECT @LastSeq = ISNULL(MAX(seq), 0) + 1 FROM inspection_transaction_history WHERE job_id = @JobId;
		SELECT @TaskStatusDesc = mjs.state_desc FROM master_job_state mjs WHERE mjs.state_code = @TaskStatus;
        INSERT INTO inspection_transaction_history 
          (job_id, seq,      create_date, create_by,     code,      status,    job_code,    job_status,      job_desc) 
        VALUES
          (@JobId, @LastSeq, GETDATE(),   @TaskCreateBy, @TaskCode, @TaskDesc, @TaskStatus, @TaskStatusDesc, @TaskDetail);
		END
        SELECT 1;
        ";

        const string sqlDelModify = @"DELETE FROM inspection_modify_vehicle WHERE job_id = @JobId";

        const string sqlInsModify = @"
        INSERT INTO inspection_modify_vehicle 
        (job_id, accessory_no ,accessory_code ,accessory_desc ,accessory_brand ,accessory_price ,create_date)
        VALUES 
        (@JobId, @AccessoryNo , @AccessoryCode , @AccessoryDesc , @AccessoryBrand , @AccessoryPrice ,GETDATE())";

        try
        {
            foreach (var d in tasks)
            {
                // รันทีละตัวภายใต้ Transaction เดียวกัน
                int result = await db.ExecuteScalarAsync<int>(sql, d, transaction: trans);

                if (d.ResultReport == "Y" && d.ModifyVehicle == "Y" && d.ModifyVehicleList?.Any() == true)
                {
                    await db.ExecuteAsync(sqlDelModify, new { d.JobId }, transaction: trans);

                    var modifyParams = d.ModifyVehicleList.Select(v => new
                    {
                        d.JobId,
                        v?.AccessoryNo,
                        v?.AccessoryCode,
                        v?.AccessoryDesc,
                        v?.AccessoryBrand,
                        v?.AccessoryPrice
                    });

                    // ใช้ Dapper Feature: ส่ง List เข้าไปทีเดียวเพื่อทำ Batch Insert (เร็วขึ้นมาก)
                    await db.ExecuteAsync(sqlInsModify, modifyParams, transaction: trans);
                }

                if (result > 0)
                {
                    responseList.Add(new InspectionTaskResponse { JobId = d.JobId });
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
            code,
            status,
            job_code,
            job_status ,
            job_desc ,
            seq
            FROM inspection_transaction_history  
            WHERE job_id = @JobId
            ORDER BY create_date desc
        ";

        using var db = _context.CreateConnection();

        var allData = await db.QueryAsync<InspectionTransactionHistory>(sql, new { jobId });

        return allData;
    }

    public async Task<IEnumerable<InspectionTransactionHistory>> JobHistoryDelete(InspectionRequestJobId d)
    {
        var responseList = new List<InspectionTransactionHistory>();
        using var db = (DbConnection)_context.CreateConnection();
        await db.OpenAsync();
        using var trans = await db.BeginTransactionAsync();

        const string sql = @"
        DELETE FROM inspection_transaction_history  
        WHERE job_id = @JobId
        AND seq = @Seq ;
    ";

        try
        {
            // anonymous object เพื่อ map ค่าให้ชัดเจน
            int rowsAffected = await db.ExecuteAsync(sql, new { JobId = d.JobId, Seq = d.Seq }, transaction: trans);

            if (rowsAffected > 0)
            {
                responseList.Add(new InspectionTransactionHistory
                {
                    JobId = d.JobId,
                    Seq = d.Seq 
                });
            }

            await trans.CommitAsync();

            _logger.LogInformation("Deleted {Rows} history records for JobId: {JobId}, Seq: {Seq}", rowsAffected, d.JobId, d.Seq);

            return responseList;
        }
        catch (Exception ex)
        {
            if (trans.Connection != null)
            {
                await trans.RollbackAsync();
            }
            _logger.LogError(ex, "Delete history Repository Failed for JobId: {JobId}", d.JobId);
            throw;
        }
    }

    public async Task<List<InspectionTransaction>> AssignJob(string jobId, string userId)
    {
        var responseList = new List<InspectionTransaction>();
        using var db = (DbConnection)_context.CreateConnection();
        await db.OpenAsync();
        using var trans = await db.BeginTransactionAsync();

        const string sql = @" 
        UPDATE inspection_transaction 
        SET job_owner = @userId 
        WHERE job_id = @jobId
    ";

        try
        {
            // 1. ใช้ ExecuteAsync แทน ExecuteScalarAsync สำหรับคำสั่ง UPDATE
            // เพราะ ExecuteAsync จะคืนค่าจำนวนแถวที่ได้รับผลกระทบ (Rows Affected)
            int rowsAffected = await db.ExecuteAsync(sql, new { jobId, userId }, transaction: trans);

            if (rowsAffected > 0)
            {
                // 2. แก้ไข d.JobId เป็น jobId (ตาม Parameter ที่รับมา)
                responseList.Add(new InspectionTransaction { JobId = jobId });
            }

            await trans.CommitAsync();
            return responseList;
        }
        catch (Exception ex)
        {
            // 3. ตรวจสอบสถานะการเชื่อมต่อก่อน Rollback เพื่อป้องกัน Error ซ้ำซ้อน
            if (trans.Connection != null)
            {
                await trans.RollbackAsync();
            }
            _logger.LogError(ex, "AssignJob Repository Failed for JobId: {JobId}", jobId);
            throw;
        }
    }

    public async Task<IEnumerable<InspectionModifyVehicle>> GetModifyVehicle(string jobId)
    {

        const string sql = @"
        SELECT 
            accessory_no ,
            accessory_code ,
            accessory_desc ,
            accessory_brand ,
            accessory_price
        FROM inspection_modify_vehicle
        WHERE job_id = @JobId
        ORDER BY accessory_no
        ";

        using var db = _context.CreateConnection();

        var allData = await db.QueryAsync<InspectionModifyVehicle>(sql, new { jobId });

        return allData;
    }


}