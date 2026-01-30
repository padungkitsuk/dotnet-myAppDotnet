using Microsoft.AspNetCore.Mvc;
using MyBackend.Models.Api;
using MyBackend.Models.Inspection;
using MyBackend.Models.Paged;
using MyBackend.Models.Vehicle;
using MyBackend.Repositories.Sequence;
using MyBackend.Services.Inspection;
using MyBackend.Utils.Constants;

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
    public async Task<ActionResult<PagedResult<IEnumerable<InspectionTransaction>>>> GetPaged([FromQuery] int pageNo = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNo < 1) pageNo = 1;
        if (pageSize < 1) pageSize = 10;

        var result = await _inspectService.GetPagedAsync(pageNo, pageSize);
        if (result.Data.Count() == 0)
            return NotFound(new PagedResult<IEnumerable<InspectionTransaction>> { Message = StatusConstant.NotFoundMessage, Status = StatusConstant.NotFoundCode });
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<InspectionTransaction>>> GetById(string id)
    {
        var result = await _inspectService.GetByIdAsync(id);
        if (result == null)
            return NotFound(new ApiResponse<InspectionTransaction> { Message = StatusConstant.NotFoundMessage, Status = StatusConstant.NotFoundCode });

        return Ok(new ApiResponse<InspectionTransaction> { Data = result });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<IEnumerable<VehicleInfo>>>> CreateAsync(InspectionRequest d)
    {
        var result = await _inspectService.CreateAsync(d);
        if (result == null)
            return BadRequest(new ApiResponse<string> { Message = StatusConstant.BadRequestMessage, Status = StatusConstant.BadRequestCode });

        return result.Status switch
        {
            "00" => Ok(new ApiResponse<IEnumerable<VehicleInfo>> { Data = result.Data }),

            "01" => Conflict(new ApiResponse<IEnumerable<VehicleInfo>>
            {
                Message = StatusConstant.DuplicateMessage,
                Status = StatusConstant.DuplicateCode,
                Data = result.Data
            }),

            "99" => StatusCode(500, new ApiResponse<IEnumerable<VehicleInfo>>
            {
                Message = StatusConstant.ErrorMessage,
                Status = StatusConstant.ErrorCode,
                Data = result.Data
            }),

            _ => BadRequest(new ApiResponse<IEnumerable<VehicleInfo>>
            {
                Message = "Unknown Error",
                Status = result.Status,
                Data = result.Data
            })
        };
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateAsync(InspectionDetail d)
    {
        if (d.jobId == "") return BadRequest(new ApiResponse<bool> { Message = StatusConstant.BadRequestMessage, Status = StatusConstant.BadRequestCode });
        var success = await _inspectService.UpdateAsync(d);
        if (!success) return NotFound(new ApiResponse<bool> { Message = StatusConstant.NotFoundMessage, Status = StatusConstant.NotFoundCode });
        return Ok(new ApiResponse<bool> { Data = success });
    }

    [HttpGet("seq")]
    public async Task<ActionResult<ApiResponse<string>>> GetSeq()
    {
        var result = await _inspectService.GetSeq();
        return Ok(new ApiResponse<string> { Data = result });
    }

}