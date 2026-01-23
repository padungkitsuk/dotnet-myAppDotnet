using MyBackend.Models;

namespace MyBackend.Services;

public interface IProductService {
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<bool> UpdateAsync(Product product);
}