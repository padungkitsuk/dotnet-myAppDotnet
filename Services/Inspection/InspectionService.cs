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

    public async Task<PagedResult<InspectionTransaction>> GetPagedAsync(int pageNo, int pageSize)
    {
        return await _repository.GetPagedAsync(pageNo, pageSize);
    }

    public async Task<InspectionTransaction?> GetByIdAsync(string id)
    {
        try
        {
            return await _repository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"เกิดข้อผิดพลาด: {ex.Message}");
            return new InspectionTransaction();
        }
    }

    public async Task<ApiResponse<IEnumerable<VehicleInfo>>> CreateAsync(InspectionRequest d)
    {
        _logger.LogInformation("Inspect req: {Json}", JsonSerializer.Serialize(d, _jsonOptions));
        var result = new ApiResponse<IEnumerable<VehicleInfo>>();
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

            var plates = (d.FleetStatus == "Y"
            ? d.VehicleInfo.Select(v => v.CarPlateNo)
            : d.VehicleInfo.Take(1).Select(v => v.CarPlateNo)
            ).Where(p => !string.IsNullOrEmpty(p)).ToList();


            var carsHistory = (await _repository.GetCarInfo(plates)).ToList();
            _logger.LogInformation("carsHistory: {Json}", JsonSerializer.Serialize(carsHistory, _jsonOptions));

            if (carsHistory.Any())
            {
                // check Duplicate ใช้ LINQ Join/Any // เช็คว่ามีคันไหนที่ทั้ง ทะเบียน และ จังหวัด ตรงกับที่ส่งมาหรือไม่
                var duplicates = carsHistory.Where(h =>
                    d.VehicleInfo.Any(v => v.CarPlateNo == h.CarPlateNo && v.CarProvince == h.CarProvince)
                ).ToList();

                if (duplicates.Any())
                {
                    return new ApiResponse<IEnumerable<VehicleInfo>>
                    {
                        Message = StatusConstant.DuplicateMessage,
                        Status = StatusConstant.DuplicateCode,
                        Data = duplicates
                    };
                }
            }




            //var request = _mapper.Map<InspectionTransaction>(d);
            // if (d != null && d.VehicleInfo != null && d.VehicleInfo.Count >= 1)
            // {
            //     request.CarType = d.VehicleInfo[0].CarType;
            //     request.CarRedLicense = d.VehicleInfo[0].CarRedLicense;
            //     request.CarPlateNo = d.VehicleInfo[0].CarPlateNo;
            //     request.CarProvince = d.VehicleInfo[0].CarProvince;
            //     request.CarBrand = d.VehicleInfo[0].CarBrand;
            //     request.CarModel = d.VehicleInfo[0].CarModel;
            //     request.CarSubModel = d.VehicleInfo[0].CarSubModel;
            //     request.ChassisNumber = d.VehicleInfo[0].ChassisNumber;
            // }

            //result = await _repository.CreateAsync(request);
            return new ApiResponse<IEnumerable<VehicleInfo>> { Status = StatusConstant.SuccessCode };
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

    public async Task<bool> UpdateAsync(InspectionDetail d)
    {
        bool isSuccess = false;
        try
        {
            string jsonFileContent = await File.ReadAllTextAsync(filePath);
            if (d == null) return false;
            d.createDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            isSuccess = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "เกิดข้อผิดพลาดในการ Update: {Message}", ex.Message);
            isSuccess = false;
        }
        return isSuccess;
    }

    public async Task<string> GetSeq()
    {
        return await _repository.GetSeq();
    }
}