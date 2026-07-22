using OrganizationChart.Api.DTOs.OrganizationChart;

namespace OrganizationChart.Api.Services.Interfaces;

public interface IOrganizationChartService
{
    Task<List<OrganizationChartNodeDto>> GetOrganizationChartAsync();
}