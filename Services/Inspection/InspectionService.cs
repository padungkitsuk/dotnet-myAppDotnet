using System.Text.Json;
using System.Text.Encodings.Web;
using MyBackend.Models.Inspection;
using MyBackend.Repositories.Inspection;
using MyBackend.Models.Utils.Paged;
using AutoMapper;
using MyBackend.Models.Utils.Api;
using MyBackend.Models.Vehicle;
using MyBackend.Utils.Constants;
using System.Text.Json.Serialization;
using System.Collections.Immutable;
using System.Globalization;

namespace MyBackend.Services.Inspection;

public class InspectionService : IInspectionService
{
    private readonly IInspectionRepository _repository;
    private readonly ILogger<InspectionService> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    private readonly IMapper _mapper;

    public InspectionService(IInspectionRepository repository, ILogger<InspectionService> logger, IMapper mapper)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
    }

    //string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "data.json");

    public async Task<PagedResult<IEnumerable<InspectionTransaction>>> GetPagedAsync(RequestDataInspection d)
    {
        return await _repository.GetPagedAsync(d);
    }

    public async Task<ApiResponse<InspectionTransactionDetail>> GetByIdAsync(RequestDataInspection d)
    {
        try
        {
            if (string.IsNullOrEmpty(d.JobId))
                return new ApiResponse<InspectionTransactionDetail> { Message = StatusConstant.NotFoundMessage, Status = StatusConstant.NotFoundCode };

            var detail = await _repository.GetByIdAsync(d);
            if (detail == null)
                return new ApiResponse<InspectionTransactionDetail> { Message = StatusConstant.NotFoundMessage, Status = StatusConstant.NotFoundCode };

            _logger.LogInformation("GetByIdAsync id: {id} res: {Json}", d.JobId, JsonSerializer.Serialize(detail, _jsonOptions));

            var result = new InspectionTransactionDetail { Detail = detail };

            if (detail.FleetStatus == "Y" && !string.IsNullOrEmpty(detail.FleetId))
            {
                result.JobList = await _repository.GetJobListInfo(detail.FleetId);
            }
            else
            {
                // กรณีไม่ใช่ Fleet หรือไม่มี FleetId ให้แสดงแค่คันเดียว
                result.JobList = [new() { JobId = detail.JobId ?? "", CarPlateNo = detail.CarPlateNo ?? "" }];
            }

            return new ApiResponse<InspectionTransactionDetail> { Data = result };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "เกิดข้อผิดพลาดใน GetByIdAsync");
            return new ApiResponse<InspectionTransactionDetail>()
            {
                Message = StatusConstant.ErrorMessage,
                Status = StatusConstant.ErrorCode
            };
        }
    }

    public async Task<ApiResponse<IEnumerable<VehicleInfo>>> CreateAsync(InspectionRequest d)
    {
        d.JobStatus = "NEW";
        //_logger.LogInformation("Inspect req: {Json}", JsonSerializer.Serialize(d, _jsonOptions));

        try
        {

            //Validate
            var isValid = d.FleetStatus == "Y" ? (d.VehicleInfo.Count >= 2) : (d.VehicleInfo.Count == 1);
            if (!isValid)
                return new ApiResponse<IEnumerable<VehicleInfo>> { Message = "VehicleInfo" + StatusConstant.InvalidInfoMessage, Status = StatusConstant.InvalidInfoCode };

            // check data
            if (d.NoSurveyStatus == null || d.NoSurveyStatus == "N")
            {
                d.NoSurveyStatus = "N";
                d.NoSurveyCode = null;
                d.NoSurveyDesc = null;
            }

            // Corporate customers
            if (d.CustomerInfo?.CustomerType == "C") d.CustomerInfo.CustomerLastName = null;

            d.VehicleInfo.ForEach(v =>
            {
                v.CarPlateNo = v.CarPlateNo?.Replace(" ", "");
                if (v.CarRedLicense == "Y")
                {
                    v.CarPlateNo = "ใหม่";
                    v.CarProvince = "99";
                }
            });

            var plates = (d.FleetStatus == "Y" ? d.VehicleInfo.Select(v => v.CarPlateNo) : d.VehicleInfo.Take(1).Select(v => v.CarPlateNo))
            .Where(p => !string.IsNullOrEmpty(p))
            .ToImmutableArray();


            var carsHistory = (await _repository.GetCarInfo(plates)).ToList();
            _logger.LogInformation("carsHistory: {Json}", JsonSerializer.Serialize(carsHistory, _jsonOptions));

            if (carsHistory.Count != 0)
            {
                // check Duplicate 
                var duplicates = carsHistory.Where(h =>
                    d.VehicleInfo.Any(v => v.CarPlateNo == h.CarPlateNo && v.CarProvince == h.CarProvince)
                ).ToList();

                if (duplicates.Count != 0)
                {
                    return new ApiResponse<IEnumerable<VehicleInfo>> { Message = StatusConstant.DuplicateMessage, Status = StatusConstant.DuplicateCode, Data = duplicates };
                }
            }

            var vehicleToProcess = d.FleetStatus == "Y" ? d.VehicleInfo : d.VehicleInfo.Take(1).ToList();

            var requests = vehicleToProcess.Select(v =>
            {
                var req = _mapper.Map<InspectionTransaction>(d);

                // Map Customer Info
                req.CustomerType = d.CustomerInfo?.CustomerType;
                req.CustomerFirstName = d.CustomerInfo?.CustomerFirstName;
                req.CustomerLastName = d.CustomerInfo?.CustomerLastName;
                req.CustomerPhone = d.CustomerInfo?.CustomerPhone;
                req.PaymentInfo = d.CustomerInfo?.PaymentInfo;

                // Map Vehicle Info
                req.CarType = v.CarType;
                req.CarRedLicense = v.CarRedLicense;
                req.CarPlateNo = v.CarPlateNo;
                req.CarProvince = v.CarProvince;
                req.CarBrand = v.CarBrand;
                req.CarModel = v.CarModel;
                req.CarSubModel = v.CarSubModel;
                req.ChassisNumber = v.ChassisNumber;

                return req;
            }).ToList();

            var savedData = await _repository.CreateAsync(requests, d.FleetStatus);

            var result = savedData.Select(s => new VehicleInfo
            {
                CarPlateNo = s.CarPlateNo,
                CarProvince = s.CarProvince,
                JobId = s.JobId
            });

            return new ApiResponse<IEnumerable<VehicleInfo>> { Data = result };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "เกิดข้อผิดพลาดใน CreateAsync");
            return new ApiResponse<IEnumerable<VehicleInfo>>
            {
                Message = StatusConstant.ErrorMessage,
                Status = StatusConstant.ErrorCode
            };

        }
    }

    public async Task<ApiResponse<IEnumerable<InspectionTransactionHistory>>> GetJobHistory(string jobId)
    {
        var result = await _repository.GetJobHistory(jobId);
        return new ApiResponse<IEnumerable<InspectionTransactionHistory>>()
        {
            Data = result
        };
    }

    public async Task<ApiResponse<IEnumerable<InspectionTaskDetail>>> GetTaskDetailAsync(string jobId)
    {
        var result = await _repository.GetTaskDetailAsync(jobId);
        return new ApiResponse<IEnumerable<InspectionTaskDetail>>()
        {
            Data = result
        };
    }

    public async Task<ApiResponse<IEnumerable<InspectionTransaction>>> UpdateTaskAsync(InspectionTaskRequest d)
    {
        try
        {
            if (d.JobList == null || d.JobList.Count == 0)
                return new ApiResponse<IEnumerable<InspectionTransaction>> { Message = "No jobs to update", Status = StatusConstant.NotFoundCode };

            // Task 5 => check RemarkCode {01,02} => {ลบรอย Remark ติดตามนัดหมายลูกค้า, ลบรอย Remark รอผลตรวจรถยนต์}
            var (taskDesc, round) = d.Task switch
            {
                "1" => ("ติดตามนัดหมายลูกค้า", "1"),
                "2" => ("ส่ง SV ออกตรวจสอบ", "1"),
                "3" => ("ติดตาม SV", "1"),
                "4" => ("รอผลตรวจรถยนต์", "1"),
                "5" => (d.RemarkCode == "01" ? "ลบรอย Remark ติดตามนัดหมายลูกค้า" : "ลบรอย Remark รอผลตรวจรถยนต์", "2"),
                "6" => ("ส่ง SV ออกตรวจสอบ", "2"),
                "7" => ("ติดตาม SV", "2"),
                "8" => ("รอผลตรวจรถยนต์", "2"),
                _ => (null, null)
            };

            if (taskDesc == null) // ถ้าเลข Task ไม่ถูกต้อง
                return new ApiResponse<IEnumerable<InspectionTransaction>> { Message = "Invalid Task Number", Status = StatusConstant.ErrorCode };

            /// Task 4,8 => validate report = Y
            if(d.Task == "4" || d.Task == "8")
            {
                // codition
            }
            else
            {
                d.ResultReport = null;
                d.VerifyResultDatetime = null;
                d.MileNumber = null;
                d.InspectionDatetime = null;
                d.CarInspectionResult = null;
                d.CarType = null;
                d.Spare = null;
                d.Gas = null;
                d.GasNumber = null;
                d.GasType = null;
                d.GasPrice = null;
                d.ModifyVehicle = null;
                d.ModifyVehicleList = [];
            }

            // Action => {save 001=In Progress}, {002=Complete}
            var (taskCompleteStatus, taskCompleteDate, taskCompleteBy) = d.Action switch
            {
                "save" => ("001", (string?)null, (string?)null),
                "complete" => ("002", DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), d.TaskCreateBy),
                _ => (null, null, null)
            };

            var tasks = d.JobList.Select(jobId =>
            {
                var newTask = d.Clone();
                newTask.JobId = jobId;
                newTask.JobList = [];
                newTask.TaskDesc = taskDesc;
                newTask.TaskCompleteStatus = taskCompleteStatus;
                newTask.TaskCompleteDate = taskCompleteDate;
                newTask.TaskCompleteBy = taskCompleteBy;
                newTask.Round = round;
                newTask.Action = null;
                _logger.LogInformation("newTask : {Json}", JsonSerializer.Serialize(newTask, _jsonOptions));
                return newTask;
            }).ToList();

            _logger.LogInformation("Updating {Count} tasks for Task No: {TaskNo}", tasks.Count, d.Task);

            var resultTask = d.Task switch
            {
                "1" or "5" => _repository.UpdateTask001(tasks),
                "2" or "6" => _repository.UpdateTask002(tasks),
                "3" or "7" => _repository.UpdateTask003(tasks),
                "4" or "8" => _repository.UpdateTask004(tasks),
                _ => Task.FromResult(new List<InspectionTransaction>())
            };

            var result = await resultTask;

            if (result == null || result.Count == 0) return new ApiResponse<IEnumerable<InspectionTransaction>>() { Message = StatusConstant.ErrorMessage, Status = StatusConstant.ErrorCode };
            return new ApiResponse<IEnumerable<InspectionTransaction>>() { Data = result };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "เกิดข้อผิดพลาดในการ UpdateTaskAsync: {Message}", ex.Message);
            return new ApiResponse<IEnumerable<InspectionTransaction>>()
            {
                Message = StatusConstant.ErrorMessage,
                Status = StatusConstant.ErrorCode
            };
        }
    }

    public async Task<string> GetSeq()
    {
        return await _repository.GetSeq();
    }


}