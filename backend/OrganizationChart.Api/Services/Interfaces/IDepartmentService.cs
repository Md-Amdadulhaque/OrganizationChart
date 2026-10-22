using OrganizationChart.Api.DTOs.Department;
using OrganizationChart.Domain.Models; 
namespace OrganizationChart.Api.Services.Interfaces;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllAsync();

    Task<DepartmentDto?> GetByIdAsync(int id);

    Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto);

    Task<bool> UpdateAsync(int id, UpdateDepartmentDto dto);

    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<User>> GetEmployeesInDepartmentAsync(string departmentId);
}