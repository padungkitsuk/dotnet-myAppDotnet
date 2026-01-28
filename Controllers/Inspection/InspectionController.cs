using Microsoft.AspNetCore.Mvc;
using MyBackend.Models.Api;
using MyBackend.Models.Inspection;
using MyBackend.Repositories.Sequence;
using MyBackend.Services.Inspection;

namespace MyBackend.Controllers.Inspect;

[Route("api/[controller]")]
[ApiController]
public class InspectionController : ControllerBase
{
    private readonly IInspectionService _inspectService;
    private readonly ISequenceRepository _seq;

    public InspectionController(IInspectionService inspectService, ISequenceRepository seq)
    {
        _inspectService = inspectService;
        _seq = seq;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<InspectionTransaction>>>> Get()
    {
        var result = await _inspectService.GetAllAsync();
        if (result.Count() == 0)
            return NotFound(new ApiResponse<InspectionTransaction> { Message = "Data not found.", Status = "01" });

        return Ok(new ApiResponse<IEnumerable<InspectionTransaction>> { Data = result });
    }

    [HttpGet("seq")]
    public async Task<ActionResult<ApiResponse<string>>> GetSeq()
    {
        var result = await _seq.GetNextSequenceValue();
        return Ok(new ApiResponse<string> { Data = result });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<InspectionDetail>>> GetById(int id)
    {
        var result = await _inspectService.GetByIdAsync(id);
        if (result == null)
            return NotFound(new ApiResponse<InspectionDetail> { Message = "Data not found.", Status = "01" });

        return Ok(new ApiResponse<InspectionDetail> { Data = result });
    }

    [HttpPost]
    public async Task<ActionResult<InspectionDetail>> Post(InspectionDetail d)
    {
        try
        {
            var result = await _inspectService.CreateAsync(d);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponse<bool>>> Put(InspectionDetail d)
    {
        if (d.jobId == "") return BadRequest(new ApiResponse<bool> { Message = "Bad Request", Status = "400", Data = false });
        var success = await _inspectService.UpdateAsync(d);
        if (!success) return NotFound(new ApiResponse<bool> { Message = "Data not found.", Status = "01", Data = false });
        return Ok(new ApiResponse<bool> { Data = success });
    }


}