namespace OrganizationChart.Domain.Models;

public class UserHistory
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Action { get; set; } = string.Empty;

    public DateTime ChangedAt { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }
}