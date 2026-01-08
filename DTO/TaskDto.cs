using System.ComponentModel.DataAnnotations;
using TaskManagementSystemAPI.Models;

namespace TaskManagementSystemAPI.DTO
{
    public class TaskDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public TaskItemStatus Status { get; set; } = TaskItemStatus.TODO;

        public TaskPriority Priority { get; set; } = TaskPriority.MEDIUM;

        public DateTime? DueDate { get; set; }

        public int? AssignedToId { get; set; }
    }
}
