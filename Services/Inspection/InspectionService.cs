using System.Text.Json;
using System.Text.Encodings.Web;
using MyBackend.Models.Inspection;
using MyBackend.Repositories.Inspection;
using MyBackend.Models.Paged;
using AutoMapper;
using MyBackend.Models.Api;
using MyBackend.Models.Vehicle;
using MyBackend.Utils.Constants;
using System.Text.Json.Serialization;
using System.Collections.Immutable;

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

    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "data.json");

    public async Task<PagedResult<IEnumerable<InspectionTransaction>>> GetPagedAsync(RequestDataInspection d)
    {
        return await _repository.GetPagedAsync(d);
    }

    public async Task<ApiResponse<InspectionTransaction>?> GetByIdAsync(RequestDataInspection d)
    {
        try
        {
            if(string.IsNullOrEmpty(d.JobId)) return new ApiResponse<InspectionTransaction>() {  Message = StatusConstant.NotFoundMessage, Status = StatusConstant.NotFoundCode };
            
            var result = await _repository.GetByIdAsync(d);
            _logger.LogInformation("GetByIdAsync id: {id} res: {Json}", d.JobId, JsonSerializer.Serialize(result, _jsonOptions));
            if(result == null) return new ApiResponse<InspectionTransaction>() {  Message = StatusConstant.NotFoundMessage, Status = StatusConstant.NotFoundCode };

            return new ApiResponse<InspectionTransaction>()
            {
                Data = result
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "เกิดข้อผิดพลาดใน GetByIdAsync");
            return new ApiResponse<InspectionTransaction>()
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

            // Validation
            if (d.VehicleInfo == null || d.VehicleInfo.Count == 0)
            {
                return new ApiResponse<IEnumerable<VehicleInfo>>
                {
                    Message = StatusConstant.ErrorMessage,
                    Status = StatusConstant.ErrorCode
                };
            }

            d.VehicleInfo.ForEach(v => v.CarPlateNo = v.CarPlateNo?.Replace(" ", ""));

            var plates = (d.FleetStatus == "Y" ? d.VehicleInfo.Select(v => v.CarPlateNo) : d.VehicleInfo.Take(1).Select(v => v.CarPlateNo))
            .Where(p => !string.IsNullOrEmpty(p))
            .ToImmutableArray();


            var carsHistory = (await _repository.GetCarInfo(plates)).ToList();
            _logger.LogInformation("carsHistory: {Json}", JsonSerializer.Serialize(carsHistory, _jsonOptions));

            if (carsHistory.Any())
            {
                // check Duplicate 
                var duplicates = carsHistory.Where(h =>
                    d.VehicleInfo.Any(v => v.CarPlateNo == h.CarPlateNo && v.CarProvince == h.CarProvince)
                ).ToList();

                if (duplicates.Any())
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

            var savedData = await _repository.CreateAsync(requests);

            var result = savedData.Select(s => new VehicleInfo
            {
                CarPlateNo = s.CarPlateNo,
                CarProvince = s.CarProvince,
                JobId = s.JobId
            });

            return new ApiResponse<IEnumerable<VehicleInfo>>{ Data = result };
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

    public async Task<ApiResponse<InspectionTransaction>> UpdateAsync(InspectionTransaction d)
    {
        try
        {
            // string jsonFileContent = await File.ReadAllTextAsync(filePath);
            // if (d == null) return false;
            // d.createDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            // isSuccess = true;
            var result = await _repository.UpdateAsync(d);
            if(!result) return new ApiResponse<InspectionTransaction>(){ Message = StatusConstant.ErrorMessage, Status = StatusConstant.ErrorCode };
            return new ApiResponse<InspectionTransaction>(){};
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "เกิดข้อผิดพลาดในการ Update: {Message}", ex.Message);
            return new ApiResponse<InspectionTransaction>()
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