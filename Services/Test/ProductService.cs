using Microsoft.EntityFrameworkCore;
// using MyBackend.Data;
using System.Text.Json;
using System.Text.Encodings.Web;
using MyBackend.Repositories.Test;
using MyBackend.Models.Test;
using MyBackend.Models.Paged;

namespace MyBackend.Services.Test;

public class ProductService : IProductService
{
    private readonly ILogger<ProductService> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new() {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true
    };

    private readonly IProductRepository _repository;


    public ProductService(
        ILogger<ProductService> logger, 
        //AppDbContext context, 
        IProductRepository repository
    ){
        _logger = logger;
        //_context = context;
        _repository = repository;
    }

    public async Task<IEnumerable<Product>> GetAllAsync(){
        //return await _context.products.ToListAsync();
        return await _repository.GetAllAsync();
    }

    public async Task<Product?> GetByIdAsync(int id){
        //return await _context.products.FindAsync(id);
        var data = await _repository.GetByIdAsync(id);
        _logger.LogInformation("{Json} เวลา {Time}", JsonSerializer.Serialize(data), DateTime.Now);
        return data;
    }

    public async Task<Product> CreateAsync(Product product){
        if (product.price < 0) throw new Exception("Price cannot be negative");

        //_context.products.Add(product);
        //await _context.SaveChangesAsync();
        int newId = await _repository.CreateAsync(product);
        product.id = newId;
        _logger.LogInformation("สร้างสินค้าสำเร็จ: {Json} เวลา {Time}", JsonSerializer.Serialize(product, _jsonOptions), DateTime.Now);
        return product;
    }

    public async Task<bool> UpdateAsync(Product product){
        //var existingProduct = await _context.products.FindAsync(product.id);
        //if (existingProduct == null) return false;
        //existingProduct.name = product.name;
        //existingProduct.price = product.price;
        //await _context.SaveChangesAsync();
        return await _repository.UpdateAsync(product);
    }

    public async Task<PagedResult<Product>> GetPagedAsync(int pageNo, int pageSize)
    {
        return await _repository.GetPagedAsync(pageNo, pageSize);
    }
}