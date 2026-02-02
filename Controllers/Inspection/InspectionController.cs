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
    public async Task<ActionResult<ApiResponse<IEnumerable<VehicleInfo>>>> CreateAsync(InspectionRequest d)
    {
        var result = await _inspectService.CreateAsync(d);

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new ApiResponse<IEnumerable<VehicleInfo>>
            {
                Message = "Unknown Error",
                Status = result.Status,
                Data = result.Data
            })
        };
    }

    [HttpPost("update/status")]
    public async Task<ActionResult<ApiResponse<InspectionTransaction>>> UpdateStatusAsync(InspectionTransactionHistory d)
    {
        var result = await _inspectService.UpdateStatusAsync(d);

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new ApiResponse<InspectionTransaction>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("seq")]
    public async Task<ActionResult<ApiResponse<string>>> GetSeq()
    {
        var result = await _inspectService.GetSeq();
        return Ok(new ApiResponse<string> { Data = result });
    }

}