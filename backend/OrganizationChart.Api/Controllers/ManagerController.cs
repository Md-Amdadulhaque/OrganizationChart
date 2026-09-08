using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationChart.Api.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Manager")]
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

    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
    {
        if (emailRequest == null || string.IsNullOrEmpty(emailRequest.To))
        {
            return BadRequest("Invalid email request.");
        }
        await _emailSenderService.SendEmailAsync(emailRequest.To, emailRequest.Subject, emailRequest.Body);
        return Ok("Email sent successfully.");
    }
}

public class EmailRequest
{
    public string To { get; set; }
    public string ?Subject { get; set; }
    public string ?Body { get; set; }
}