using MyBackend.Models.Inspection;
using MyBackend.Models.Utils.Paged;
using MyBackend.Models.Vehicle;

namespace MyBackend.Repositories.Inspection;

public interface IInspectionRepository {
    Task<PagedResult<IEnumerable<InspectionTransaction>>> GetPagedAsync(RequestDataInspection d);
    Task<InspectionTransaction> GetByIdAsync(RequestDataInspection d);
    Task<List<InspectionTransaction>> CreateAsync(List<InspectionTransaction> d, string? fleetStatus);
    //Task<bool> UpdateJobHistory(InspectionTransactionHistory d);
    Task<IEnumerable<InspectionTransactionHistory>> GetJobHistory(string jobId);
    Task<IEnumerable<InspectionTransactionHistory>> JobHistoryDelete(InspectionRequestJobId d);
    Task<IEnumerable<InspectionTaskDetail>> GetDetailTaskAsync(string jobId);
    Task<List<InspectionTaskResponse>> UpdateTask001(List<InspectionTaskRequest> tasks);
    Task<List<InspectionTaskResponse>> UpdateTask002(List<InspectionTaskRequest> tasks);
    Task<List<InspectionTaskResponse>> UpdateTask003(List<InspectionTaskRequest> tasks);
    Task<List<InspectionTaskResponse>> UpdateTask004(List<InspectionTaskRequest> tasks);
    Task<List<VehicleInfo>> GetCarInfo(IEnumerable<string> carPlateNos);
    Task<List<JobList>> GetJobListInfo(string fleetId);
    Task<List<JobList>> GetJobListInfoById(string jobId);
    Task<string> GetSeq();
    Task<List<InspectionTransaction>> AssignJob(string jobId, string userId);
    Task<IEnumerable<InspectionModifyVehicle>> GetModifyVehicle(string jobId);
}