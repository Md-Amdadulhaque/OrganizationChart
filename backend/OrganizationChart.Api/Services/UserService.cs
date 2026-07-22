using Microsoft.EntityFrameworkCore;
using OrganizationChart.Api.DTOs.User;
using OrganizationChart.Api.Services.Interfaces;
using OrganizationChart.Domain.Models;

namespace OrganizationChart.Api.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Department)
            .Select(u => new UserDto
            {
                Id = u.Id,
                EmployeeNumber = u.EmployeeNumber,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Title = u.Title,
                DepartmentId = u.DepartmentId,
                DepartmentName = u.Department.Name
            })
            .ToListAsync();
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _context.Users
            .Include(u => u.Department)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return null;

        return new UserDto
        {
            Id = user.Id,
            EmployeeNumber = user.EmployeeNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Title = user.Title,
            DepartmentId = user.DepartmentId,
            DepartmentName = user.Department.Name
        };
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        var department = await _context.Departments.FindAsync(dto.DepartmentId);

        if (department == null)
            throw new ArgumentException("Department does not exist.");

        var user = new User
        {
            EmployeeNumber = dto.EmployeeNumber,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Title = dto.Title,
            DepartmentId = dto.DepartmentId
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            EmployeeNumber = user.EmployeeNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Title = user.Title,
            DepartmentId = department.Id,
            DepartmentName = department.Name
        };
    }

    public async Task<bool> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return false;

        var departmentExists = await _context.Departments
            .AnyAsync(d => d.Id == dto.DepartmentId);

        if (!departmentExists)
            throw new ArgumentException("Department does not exist.");

        user.EmployeeNumber = dto.EmployeeNumber;
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Title = dto.Title;
        user.DepartmentId = dto.DepartmentId;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return false;

        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        return true;
    }
}