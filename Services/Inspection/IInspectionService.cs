using MyBackend.Models.Api;
using MyBackend.Models.Inspection;
using MyBackend.Models.Paged;
using MyBackend.Models.Vehicle;

namespace MyBackend.Services.Inspection;

public interface IInspectionService {
    Task<PagedResult<InspectionTransaction>> GetPagedAsync(int pageNo, int pageSize);
    Task<InspectionTransaction?> GetByIdAsync(string id);
    Task<ApiResponse<IEnumerable<VehicleInfo>>> CreateAsync(InspectionRequest d);
    Task<bool> UpdateAsync(InspectionDetail d);
    Task<string> GetSeq();
}