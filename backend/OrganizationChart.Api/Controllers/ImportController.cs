using Microsoft.AspNetCore.Mvc;
using OrganizationChart.Api.Services.Interfaces;

namespace OrganizationChart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportController : ControllerBase
{
    private readonly IImportService _service;

    public ImportController(IImportService service)
    {
        _service = service;
    }

    [HttpPost("departments")]
    public async Task<IActionResult> ImportDepartments(IFormFile file)
    {
        await _service.ImportDepartmentsAsync(file);
        return Ok("Departments imported successfully.");
    }

    [HttpPost("users")]
    public async Task<IActionResult> ImportUsers(IFormFile file)
    {
        await _service.ImportUsersAsync(file);
        return Ok("Users imported successfully.");
    }
}