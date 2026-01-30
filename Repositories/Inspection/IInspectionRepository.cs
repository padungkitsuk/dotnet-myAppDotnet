using MyBackend.Models.Inspection;
using MyBackend.Models.Paged;
using MyBackend.Models.Vehicle;

namespace MyBackend.Repositories.Inspection;

public interface IInspectionRepository {
    Task<PagedResult<InspectionTransaction>> GetPagedAsync(int pageNo, int pageSize);
    Task<InspectionTransaction?> GetByIdAsync(string job_id);
    Task<string> CreateAsync(InspectionTransaction d);
    Task<bool> UpdateAsync(InspectionTransaction d);
    Task<List<VehicleInfo>> GetCarInfo(IEnumerable<string> carPlateNos);
    Task<string> GetSeq();
    
}