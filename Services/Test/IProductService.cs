using MyBackend.Models.Paged;
using MyBackend.Models.Test;

namespace MyBackend.Services.Test;

public interface IProductService {
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task<PagedResult<Product>> GetPagedAsync(int pageNo, int pageSize);
}