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

            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

            Dictionary<string, Department> departmentDic = await _context.Departments
            .ToDictionaryAsync(d => d.Name, d => d);


            var departments = new List<Department>();

            for (int row = 2; row <= lastRow; row++)
            {
                var departmentCode = worksheet.Cell(row, 1).GetString().Trim();
                var departmentName = worksheet.Cell(row, 2).GetString().Trim();

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
            await SetRelations(worksheet);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

    }

    private async Task SetRelations(IXLWorksheet worksheet)
    {
        var lookup = await _context.Departments
        .ToDictionaryAsync(d => d.Name, d => d);

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
        for (int row = 2; row <= lastRow; row++)
        {
            var departmentName = worksheet.Cell(row, 2).GetString().Trim();
            var parentName = worksheet.Cell(row, 3).GetString().Trim();

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

    }
}