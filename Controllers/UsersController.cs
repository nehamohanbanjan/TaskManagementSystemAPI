using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystemAPI.Data;
using TaskManagementSystemAPI.DTO;
using TaskManagementSystemAPI.Models;

namespace TaskManagementSystemAPI.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    public UsersController(AppDbContext context) => _context = context;


    [HttpPost]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == dto.Email);

        if (emailExists)
            return Conflict(new { message = "Email already registered" });

        var hasher = new PasswordHasher<User>();

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email
        };

        user.PasswordHash = hasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return StatusCode(201, new { message = "User registered successfully" });
    }



    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        => Ok(await _context.Users.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound(new { message = $"Task with id {id} not found" });

        return Ok(user);
    }

}