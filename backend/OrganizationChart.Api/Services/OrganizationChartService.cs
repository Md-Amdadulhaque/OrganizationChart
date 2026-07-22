using Microsoft.EntityFrameworkCore;
using OrganizationChart.Api.DTOs.OrganizationChart;
using OrganizationChart.Api.DTOs.User;
using OrganizationChart.Api.Services.Interfaces;

namespace OrganizationChart.Api.Services;

public class OrganizationChartService : IOrganizationChartService
{
    private readonly AppDbContext _context;

    public OrganizationChartService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrganizationChartNodeDto>> GetOrganizationChartAsync()
    {
        var departments = await _context.Departments
    .AsNoTracking()
    .ToListAsync();


        var users = await _context.Users
        .AsNoTracking()
        .ToListAsync();

        var nodes = departments.ToDictionary(
        d => d.Id,
        d => new OrganizationChartNodeDto
        {
            DepartmentId = d.Id,
            DepartmentCode = d.DepartmentCode,
            DepartmentName = d.Name
        });

        foreach (var user in users)
        {
            if (nodes.TryGetValue(user.DepartmentId, out var node))
            {
                node.Users.Add(new UserDto
                {
                    Id = user.Id,
                    EmployeeNumber = user.EmployeeNumber,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Title = user.Title,
                    DepartmentId = user.DepartmentId,
                    DepartmentName = node.DepartmentName
                });
            }
        }
        var roots = new List<OrganizationChartNodeDto>();

        foreach (var department in departments)
        {
            var node = nodes[department.Id];

            if (department.ParentDepartmentId == null)
            {
                roots.Add(node);
            }
            else if (nodes.TryGetValue(department.ParentDepartmentId.Value, out var parent))
            {
                parent.Children.Add(node);
            }
        }
        return roots;
    }
}