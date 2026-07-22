
namespace OrganizationChart.Domain.Models;

public class Department
{
    public int Id { get; set; }

    public string DepartmentCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int? ParentDepartmentId { get; set; }

    public Department? ParentDepartment { get; set; }

    public ICollection<Department> Children { get; set; }
        = new List<Department>();

    public ICollection<User> Users { get; set; }
        = new List<User>();
}