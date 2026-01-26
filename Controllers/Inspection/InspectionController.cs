using Microsoft.AspNetCore.Mvc;
using MyBackend.Models;
using MyBackend.Models.Inspection;
using MyBackend.Services;
using MyBackend.Services.Inspection;

namespace MyBackend.Controllers.Inspect;

[Route("api/[controller]")]
[ApiController]
public class InspectionController : ControllerBase
{
    private readonly IInspectionService _inspectService;

    public InspectionController(IInspectionService inspectService)
    {
        _inspectService = inspectService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<InspectionTransaction>>>> Get()
    {
        var result = await _inspectService.GetAllAsync();
        if (result.Count() == 0)
            return NotFound(new ApiResponse<InspectionTransaction> { Message = "Data not found.", Status = "01" });

        return Ok(new ApiResponse<IEnumerable<InspectionTransaction>> { Data = result });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<InspectionDetail>>> GetById(int id)
    {
        var result = await _inspectService.GetByIdAsync(id);
        if (result == null)
            return NotFound(new ApiResponse<InspectionDetail> { Message = "Data not found.", Status = "01" });

        return Ok(new ApiResponse<InspectionDetail> { Data = result });
    }

    
}