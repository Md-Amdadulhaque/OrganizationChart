using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganizationChart.Api.DTOs.Department;
using OrganizationChart.Api.Services.Interfaces;
using OrganizationChart.Domain.Models;

namespace OrganizationChart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var departments = await _departmentService.GetAllAsync();
        return Ok(departments);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var department = await _departmentService.GetByIdAsync(id);

        if (department == null)
            return NotFound();

        return Ok(department);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateDepartmentDto dto)
    {
        var department = await _departmentService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = department.Id },
            department);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateDepartmentDto dto)
    {
        var updated = await _departmentService.UpdateAsync(id, dto);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _departmentService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (DbUpdateException)
        {
            // FK restrict: the department still has members or sub-departments.
            return Conflict(new
            {
                message = "This department still has members or sub-departments and cannot be deleted."
            });
        }
    }
}