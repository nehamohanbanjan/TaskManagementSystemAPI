using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystemAPI.Data;
using TaskManagementSystemAPI.DTO;
using TaskManagementSystemAPI.Models;

namespace TaskManagementSystemAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;
    public TasksController(AppDbContext context) => _context = context;

    [HttpPost]
    public async Task<IActionResult> Create(TaskDto dto)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == dto.AssignedToId);
        if (!userExists)
            return NotFound(new { message = "Assigned user not found" });

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            Priority = dto.Priority,
            DueDate = dto.DueDate?.ToUniversalTime(),
            AssignedToId = dto.AssignedToId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = task.Id },
            task
        );
    }



    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        => Ok(await _context.Tasks.Include(t => t.AssignedTo)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        return task == null ? NotFound() : Ok(task);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, TaskItemStatus status)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound();
        task.Status = status;
        task.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(task);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound();
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}