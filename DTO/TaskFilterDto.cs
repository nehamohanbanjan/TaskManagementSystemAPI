using TaskManagementSystemAPI.Models;

namespace TaskManagementSystemAPI.DTO
{
    public class TaskFilterDto
    {
        public TaskItemStatus? Status { get; set; }
        public TaskPriority? Priority { get; set; }
        public int? AssignedToUserId { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;


    }
}
