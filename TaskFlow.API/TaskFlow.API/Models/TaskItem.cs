namespace TaskFlow.API.Models;

public class TaskItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DueDate { get; set; }

    public int Priority { get; set; }

    public int CategoryId { get; set; }

    public TaskCategory? Category { get; set; }

    public int UserId { get; set; }

    public AppUser? User { get; set; }
}
