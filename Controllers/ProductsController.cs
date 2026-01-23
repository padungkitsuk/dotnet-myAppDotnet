using Microsoft.AspNetCore.Mvc;
using MyBackend.Models;
using MyBackend.Services;

namespace MyBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<Product>>>> Get()
    {
        var result = await _productService.GetAllAsync();
        if (result.Count() == 0)
            return NotFound(new ApiResponse<Product> { Message = "Data not found.", Status = "01" });

        return Ok(new ApiResponse<IEnumerable<Product>> { Data = result });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Product>>> GetById(int id)
    {
        var result = await _productService.GetByIdAsync(id);
        if (result == null)
            return NotFound(new ApiResponse<Product> { Message = "Data not found.", Status = "01" });

        return Ok(new ApiResponse<Product> { Data = result });
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Post(Product product)
    {
        try
        {
            var result = await _productService.CreateAsync(product);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT: api/Products
    [HttpPut]
    public async Task<IActionResult> Put(Product product)
    {
        if (product.id == 0) return BadRequest("ID is null");
        var success = await _productService.UpdateAsync(product);
        if (!success) return NotFound($"Product with ID {product.id} not found");
        return Ok();
    }
}