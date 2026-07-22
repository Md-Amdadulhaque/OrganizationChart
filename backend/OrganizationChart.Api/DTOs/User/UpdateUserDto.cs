using System.ComponentModel.DataAnnotations;

namespace OrganizationChart.Api.DTOs.User;

public class UpdateUserDto
{
    [Required]
    [MaxLength(20)]
    public string EmployeeNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public int DepartmentId { get; set; }
}