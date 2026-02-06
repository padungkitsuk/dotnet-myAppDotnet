using MyBackend.Models.Master;

namespace MyBackend.Repositories.Inspection;

public interface IMasterDataRepository {
    Task<List<MasterDropdown>> GetSourceList();
    Task<List<MasterDropdown>> GetBUList();
    Task<List<MasterDropdown>> GetReasonServeyList();
    Task<List<MasterDropdown>> GetProvinceList();
    Task<List<MasterDropdown>> GetCarBrandList();
    Task<List<MasterDropdown>> GetCarModelList(string carBrand);
    Task<List<MasterDropdown>> GetJobStateList(string groupCode);
    Task<List<MasterDropdown>> GetRemarkMethodList();
}