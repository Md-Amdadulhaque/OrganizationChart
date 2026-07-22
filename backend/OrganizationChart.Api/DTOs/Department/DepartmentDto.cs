namespace OrganizationChart.Api.DTOs.Department;

public class DepartmentDto
{
    public int Id { get; set; }

    public string DepartmentCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int? ParentDepartmentId { get; set; }
}