using Microsoft.AspNetCore.Mvc;
using MyBackend.Models.Utils.Paged;
using MyBackend.Services.Master;
using MyBackend.Models.Master;

namespace MyBackend.Controllers.Master;

[Route("api/[controller]")]
[ApiController]
public class MasterController : ControllerBase
{

    private readonly IMasterService _service;

    public MasterController(IMasterService service)
    {
        _service = service;
    }

    [HttpPost("source")]
    public async Task<ActionResult<PagedResult<IEnumerable<MasterDropdown>>>> GetSourceList()
    {
        var result = await _service.GetSourceList();

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new PagedResult<IEnumerable<MasterDropdown>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("agent")]
    public async Task<ActionResult<PagedResult<IEnumerable<MasterDropdown>>>> GetAgentList()
    {
        var result = await _service.GetAgentList();

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new PagedResult<IEnumerable<MasterDropdown>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("bu")]
    public async Task<ActionResult<PagedResult<IEnumerable<MasterDropdown>>>> GetBUList()
    {
        var result = await _service.GetBUList();

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new PagedResult<IEnumerable<MasterDropdown>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("reason/servey")]
    public async Task<ActionResult<PagedResult<IEnumerable<MasterDropdown>>>> GetReasonServeyList()
    {
        var result = await _service.GetReasonServeyList();

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new PagedResult<IEnumerable<MasterDropdown>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("province")]
    public async Task<ActionResult<PagedResult<IEnumerable<MasterDropdown>>>> GetProvinceList()
    {
        var result = await _service.GetProvinceList();

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new PagedResult<IEnumerable<MasterDropdown>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("car/brand")]
    public async Task<ActionResult<PagedResult<IEnumerable<MasterDropdown>>>> GetCarBrandList()
    {
        var result = await _service.GetCarBrandList();

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new PagedResult<IEnumerable<MasterDropdown>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("car/model")]
    public async Task<ActionResult<PagedResult<IEnumerable<MasterDropdown>>>> GetCarModelList(MasterCarRequest d)
    {
        var result = await _service.GetCarModelList(d.CarBrand);

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new PagedResult<IEnumerable<MasterDropdown>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("job/state/job")]
    public async Task<ActionResult<PagedResult<IEnumerable<MasterDropdown>>>> GetStateJob()
    {
        var result = await _service.GetJobStateList("01");

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new PagedResult<IEnumerable<MasterDropdown>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("job/state/follow/appointment")]
    public async Task<ActionResult<PagedResult<IEnumerable<MasterDropdown>>>> GetFollowApp()
    {
        var result = await _service.GetJobStateList("02");

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new PagedResult<IEnumerable<MasterDropdown>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("job/state/send/sv")]
    public async Task<ActionResult<PagedResult<IEnumerable<MasterDropdown>>>> GetSendSV()
    {
        var result = await _service.GetJobStateList("03");

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new PagedResult<IEnumerable<MasterDropdown>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("job/state/follow/sv")]
    public async Task<ActionResult<PagedResult<IEnumerable<MasterDropdown>>>> GetFollowSV()
    {
        var result = await _service.GetJobStateList("04");

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new PagedResult<IEnumerable<MasterDropdown>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("job/state/waiting/result")]
    public async Task<ActionResult<PagedResult<IEnumerable<MasterDropdown>>>> GetWaitReult()
    {
        var result = await _service.GetJobStateList("05");

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new PagedResult<IEnumerable<MasterDropdown>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("remark/method")]
    public async Task<ActionResult<PagedResult<IEnumerable<MasterDropdown>>>> GetRemarkMethodList()
    {
        var result = await _service.GetRemarkMethodList();

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new PagedResult<IEnumerable<MasterDropdown>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("service/region")]
    public async Task<ActionResult<PagedResult<IEnumerable<MasterDropdown>>>> GetRegionList()
    {
        var result = await _service.GetRegionList();

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new PagedResult<IEnumerable<MasterDropdown>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("service/province")]
    public async Task<ActionResult<PagedResult<IEnumerable<MasterDropdown>>>> GetProvinceCodeList()
    {
        var result = await _service.GetProvinceCodeList();

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new PagedResult<IEnumerable<MasterDropdown>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("service/district")]
    public async Task<ActionResult<PagedResult<IEnumerable<MasterDropdown>>>> GetDistrictList()
    {
        var result = await _service.GetDistrictList();

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new PagedResult<IEnumerable<MasterDropdown>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

    [HttpPost("company/servey")]
    public async Task<ActionResult<PagedResult<IEnumerable<MasterDropdownPrice>>>> GetServeyServiceList(MasterServiceRequest d)
    {
        var result = await _service.GetServeyServiceList(d.RegionCode! , d.ProvinceCode!, d.DistrictCode!);

        return result.Status switch
        {
            "00" => Ok(result),
            "01" => Conflict(result),
            "02" => NotFound(result),
            "99" => StatusCode(500, result),
            _ => BadRequest(new PagedResult<IEnumerable<MasterDropdownPrice>>
            {
                Message = "Unknown Error",
                Status = result.Status
            })
        };
    }

}