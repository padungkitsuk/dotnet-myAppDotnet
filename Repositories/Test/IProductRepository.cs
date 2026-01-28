using MyBackend.Models.Paged;
using MyBackend.Models.Test;

namespace MyBackend.Repositories.Test;

public interface IProductRepository {
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<int> CreateAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task<PagedResult<Product>> GetPagedAsync(int pageNo, int pageSize);
}