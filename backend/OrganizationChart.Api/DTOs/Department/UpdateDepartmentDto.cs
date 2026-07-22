namespace OrganizationChart.Api.DTOs.Department;

public class UpdateDepartmentDto
{
    public string DepartmentCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int? ParentDepartmentId { get; set; }
}