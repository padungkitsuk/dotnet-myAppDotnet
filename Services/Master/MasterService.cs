using System.Text.Json;
using System.Text.Encodings.Web;
using MyBackend.Models.Inspection;
using MyBackend.Repositories.Inspection;
using AutoMapper;
using MyBackend.Models.Utils.Api;
using MyBackend.Utils.Constants;
using System.Text.Json.Serialization;
using MyBackend.Models.Master;

namespace MyBackend.Services.Master;

public class MasterService : IMasterService
{
    private readonly IMasterDataRepository _repository;
    private readonly ILogger<MasterService> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    private readonly IMapper _mapper;

    public MasterService(IMasterDataRepository repository, ILogger<MasterService> logger, IMapper mapper)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<MasterDropdown>>> GetSourceList()
    {
        try
        {

            var result = await _repository.GetSourceList();
            if (result == null || result.Count == 0) return new ApiResponse<IEnumerable<MasterDropdown>>() { Message = StatusConstant.NotFoundMessage, Status = StatusConstant.NotFoundCode };
            return new ApiResponse<IEnumerable<MasterDropdown>>() { Data = result };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "เกิดข้อผิดพลาดในการ GetProvinceList: {Message}", ex.Message);
            return new ApiResponse<IEnumerable<MasterDropdown>>()
            {
                Message = StatusConstant.ErrorMessage,
                Status = StatusConstant.ErrorCode
            };
        }
    }

    public async Task<ApiResponse<IEnumerable<MasterDropdown>>> GetBUList()
    {
        try
        {

            var result = await _repository.GetBUList();
            if (result == null || result.Count == 0) return new ApiResponse<IEnumerable<MasterDropdown>>() { Message = StatusConstant.NotFoundMessage, Status = StatusConstant.NotFoundCode };
            return new ApiResponse<IEnumerable<MasterDropdown>>() { Data = result };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "เกิดข้อผิดพลาดในการ GetProvinceList: {Message}", ex.Message);
            return new ApiResponse<IEnumerable<MasterDropdown>>()
            {
                Message = StatusConstant.ErrorMessage,
                Status = StatusConstant.ErrorCode
            };
        }
    }

    public async Task<ApiResponse<IEnumerable<MasterDropdown>>> GetAgentList()
    {
        try
        {

            var result = await _repository.GetAgentList();
            if (result == null || result.Count == 0) return new ApiResponse<IEnumerable<MasterDropdown>>() { Message = StatusConstant.NotFoundMessage, Status = StatusConstant.NotFoundCode };
            return new ApiResponse<IEnumerable<MasterDropdown>>() { Data = result };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "เกิดข้อผิดพลาดในการ GetProvinceList: {Message}", ex.Message);
            return new ApiResponse<IEnumerable<MasterDropdown>>()
            {
                Message = StatusConstant.ErrorMessage,
                Status = StatusConstant.ErrorCode
            };
        }
    }

    public async Task<ApiResponse<IEnumerable<MasterDropdown>>> GetReasonServeyList()
    {
        try
        {

            var result = await _repository.GetReasonServeyList();
            if (result == null || result.Count == 0) return new ApiResponse<IEnumerable<MasterDropdown>>() { Message = StatusConstant.NotFoundMessage, Status = StatusConstant.NotFoundCode };
            return new ApiResponse<IEnumerable<MasterDropdown>>() { Data = result };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "เกิดข้อผิดพลาดในการ GetProvinceList: {Message}", ex.Message);
            return new ApiResponse<IEnumerable<MasterDropdown>>()
            {
                Message = StatusConstant.ErrorMessage,
                Status = StatusConstant.ErrorCode
            };
        }
    }


    public async Task<ApiResponse<IEnumerable<MasterDropdown>>> GetProvinceList()
    {
        try
        {

            var result = await _repository.GetProvinceList();
            if (result == null || result.Count == 0) return new ApiResponse<IEnumerable<MasterDropdown>>() { Message = StatusConstant.NotFoundMessage, Status = StatusConstant.NotFoundCode };
            return new ApiResponse<IEnumerable<MasterDropdown>>() { Data = result };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "เกิดข้อผิดพลาดในการ GetProvinceList: {Message}", ex.Message);
            return new ApiResponse<IEnumerable<MasterDropdown>>()
            {
                Message = StatusConstant.ErrorMessage,
                Status = StatusConstant.ErrorCode
            };
        }
    }

    public async Task<ApiResponse<IEnumerable<MasterDropdown>>> GetCarBrandList()
    {
        try
        {

            var result = await _repository.GetCarBrandList();
            if (result == null || result.Count == 0) return new ApiResponse<IEnumerable<MasterDropdown>>() { Message = StatusConstant.NotFoundMessage, Status = StatusConstant.NotFoundCode };
            return new ApiResponse<IEnumerable<MasterDropdown>>() { Data = result };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "เกิดข้อผิดพลาดในการ GetCarBrandList: {Message}", ex.Message);
            return new ApiResponse<IEnumerable<MasterDropdown>>()
            {
                Message = StatusConstant.ErrorMessage,
                Status = StatusConstant.ErrorCode
            };
        }
    }

    public async Task<ApiResponse<IEnumerable<MasterDropdown>>> GetCarModelList(string carBrand)
    {
        try
        {

            var result = await _repository.GetCarModelList(carBrand);
            if (result == null || result.Count == 0) return new ApiResponse<IEnumerable<MasterDropdown>>() { Message = StatusConstant.NotFoundMessage, Status = StatusConstant.NotFoundCode };
            return new ApiResponse<IEnumerable<MasterDropdown>>() { Data = result };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "เกิดข้อผิดพลาดในการ GetCarModelList: {Message}", ex.Message);
            return new ApiResponse<IEnumerable<MasterDropdown>>()
            {
                Message = StatusConstant.ErrorMessage,
                Status = StatusConstant.ErrorCode
            };
        }
    }


    public async Task<ApiResponse<IEnumerable<MasterDropdown>>> GetJobStateList(string groupCode)
    {
        try
        {

            var result = await _repository.GetJobStateList(groupCode);
            if (result == null || result.Count == 0) return new ApiResponse<IEnumerable<MasterDropdown>>() { Message = StatusConstant.NotFoundMessage, Status = StatusConstant.NotFoundCode };
            return new ApiResponse<IEnumerable<MasterDropdown>>() { Data = result };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "เกิดข้อผิดพลาดในการ GetJobStateList: {Message}", ex.Message);
            return new ApiResponse<IEnumerable<MasterDropdown>>()
            {
                Message = StatusConstant.ErrorMessage,
                Status = StatusConstant.ErrorCode
            };
        }
    }

    public async Task<ApiResponse<IEnumerable<MasterDropdown>>> GetRemarkMethodList()
    {
        try
        {

            var result = await _repository.GetRemarkMethodList();
            if (result == null || result.Count == 0) return new ApiResponse<IEnumerable<MasterDropdown>>() { Message = StatusConstant.NotFoundMessage, Status = StatusConstant.NotFoundCode };
            return new ApiResponse<IEnumerable<MasterDropdown>>() { Data = result };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "เกิดข้อผิดพลาดในการ GetRemarkMethodList: {Message}", ex.Message);
            return new ApiResponse<IEnumerable<MasterDropdown>>()
            {
                Message = StatusConstant.ErrorMessage,
                Status = StatusConstant.ErrorCode
            };
        }
    }




}