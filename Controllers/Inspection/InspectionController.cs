using Microsoft.AspNetCore.Mvc;
using MyBackend.Models.Utils.Api;
using MyBackend.Models.Utils.Paged;
using MyBackend.Models.Inspection;
using MyBackend.Models.Vehicle;
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

    [HttpPost("list")]
    public async Task<ActionResult<PagedResult<IEnumerable<InspectionTransaction>>>> GetPaged(RequestDataInspection d)
    {
        if (d.PageNo < 1) d.PageNo = 1;
        if (d.PageSize < 1) d.PageSize = 10;

        var result = await _inspectService.GetPagedAsync(d);

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new PagedResult<IEnumerable<InspectionTransaction>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("detail")]
    public async Task<ActionResult<ApiResponse<InspectionTransactionDetail>>> GetById(RequestDataInspection d)
    {
        var result = await _inspectService.GetByIdAsync(d);

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new ApiResponse<InspectionTransactionDetail>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VehicleInfo>>>> CreateAsync
    (
        [FromBody] InspectionRequest d,
        [FromHeader(Name = "userId")] string userId
    )
    {
        var result = await _inspectService.CreateAsync(d, userId);

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "05" => Conflict(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new ApiResponse<IEnumerable<VehicleInfo>>
            {
                Message = "Unknown Error",
                Status = result.Status,
                Data = result.Data
            })
        };
    }

    [HttpPost("assign/job")]
    public async Task<ActionResult<ApiResponse<IEnumerable<InspectionTransaction>>>> AssignJob
    (
        [FromBody] InspectionRequestJobId d,
        [FromHeader(Name = "userId")] string userId
    )
    {
        var result = await _inspectService.AssignJob(d, userId);

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "05" => Conflict(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new ApiResponse<IEnumerable<InspectionTransaction>>
            {
                Message = "Unknown Error",
                Status = result.Status,
                Data = result.Data
            })
        };
    }

    [HttpPost("job/history")]
    public async Task<ActionResult<ApiResponse<IEnumerable<InspectionTransactionHistory>>>> GetJobHistory(InspectionRequestJobId d)
    {
        var result = await _inspectService.GetJobHistory(d.JobId);

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new ApiResponse<IEnumerable<InspectionTransactionHistory>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("detail/task")]
    public async Task<ActionResult<ApiResponse<IEnumerable<InspectionTransaction>>>> GetTaskDetailAsync(InspectionRequestJobId d)
    {
        var result = await _inspectService.GetTaskDetailAsync(d.JobId);

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new ApiResponse<IEnumerable<InspectionTransaction>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("update/task")]
    public async Task<ActionResult<ApiResponse<IEnumerable<InspectionTaskResponse>>>> UpdateTaskAsync(InspectionTaskRequest d)
    {
        var result = await _inspectService.UpdateTaskAsync(d);

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new ApiResponse<IEnumerable<InspectionTaskResponse>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    // [HttpPost("seq")]
    // public async Task<ActionResult<ApiResponse<string>>> GetSeq()
    // {
    //     var result = await _inspectService.GetSeq();
    //     return Ok(new ApiResponse<string> { Data = result });
    // }

}