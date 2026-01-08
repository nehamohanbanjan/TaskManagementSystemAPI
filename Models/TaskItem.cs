using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystemAPI.Models;

public enum TaskItemStatus { TODO, IN_PROGRESS, DONE }
public enum TaskPriority { LOW, MEDIUM, HIGH }

public class TaskItem
{
    public int Id { get; set; }

    [Required, MinLength(3), MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public TaskItemStatus Status { get; set; } = TaskItemStatus.TODO;
    public TaskPriority Priority { get; set; } = TaskPriority.MEDIUM;

    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }
}