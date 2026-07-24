using Microsoft.EntityFrameworkCore;
using OrganizationChart.Api.DTOs.Department;
using OrganizationChart.Api.Services.Interfaces;
using OrganizationChart.Domain.Models;

namespace OrganizationChart.Api.Services;

public class DepartmentService : IDepartmentService
{
    private readonly AppDbContext _context;

    public DepartmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
    {
        return await _context.Departments
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                DepartmentCode = d.DepartmentCode,
                Name = d.Name,
                ParentDepartmentId = d.ParentDepartmentId
            })
            .ToListAsync();
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department == null)
            return null;

        return new DepartmentDto
        {
            Id = department.Id,
            DepartmentCode = department.DepartmentCode,
            Name = department.Name,
            ParentDepartmentId = department.ParentDepartmentId
        };
    }

    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
    {
        var department = new Department
        {
            DepartmentCode = dto.DepartmentCode,
            Name = dto.Name,
            ParentDepartmentId = dto.ParentDepartmentId
        };

        _context.Departments.Add(department);

        return new DepartmentDto
        {
            Id = department.Id,
            DepartmentCode = department.DepartmentCode,
            Name = department.Name,
            ParentDepartmentId = department.ParentDepartmentId
        };

    }

    public async Task<bool> UpdateAsync(int id, UpdateDepartmentDto dto)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department == null)
            return false;

        var oldValue =
    $"{department.DepartmentCode}|{department.Name}|{department.ParentDepartmentId}";

        department.DepartmentCode = dto.DepartmentCode;
        department.Name = dto.Name;
        department.ParentDepartmentId = dto.ParentDepartmentId;


        var newValue =
    $"{department.DepartmentCode}|{department.Name}|{department.ParentDepartmentId}";

        _context.DepartmentHistories.Add(new DepartmentHistory
        {
            DepartmentId = department.Id,
            Action = "Update",
            OldValue = oldValue,
            NewValue = newValue
        });
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department == null)
            return false;

        _context.Departments.Remove(department);

        await _context.SaveChangesAsync();

        return true;
    }
}