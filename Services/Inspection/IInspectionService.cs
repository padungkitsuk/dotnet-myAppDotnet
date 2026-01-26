using MyBackend.Models;
using MyBackend.Models.Inspection;

namespace MyBackend.Services.Inspection;

public interface IInspectionService {
    Task<IEnumerable<InspectionTransaction>> GetAllAsync();
    Task<InspectionDetail> GetByIdAsync(int id);
}