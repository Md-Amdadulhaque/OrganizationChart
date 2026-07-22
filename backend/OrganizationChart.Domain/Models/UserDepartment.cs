namespace OrganizationChart.Domain.Models;

public class UserDepartment
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int DepartmentId { get; set; }

    public bool IsPrimary { get; set; }
}