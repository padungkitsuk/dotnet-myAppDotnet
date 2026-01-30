using MyBackend.Models.Api;
using MyBackend.Models.Inspection;
using MyBackend.Models.Paged;
using MyBackend.Models.Vehicle;

namespace MyBackend.Services.Inspection;

public interface IInspectionService {
    Task<PagedResult<IEnumerable<InspectionTransaction>>> GetPagedAsync(RequestDataInspection d);
    Task<ApiResponse<InspectionTransaction>> GetByIdAsync(RequestDataInspection d);
    Task<ApiResponse<IEnumerable<VehicleInfo>>> CreateAsync(InspectionRequest d);
    Task<ApiResponse<InspectionTransaction>> UpdateAsync(InspectionTransaction d);
    Task<string> GetSeq();
}