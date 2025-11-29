using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Due date is required")]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Priority Level")]
        public Priority Priority { get; set; }

        [Display(Name = "Completed?")]
        public bool IsCompleted { get; set; }

        [Display(Name = "Category")]
        public string Category { get; set; } = string.Empty;

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
    public enum Priority
    {
        Low,
        Medium,
        High,
        Urgent
    }
}
