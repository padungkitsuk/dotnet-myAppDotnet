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
    Task<IEnumerable<InspectionTaskDetail>> GetTaskDetailAsync(string jobId);
    Task<List<InspectionTransaction>> UpdateTask001(List<InspectionTaskRequest> tasks);
    Task<List<InspectionTransaction>> UpdateTask002(List<InspectionTaskRequest> tasks);
    Task<List<InspectionTransaction>> UpdateTask003(List<InspectionTaskRequest> tasks);
    Task<List<InspectionTransaction>> UpdateTask004(List<InspectionTaskRequest> tasks);
    Task<List<VehicleInfo>> GetCarInfo(IEnumerable<string> carPlateNos);
    Task<List<JobList>> GetJobListInfo(string fleetId);
    Task<string> GetSeq();
    
}