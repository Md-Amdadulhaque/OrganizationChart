using ClosedXML.Excel;
using OrganizationChart.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace OrganizationChart.Api.Services;

public class ImportService : IImportService
{
    private readonly AppDbContext _context;

    public ImportService(AppDbContext context)
    {
        _context = context;
    }

    // ---- Column headers as they appear in the source Excel files ----
    // cmn_department.xlsx
    private const string DeptCodeHeader = "ID";
    private const string DeptNameHeader = "Name";
    private const string DeptParentHeader = "Parent";

    // sys_user.xlsx
    // NOTE: the "Employee number" column is empty in the source file, so the
    // per-person identifier comes from "User ID" (e.g. 0002, 0018).
    private const string UserEmployeeNumberHeader = "User ID";
    private const string UserFirstNameHeader = "First name";
    private const string UserLastNameHeader = "Last name";
    private const string UserTitleHeader = "Title";
    private const string UserDepartmentHeader = "Department";

    public async Task ImportDepartmentsAsync(IFormFile file)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);

            var worksheet = workbook?.Worksheet(1);
            if (worksheet == null)
            {
                throw new InvalidOperationException("Worksheet not found.");
            }

            var columns = BuildHeaderMap(worksheet);
            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

            Dictionary<string, Department> departmentDic = await _context.Departments
            .ToDictionaryAsync(d => d.Name, d => d);


            var departments = new List<Department>();

            for (int row = 2; row <= lastRow; row++)
            {
                var departmentCode = GetCell(worksheet, columns, row, DeptCodeHeader);
                var departmentName = GetCell(worksheet, columns, row, DeptNameHeader);

                if (string.IsNullOrWhiteSpace(departmentCode) || string.IsNullOrWhiteSpace(departmentName))
                    continue;

                if (departmentDic.ContainsKey(departmentName))
                    continue;

                var newDept = new Department
                {
                    DepartmentCode = departmentCode,
                    Name = departmentName,
                };
                departments.Add(newDept);
                departmentDic.Add(departmentName, newDept);
            }
            _context.Departments.AddRange(departments);
            await _context.SaveChangesAsync();
            await SetRelations(worksheet, columns);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task SetRelations(IXLWorksheet worksheet, Dictionary<string, int> columns)
    {
        var lookup = await _context.Departments
        .ToDictionaryAsync(d => d.Name, d => d);

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
        for (int row = 2; row <= lastRow; row++)
        {
            var departmentName = GetCell(worksheet, columns, row, DeptNameHeader);
            var parentName = GetCell(worksheet, columns, row, DeptParentHeader);

            if (string.IsNullOrWhiteSpace(parentName))
                continue;

            if (!lookup.TryGetValue(departmentName, out var department))
                continue;

            if (!lookup.TryGetValue(parentName, out var parent))
                continue;
            department.ParentDepartmentId = parent.Id;
        }
        await _context.SaveChangesAsync();
    }

    public async Task ImportUsersAsync(IFormFile file)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);

            var worksheet = workbook.Worksheet(1);

            var columns = BuildHeaderMap(worksheet);
            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

            var departmentLookup = await _context.Departments
                .ToDictionaryAsync(d => d.Name, d => d);

            var existingEmployees = await _context.Users
                .Select(u => u.EmployeeNumber)
                .ToHashSetAsync();

            var users = new List<User>();

            for (int row = 2; row <= lastRow; row++)
            {
                var employeeNumber = GetCell(worksheet, columns, row, UserEmployeeNumberHeader);
                var firstName = GetCell(worksheet, columns, row, UserFirstNameHeader);
                var lastName = GetCell(worksheet, columns, row, UserLastNameHeader);
                var title = GetCell(worksheet, columns, row, UserTitleHeader);
                var departmentName = GetCell(worksheet, columns, row, UserDepartmentHeader);

                if (string.IsNullOrWhiteSpace(employeeNumber))
                    continue;

                if (existingEmployees.Contains(employeeNumber))
                    continue;

                if (!departmentLookup.TryGetValue(departmentName, out var department))
                    continue;

                users.Add(new User
                {
                    EmployeeNumber = employeeNumber,
                    FirstName = firstName,
                    LastName = lastName,
                    Title = title,
                    DepartmentId = department.Id
                });

                existingEmployees.Add(employeeNumber);
            }

            _context.Users.AddRange(users);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    private static Dictionary<string, int> BuildHeaderMap(IXLWorksheet worksheet)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        var headerRow = worksheet.Row(1);
        var lastColumn = headerRow.LastCellUsed()?.Address.ColumnNumber ?? 0;

        for (int col = 1; col <= lastColumn; col++)
        {
            var header = worksheet.Cell(1, col).GetString().Trim();

            if (string.IsNullOrWhiteSpace(header))
                continue;

            map.TryAdd(header, col);
        }

        return map;
    }
    private static string GetCell(
        IXLWorksheet worksheet,
        Dictionary<string, int> columns,
        int row,
        string header)
    {
        if (!columns.TryGetValue(header, out var col))
            return string.Empty;

        return worksheet.Cell(row, col).GetString().Trim();
    }
}
