using System.ComponentModel.DataAnnotations;
using TaskManagementSystemAPI.Models;

namespace TaskManagementSystemAPI.DTO
{
    public class TaskDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(100)]
        public string Title { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public TaskItemStatus Status { get; set; }

        [Required]
        public TaskPriority Priority { get; set; }

        public DateTime? DueDate { get; set; }

        public int? AssignedToId { get; set; }
    }
}
