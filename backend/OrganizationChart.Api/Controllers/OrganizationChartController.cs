using Microsoft.AspNetCore.Mvc;
using OrganizationChart.Api.Services.Interfaces;

namespace OrganizationChart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationChartController : ControllerBase
{
    private readonly IOrganizationChartService _service;

    public OrganizationChartController(IOrganizationChartService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var chart = await _service.GetOrganizationChartAsync();
        return Ok(chart);
    }
}