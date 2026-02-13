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
    private readonly IMasterDataRepository _master;

    public InspectionService(IInspectionRepository repository, ILogger<InspectionService> logger, IMapper mapper, IMasterDataRepository master)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
        _master = master;
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

    public async Task<ApiResponse<IEnumerable<VehicleInfo>>> CreateAsync(InspectionRequest d, string userId)
    {
        // 0101 = งานเข้าใหม่
        // log history
        d.StatusCode = "0101"; 
        d.Status = "งานเข้าใหม่"; 
        // inspection
        d.JobStatus = "0101"; 
        d.JobCreateBy = userId;
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

            // check C=Corporate customers
            if (d.CustomerInfo?.CustomerType == "C") d.CustomerInfo.CustomerLastName = null;

            var masterProvince = await _master.GetProvinceList();
            var provinceDict = masterProvince.Where(p => p.Code != null).ToDictionary(p => p.Code!, p => p.Label);

            // check province not in masterProvince
            var invalidVehicles = d.VehicleInfo.Where(v => 
            {
                var provinceCode = v.CarRedLicense == "Y" ? "99" : v.CarProvince?.Replace(" ", "");
                return string.IsNullOrEmpty(provinceCode) || !provinceDict.ContainsKey(provinceCode);
            }).ToList();

            if (invalidVehicles.Count != 0)
            {
                var invalidCodes = string.Join(", ", invalidVehicles.Select(v => v.CarProvince).Distinct());
                return new ApiResponse<IEnumerable<VehicleInfo>> 
                { 
                    Message = $"Invalid province code found: {invalidCodes}", 
                    Status = StatusConstant.ErrorCode,
                    Data = invalidVehicles
                };
            }

            // clean space
            d.VehicleInfo.ForEach(v =>
            {
                v.CarPlateNo = v.CarPlateNo?.Replace(" ", "");
                v.CarProvince = v.CarProvince?.Replace(" ", "");
                if (v.CarRedLicense == "Y")
                {
                    v.CarPlateNo = "ใหม่";
                    v.CarProvince = "99";
                }
                v.CarProvinceDesc = provinceDict[v.CarProvince!];
            });

            // check duplicate from request
            var duplicateVehicles = d.VehicleInfo
                .GroupBy(v => new { v.CarPlateNo, v.CarProvince })
                .Where(g => g.Count() > 1) // where duplicate
                .SelectMany(g => g)        // return List
                .ToList();

            // (Count > 0) Return ข้อมูลที่ซ้ำ
            if (duplicateVehicles.Count != 0) 
            {
                return new ApiResponse<IEnumerable<VehicleInfo>> 
                { 
                    Message = "Duplicate license plate and province found.", 
                    Status = StatusConstant.DuplicateCode, 
                    Data = duplicateVehicles 
                };
            }

            // prepare data check car plate no
            var plates = (d.FleetStatus == "Y" ? d.VehicleInfo.Select(v => v.CarPlateNo) : d.VehicleInfo.Take(1).Select(v => v.CarPlateNo))
            .Where(p => !string.IsNullOrEmpty(p))
            .ToImmutableArray();

            var carsHistory = (await _repository.GetCarInfo(plates)).ToList();
            _logger.LogInformation("carsHistory: {Json}", JsonSerializer.Serialize(carsHistory, _jsonOptions));

            // check duplicate from database
            if (carsHistory.Count != 0)
            {
                var duplicates = carsHistory.Where(h =>
                    d.VehicleInfo.Any(v => v.CarPlateNo == h.CarPlateNo && v.CarProvince == h.CarProvince)
                ).ToList();

                duplicates.ForEach(dup => { dup.CarProvinceDesc = provinceDict[dup.CarProvince!]; });

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
                req.CarProvinceDesc = v.CarProvinceDesc;
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
                CarProvinceDesc = s.CarProvinceDesc,
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
        try
        {
            var resultData = await _repository.GetTaskDetailAsync(jobId);
            if (resultData == null)
                return new ApiResponse<IEnumerable<InspectionTaskDetail>> { Message = "Data not found", Status = StatusConstant.NotFoundCode };

            // convert to List use .Concat / .AddRange
            var resultList = resultData.ToList();

            var targetTasks = new HashSet<string> { "1", "2", "3"
            //, "4", "5", "6", "7" 
            };
            var targetTaskCompleteStatus = new HashSet<string> { "002" }; //002 = conplete
            var targetTaskStatus = new HashSet<string> { "0528" }; // 0528 = ลบรอย Remark

            var generatedTasks = new List<InspectionTaskDetail>();

            if (resultList.Count == 0)
            {
                var nextTaskNo = "1";
                var nextTaskDesc = GetTaskMetadata(nextTaskNo, null).taskDesc;
                generatedTasks.Add(new InspectionTaskDetail
                {
                    Task = nextTaskNo,
                    TaskDesc = nextTaskDesc,
                    TaskCompleteStatus = "001",
                    TaskCompleteStatusDesc = "In Progress"
                });
            }
            else
            {
                var lastCompletedTask = resultList
                .Where(c => !string.IsNullOrEmpty(c.Task)
                        && targetTasks.Contains(c.Task)
                        )
                .OrderByDescending(c => Convert.ToInt32(c.Task)) // last task
                .FirstOrDefault();

                if (lastCompletedTask != null)
                {
                    if (!string.IsNullOrEmpty(lastCompletedTask.TaskCompleteStatus)
                    && targetTaskCompleteStatus.Contains(lastCompletedTask.TaskCompleteStatus))
                    {
                        if (int.TryParse(lastCompletedTask.Task, out int currentTaskNum))
                        {
                            string nextTaskNo = (currentTaskNum + 1).ToString();
                            // เช็คว่าในระบบมี Task ถัดไปอยู่แล้วหรือยัง (ถ้ายังไม่มีค่อยสร้าง)
                            bool alreadyExists = resultList.Any(x => x.Task == nextTaskNo);
                            if (!alreadyExists)
                            {
                                var nextTaskDesc = GetTaskMetadata(nextTaskNo, lastCompletedTask.RemarkCode).taskDesc;
                                generatedTasks.Add(new InspectionTaskDetail
                                {
                                    Task = nextTaskNo,
                                    TaskDesc = nextTaskDesc,
                                    TaskCompleteStatus = "001",
                                    TaskCompleteStatusDesc = "In Progress"
                                });
                            }
                        }
                    }
                }

                //if modifyVehicle = Y
                var allModifyItems = await _repository.GetModifyVehicle(jobId);
                foreach (var item in resultList)
                {
                    if (item.ModifyVehicle == "Y")
                    {
                        item.ModifyVehicleList = (List<InspectionModifyVehicle?>?)allModifyItems; 
                    }
                }
            }

            // final 
            var finalData = resultList.Concat(generatedTasks);

            _logger.LogInformation("Task List: {Count}, Added: {NewCount}", resultList.Count, generatedTasks.Count);

            return new ApiResponse<IEnumerable<InspectionTaskDetail>>
            {
                Data = finalData
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetTaskDetailAsync Error: {Message}", ex.Message);
            return new ApiResponse<IEnumerable<InspectionTaskDetail>>
            {
                Message = StatusConstant.ErrorMessage,
                Status = StatusConstant.ErrorCode
            };
        }
    }

    private static (string? taskCode, string? taskDesc, string? round, string? nextTask) GetTaskMetadata(string? task, string? remarkCode)
    {
        return task switch
        {
            "1" => ("0102","ติดตามนัดหมายลูกค้า", "1", "2"),
            "2" => ("0103","ส่ง SV ออกตรวจสอบ", "1", "3"),
            "3" => ("0104","ติดตาม SV", "1", "4"),
            "4" => ("0105","รอผลตรวจรถยนต์", "1", null),

            //Task 5 => check RemarkCode {01,02} => {ลบรอย Remark ติดตามนัดหมายลูกค้า, ลบรอย Remark รอผลตรวจรถยนต์}
            //"5" => (remarkCode == "01" ? "ลบรอย Remark ติดตามนัดหมายลูกค้า" : "ลบรอย Remark รอผลตรวจรถยนต์", "2", "6"),
            //"6" => ("ส่ง SV ออกตรวจสอบ", "2", "7"),
            //"7" => ("ติดตาม SV", "2", "8"),
            //"8" => ("รอผลตรวจรถยนต์", "2", null),
            _ => (null, null, null, null)
        };
    }

    public async Task<ApiResponse<IEnumerable<InspectionTaskResponse>>> UpdateTaskAsync(InspectionTaskRequest d)
    {
        try
        {
            if (d.JobList == null || d.JobList.Count == 0)
                return new ApiResponse<IEnumerable<InspectionTaskResponse>> { Message = "No jobs to update", Status = StatusConstant.NotFoundCode };

            // 
            var (taskCode, taskDesc, round, next) = GetTaskMetadata(d.Task, d.RemarkCode);

            if (taskDesc == null) // ถ้าเลข Task ไม่ถูกต้อง
                return new ApiResponse<IEnumerable<InspectionTaskResponse>> { Message = "Invalid Task Number", Status = StatusConstant.ErrorCode };

            /// Task 4,8 => validate report = Y
            if (d.Task == "4" || d.Task == "8")
            {
                // codition
            }
            else
            {
                ClearReportData(d);
            }

            // Action => {save 001=In Progress}, {002=Complete}
            var (taskCompleteStatus, taskCompleteDate, taskCompleteBy, nextTask, nextTaskDesc) = d.Action switch
            {
                "save" => ("001", (string?)null, (string?)null, (string?)null, (string?)null),
                "complete" => ("002",
                DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                d.TaskCreateBy,
                next,
                GetTaskMetadata(next, d.RemarkCode).taskDesc),
                _ => (null, null, null, null, null)
            };

            var tasks = d.JobList.Select(jobId =>
            {
                var newTask = d.Clone();
                newTask.JobId = jobId;
                newTask.JobList = [];
                newTask.TaskCode = taskCode;
                newTask.TaskDesc = taskDesc;
                newTask.TaskCompleteStatus = taskCompleteStatus;
                newTask.TaskCompleteDate = newTask.AppointmentDatetime;
                newTask.TaskCompleteBy = taskCompleteBy;
                newTask.Round = round;
                newTask.Action = null;
                _logger.LogInformation("newTask : {Json}", JsonSerializer.Serialize(newTask, _jsonOptions));
                return newTask;
            }).ToList();

            _logger.LogInformation("Updating {Count} tasks for Task: {Task}", tasks.Count, d.Task);

            var resultTask = d.Task switch
            {
                "1" or "5" => _repository.UpdateTask001(tasks),
                "2" or "6" => _repository.UpdateTask002(tasks),
                "3" or "7" => _repository.UpdateTask003(tasks),
                "4" or "8" => _repository.UpdateTask004(tasks),
                _ => Task.FromResult(new List<InspectionTaskResponse>())
            };

            var result = await resultTask;

            if (result == null || result.Count == 0) return new ApiResponse<IEnumerable<InspectionTaskResponse>>() { Message = StatusConstant.ErrorMessage, Status = StatusConstant.ErrorCode };

            foreach (var item in result)
            {
                item.NextTask = nextTask;
                item.NextTaskDesc = nextTaskDesc;
            }

            return new ApiResponse<IEnumerable<InspectionTaskResponse>>() { Data = result };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "เกิดข้อผิดพลาดในการ UpdateTaskAsync: {Message}", ex.Message);
            return new ApiResponse<IEnumerable<InspectionTaskResponse>>()
            {
                Message = StatusConstant.ErrorMessage,
                Status = StatusConstant.ErrorCode
            };
        }
    }

    private static void ClearReportData(InspectionTaskRequest d)
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

    public async Task<string> GetSeq()
    {
        return await _repository.GetSeq();
    }

    public async Task<ApiResponse<IEnumerable<InspectionTransaction>>> AssignJob(InspectionRequestJobId d, string userId)
    {
        try
        {
            var result = await _repository.AssignJob(d.JobId, userId);

            return new ApiResponse<IEnumerable<InspectionTransaction>>() { Data = result };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "เกิดข้อผิดพลาดในการ AssignJob: {Message}", ex.Message);
            return new ApiResponse<IEnumerable<InspectionTransaction>>()
            {
                Message = StatusConstant.ErrorMessage,
                Status = StatusConstant.ErrorCode
            };
        }
    }
}