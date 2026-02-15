using MyBackend.Models.Utils.Api;
using MyBackend.Models.Inspection;
using MyBackend.Models.Utils.Paged;
using MyBackend.Models.Vehicle;

namespace MyBackend.Services.Inspection;

public interface IInspectionService {
    Task<PagedResult<IEnumerable<InspectionTransaction>>> GetPagedAsync(RequestDataInspection d);
    Task<ApiResponse<InspectionTransactionDetail>> GetByIdAsync(RequestDataInspection d);
    Task<ApiResponse<IEnumerable<VehicleInfo>>> CreateAsync(InspectionRequest d, string userId);
    Task<ApiResponse<IEnumerable<InspectionTransactionHistory>>> GetJobHistory(string jobId);
    Task<ApiResponse<IEnumerable<InspectionTransactionHistory>>> JobHistoryDelete(InspectionRequestJobId d);
    Task<ApiResponse<IEnumerable<InspectionTaskDetail>>> GetTaskDetailAsync(string jobId);
    Task<ApiResponse<IEnumerable<InspectionTaskResponse>>> UpdateTaskAsync(InspectionTaskRequest d);
    Task<ApiResponse<IEnumerable<InspectionTransaction>>> AssignJob(InspectionRequestJobId d, string userId);
    //Task<string> GetSeq();
}