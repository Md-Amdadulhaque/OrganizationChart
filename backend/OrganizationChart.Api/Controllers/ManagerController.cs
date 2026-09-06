using Microsoft.AspNetCore.Mvc;
using OrganizationChart.Api.Services.Interfaces;
using OrganizationChart.Api.DTOs;

[ApiController]
[Route("api/[controller]")]
public class ManagerController : ControllerBase
{
    private readonly IDepartmentService _departmentService;
    private readonly IEmailSenderService _emailSenderService;

    public ManagerController(
        IDepartmentService departmentService,
        IEmailSenderService emailSenderService)
    {
        _departmentService = departmentService;
        _emailSenderService = emailSenderService;
    }

    // ...
}