using TaskManagementSystemAPI.DTO;
using TaskManagementSystemAPI.Models;

namespace TaskManagementSystemAPI.Services.Interface
{
    public interface ITaskService
    {
        Task<TaskItem> CreateTaskAsync(TaskDto dto);
        Task<(List<TaskItem> tasks, int totalCount)> GetTasksAsync(TaskFilterDto filter);
        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task<TaskItem?> UpdateTaskAsync(int id, TaskDto dto);
        Task<TaskItem?> UpdateTaskStatusAsync(int id, TaskItemStatus status);
        Task<bool> DeleteTaskAsync(int id);
    }
}
