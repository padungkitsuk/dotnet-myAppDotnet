

using MyBackend.Models.Master;
using MyBackend.Models.Utils.Api;

namespace MyBackend.Services.Master;

public interface IMasterService {
    Task<ApiResponse<IEnumerable<MasterDropdown>>> GetSourceList();
    Task<ApiResponse<IEnumerable<MasterDropdown>>> GetAgentList();
    Task<ApiResponse<IEnumerable<MasterDropdown>>> GetBUList();
    Task<ApiResponse<IEnumerable<MasterDropdown>>> GetReasonServeyList();
    Task<ApiResponse<IEnumerable<MasterDropdown>>> GetProvinceList();
    Task<ApiResponse<IEnumerable<MasterDropdown>>> GetCarBrandList();
    Task<ApiResponse<IEnumerable<MasterDropdown>>> GetCarModelList(string carBrand);
    Task<ApiResponse<IEnumerable<MasterDropdown>>> GetJobStateList(string groupCode);
    Task<ApiResponse<IEnumerable<MasterDropdown>>> GetRemarkMethodList();
    Task<ApiResponse<IEnumerable<MasterDropdown>>> GetRegionList();
    Task<ApiResponse<IEnumerable<MasterDropdown>>> GetProvinceCodeList();
    Task<ApiResponse<IEnumerable<MasterDropdown>>> GetDistrictList();
    Task<ApiResponse<IEnumerable<MasterDropdownPrice>>> GetServeyServiceList(string regionCode, string provinceCode, string districtCode);
}