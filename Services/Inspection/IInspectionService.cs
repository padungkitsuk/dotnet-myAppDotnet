using MyBackend.Models;
using MyBackend.Models.Inspection;

namespace MyBackend.Services.Inspection;

public interface IInspectionService {
    Task<InspectionDetail> CreateAsync(InspectionDetail d);
    Task<IEnumerable<InspectionTransaction>> GetAllAsync();
    Task<InspectionDetail> GetByIdAsync(int id);
    Task<bool> UpdateAsync(InspectionDetail d);
}