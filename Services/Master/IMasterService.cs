

using MyBackend.Models.Master;
using MyBackend.Models.Utils.Api;

namespace MyBackend.Services.Master;

public interface IMasterService {
    Task<ApiResponse<IEnumerable<MasterDropdown>>> GetSourceList();
    Task<ApiResponse<IEnumerable<MasterDropdown>>> GetBUList();
    Task<ApiResponse<IEnumerable<MasterDropdown>>> GetReasonServeyList();
    Task<ApiResponse<IEnumerable<MasterDropdown>>> GetProvinceList();
    Task<ApiResponse<IEnumerable<MasterDropdown>>> GetCarBrandList();
    Task<ApiResponse<IEnumerable<MasterDropdown>>> GetCarModelList(string carBrand);
    Task<ApiResponse<IEnumerable<MasterDropdown>>> GetJobStateList(string groupCode);
}