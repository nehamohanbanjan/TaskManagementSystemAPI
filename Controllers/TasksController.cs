using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystemAPI.DTO;
using TaskManagementSystemAPI.Models;
using TaskManagementSystemAPI.Services.Interface;

namespace TaskManagementSystemAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks")]
public class TasksController(ITaskService taskService) : ControllerBase
{
    private readonly ITaskService _taskService = taskService;

    [HttpPost]
    public async Task<IActionResult> Create(TaskDto dto)
    {
        try
        {
            var task = await _taskService.CreateTaskAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery] TaskFilterDto filter)
    {
        var (tasks, totalCount) = await _taskService.GetTasksAsync(filter);
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
        var task = await _taskService.GetTaskByIdAsync(id);
        return task == null ? NotFound() : Ok(task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, TaskDto dto)
    {
        var task = await _taskService.UpdateTaskAsync(id, dto);
        return task == null ? NotFound(new { message = "Task not found" }) : Ok(task);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, TaskItemStatus status)
    {
        var task = await _taskService.UpdateTaskStatusAsync(id, status);
        return task == null ? NotFound() : Ok(task);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _taskService.DeleteTaskAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}