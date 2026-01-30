using MyBackend.Models.Inspection;
using MyBackend.Models.Paged;
using MyBackend.Models.Vehicle;

namespace MyBackend.Repositories.Inspection;

public interface IInspectionRepository {
    Task<PagedResult<IEnumerable<InspectionTransaction>>> GetPagedAsync(RequestDataInspection d);
    Task<InspectionTransaction?> GetByIdAsync(RequestDataInspection d);
    Task<List<InspectionTransaction>> CreateAsync(List<InspectionTransaction> d);
    Task<bool> UpdateAsync(InspectionTransaction d);
    Task<List<VehicleInfo>> GetCarInfo(IEnumerable<string> carPlateNos);
    Task<string> GetSeq();
    
}