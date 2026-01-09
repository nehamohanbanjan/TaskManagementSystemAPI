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
public async Task<IActionResult> GetTasks([FromQuery] TaskFilterDto filter)
{
    var query = _context.Tasks
        .Include(t => t.AssignedTo)
        .AsQueryable();



    if (filter.Status.HasValue)
        query = query.Where(t => t.Status == filter.Status.Value);


    if (filter.Priority.HasValue)
        query = query.Where(t => t.Priority == filter.Priority.Value);


    if (filter.AssignedToUserId.HasValue)
        query = query.Where(t => t.AssignedToId == filter.AssignedToUserId);

    if (filter.DueDateFrom.HasValue)
        query = query.Where(t => t.DueDate >= filter.DueDateFrom);

    if (filter.DueDateTo.HasValue)
        query = query.Where(t => t.DueDate <= filter.DueDateTo);

    var totalCount = await query.CountAsync();

    var tasks = await query
        .Skip((filter.Page - 1) * filter.PageSize)
        .Take(filter.PageSize)
        .ToListAsync();

    return Ok(new
    {
        totalCount,
        filter.Page,
        filter.PageSize,
        data = tasks
    });
}


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        return task == null ? NotFound() : Ok(task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, TaskDto dto)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
            return NotFound(new { message = "Task not found" });

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Status = dto.Status;
        task.Priority = dto.Priority;
        task.DueDate = dto.DueDate;
        task.AssignedToId = dto.AssignedToId;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(task);
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