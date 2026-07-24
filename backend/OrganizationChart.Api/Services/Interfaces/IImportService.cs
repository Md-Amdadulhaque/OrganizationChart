public interface IImportService
{
    Task ImportDepartmentsAsync(IFormFile file);

    Task ImportUsersAsync(IFormFile file);
}