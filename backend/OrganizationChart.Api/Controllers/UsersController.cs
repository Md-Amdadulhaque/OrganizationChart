using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganizationChart.Api.DTOs.User;
using OrganizationChart.Api.Services.Interfaces;
using OrganizationChart.Domain.Models;

namespace OrganizationChart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _userService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        var user = await _userService.CreateAsync(dto);

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateUserDto dto)
    {
        var updated = await _userService.UpdateAsync(id, dto);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _userService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}