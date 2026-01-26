using MyBackend.Models;

namespace MyBackend.Repositories.Test;

public interface IProductRepository {
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<int> CreateAsync(Product product);
    Task<bool> UpdateAsync(Product product);
}