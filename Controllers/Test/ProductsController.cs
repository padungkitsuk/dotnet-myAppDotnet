using Microsoft.AspNetCore.Mvc;
using MyBackend.Models.Api;
using MyBackend.Models.Paged;
using MyBackend.Models.Test;
using MyBackend.Services.Test;

namespace MyBackend.Controllers.Test;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // [HttpGet]
    // public async Task<ActionResult<ApiResponse<IEnumerable<Product>>>> Get()
    // {
    //     var result = await _productService.GetAllAsync();
    //     if (result.Count() == 0)
    //         return NotFound(new ApiResponse<Product> { Message = "Data not found.", Status = "01" });

    //     return Ok(new ApiResponse<IEnumerable<Product>> { Data = result });
    // }

    // [HttpGet]
    // public async Task<ActionResult<PagedResult<IEnumerable<Product>>>> GetAll([FromQuery] int pageNo = 1, [FromQuery] int pageSize = 10)
    // {
    //     // ป้องกันกรณีส่งค่าติดลบมา
    //     if (pageNo < 1) pageNo = 1;
    //     if (pageSize < 1) pageSize = 10;

    //     var result = await _productService.GetPagedAsync(pageNo, pageSize);
    //     //if (result.Data. == 0) return NotFound(new PagedResult<IEnumerable<Product>> { Message = "Data not found.", Status = "01" });
        
    //     return Ok(result);
    // }

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
        if (product.Id == 0) return BadRequest("ID is null");
        var success = await _productService.UpdateAsync(product);
        if (!success) return NotFound($"Product with ID {product.Id} not found");
        return Ok();
    }
}