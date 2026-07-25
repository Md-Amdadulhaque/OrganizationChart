
using Microsoft.EntityFrameworkCore;
using OrganizationChart.Domain.Models;


public class AppDbContext : DbContext {

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<User> Users => Set<User>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<UserHistory> UserHistories => Set<UserHistory>();
    public DbSet<DepartmentHistory> DepartmentHistories => Set<DepartmentHistory>();
    public DbSet<AuthUser> AuthUsers => Set<AuthUser>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Department self-reference
        modelBuilder.Entity<Department>()
            .HasOne(d => d.ParentDepartment)
            .WithMany(d => d.Children)
            .HasForeignKey(d => d.ParentDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Department -> Users
        modelBuilder.Entity<User>()
            .HasOne(u => u.Department)
            .WithMany(d => d.Users)
            .HasForeignKey(u => u.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Auth accounts: unique username
        modelBuilder.Entity<AuthUser>()
            .HasIndex(a => a.Username)
            .IsUnique();

        modelBuilder.Entity<AuthUser>()
            .Property(a => a.Username)
            .HasMaxLength(50);

        modelBuilder.Entity<AuthUser>()
            .Property(a => a.Role)
            .HasMaxLength(20);
    }
}