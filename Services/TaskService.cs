using Microsoft.EntityFrameworkCore;
using TaskManagementSystemAPI.Data;
using TaskManagementSystemAPI.DTO;
using TaskManagementSystemAPI.Models;
using TaskManagementSystemAPI.Services.Interface;

namespace TaskManagementSystemAPI.Services
{
    public class TaskService(AppDbContext context) : ITaskService
    {
        private readonly AppDbContext _context = context;

        public async Task<TaskItem> CreateTaskAsync(TaskDto dto)
        {
            await ValidateAssignedUserAsync(dto.AssignedToId);

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
            return task;
        }

        public async Task<(List<TaskItem> tasks, int totalCount)> GetTasksAsync(TaskFilterDto filter)
        {
            var query = _context.Tasks.Include(t => t.AssignedTo).AsQueryable();

            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status.Value);

            if (filter.Priority.HasValue)
                query = query.Where(t => t.Priority == filter.Priority.Value);

            if (filter.AssignedToUserId.HasValue)
                query = query.Where(t => t.AssignedToId == filter.AssignedToUserId);

            if (filter.DueDateFrom.HasValue)
            {
                var fromUtc = DateTime.SpecifyKind(
                    filter.DueDateFrom.Value,
                    DateTimeKind.Utc);

                query = query.Where(t => t.DueDate >= fromUtc);
            }

            if (filter.DueDateTo.HasValue)
            {
                var toUtc = DateTime.SpecifyKind(
                    filter.DueDateTo.Value,
                    DateTimeKind.Utc);

                query = query.Where(t => t.DueDate <= toUtc);
            }

            var totalCount = await query.CountAsync();

            var tasks = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (tasks, totalCount);
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<TaskItem?> UpdateTaskAsync(int id, TaskDto dto)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return null;
            await ValidateAssignedUserAsync(dto.AssignedToId);

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Status = dto.Status;
            task.Priority = dto.Priority;
            task.DueDate = dto.DueDate?.ToUniversalTime();
            task.AssignedToId = dto.AssignedToId;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskItem?> UpdateTaskStatusAsync(int id, TaskItemStatus status)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return null;

            task.Status = status;
            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return task;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task ValidateAssignedUserAsync(int? assignedToId)
        {
            if (!assignedToId.HasValue)
                return;

            var userExists = await _context.Users
                .AnyAsync(u => u.Id == assignedToId.Value);

            if (!userExists)
                throw new KeyNotFoundException("Assigned user not found");
        }

    }
}
