using Microsoft.AspNetCore.Mvc;
using TaskManagementSystemAPI.DTO;
using TaskManagementSystemAPI.Services.Interface;

namespace TaskManagementSystemAPI.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        if (await _userService.EmailExistsAsync(dto.Email))
            return Conflict(new { message = "Email already registered" });

        await _userService.RegisterUserAsync(dto);
        return StatusCode(201, new { message = "User registered successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
    {
        var users = await _userService.GetAllUsersAsync(page, pageSize);
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
            return NotFound(new { message = $"User with id {id} not found" });

        return Ok(user);
    }
}