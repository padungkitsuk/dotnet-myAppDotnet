using MyBackend.Models.Utils.Api;
using MyBackend.Models.Inspection;
using MyBackend.Models.Utils.Paged;
using MyBackend.Models.Vehicle;

namespace MyBackend.Services.Inspection;

public interface IInspectionService {
    Task<PagedResult<IEnumerable<InspectionTransaction>>> GetPagedAsync(RequestDataInspection d);
    Task<ApiResponse<InspectionTransactionDetail>> GetByIdAsync(RequestDataInspection d);
    Task<ApiResponse<IEnumerable<VehicleInfo>>> CreateAsync(InspectionRequest d);
    Task<ApiResponse<IEnumerable<InspectionTaskDetail>>> GetTaskDetailAsync(string jobId);
    Task<ApiResponse<IEnumerable<InspectionTransaction>>> UpdateTaskAsync(InspectionTaskRequest d);
    Task<string> GetSeq();
}