using OrganizationChart.Api.DTOs.User;

namespace OrganizationChart.Api.DTOs.OrganizationChart;

public class OrganizationChartNodeDto
{
    public int DepartmentId { get; set; }

    public string DepartmentCode { get; set; } = string.Empty;

    public string DepartmentName { get; set; } = string.Empty;
    public UserDto? Manager { get; set; }

    public List<UserDto> Users { get; set; } = new();

    public List<OrganizationChartNodeDto> Children { get; set; } = new();
}