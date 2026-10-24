using System.ComponentModel.DataAnnotations;
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

    private readonly IEmailPublisher _emailPublisher;

    public ManagerController(
        IDepartmentService departmentService,IEmailPublisher emailPublisher)
    {
        _departmentService = departmentService;
        _emailPublisher = emailPublisher;
    }

    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
    {
        if (emailRequest == null || string.IsNullOrEmpty(emailRequest.To))
        {
            return BadRequest("Invalid email request.");
        }
       // await _emailSenderService.SendEmailAsync(emailRequest.To, emailRequest.Subject, emailRequest.Body);
        return Ok("Email sent successfully.");
    }
    [HttpPost("SendEmailToDepartment")]
    public async Task<IActionResult> SendEmailToDepartment([FromBody] SendEmailToDepartmentRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.DepartmentId))
        {
            return BadRequest("Invalid request.");
        }

        var employees = await _departmentService.GetEmployeesInDepartmentAsync(request.DepartmentId);
        foreach (var employee in employees)
        {
            await _emailPublisher.PublishEmailAsync(employee.Email, request.Subject, request.Body);
        }
        return Ok("Emails sent successfully.");
    }
}


public class EmailRequest
{  
    public required string To { get; set; }
    public string ?Subject { get; set; }
    public string ?Body { get; set; }
}

public class SendEmailToDepartmentRequest
{
    public required string DepartmentId { get; set; }
    public string ?Subject { get; set; }
    public string ?Body { get; set; }
}