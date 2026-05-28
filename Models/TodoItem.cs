namespace TodoApp.Models;

public enum TodoPriority
{
    Low,
    Medium,
    High
}

public class TodoItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public TodoPriority Priority { get; set; } = TodoPriority.Medium;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? CompletedAt { get; set; }
}
