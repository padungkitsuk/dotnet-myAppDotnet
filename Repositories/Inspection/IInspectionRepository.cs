using MyBackend.Models.Inspection;
using MyBackend.Models.Paged;

namespace MyBackend.Repositories.Inspection;

public interface IInspectionRepository {
    Task<IEnumerable<InspectionTransaction>> GetAllAsync();
    Task<InspectionTransaction?> GetByIdAsync(String job_id);
    Task<int> CreateAsync(InspectionTransaction d);
    Task<bool> UpdateAsync(InspectionTransaction d);
    Task<PagedResult<InspectionTransaction>> GetPagedAsync(int pageNo, int pageSize);
}