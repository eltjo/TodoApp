namespace TodoApp.Models;

/// <summary>Number of todos completed on a given calendar day.</summary>
public record DailyCount(DateTime Day, int Count);

/// <summary>Aggregated productivity statistics for the todo list.</summary>
public record TodoInsights(
    int Total,
    int Open,
    int Done,
    double CompletionRate,
    IReadOnlyDictionary<TodoPriority, int> OpenByPriority,
    IReadOnlyList<DailyCount> CompletedLast7Days);
