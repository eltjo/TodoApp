using System.Text.Json;
using TodoApp.Models;

namespace TodoApp.Services;

public class TodoService
{
    private readonly string _filePath;
    private List<TodoItem> _cache = [];

    public TodoService(IWebHostEnvironment env)
    {
        var dir = Path.Combine(env.ContentRootPath, ".todo-data");
        Directory.CreateDirectory(dir);
        _filePath = Path.Combine(dir, "todos.json");
        Load();
    }

    public IReadOnlyList<TodoItem> GetAll() => _cache.AsReadOnly();

    public void Add(string title, TodoPriority priority = TodoPriority.Medium)
    {
        _cache.Add(new TodoItem { Title = title.Trim(), Priority = priority });
        Save();
    }

    public void Complete(Guid id)
    {
        var item = _cache.FirstOrDefault(t => t.Id == id);
        if (item is not null)
        {
            item.IsCompleted = true;
            item.CompletedAt = DateTime.Now;
            Save();
        }
    }

    public void Delete(Guid id)
    {
        _cache.RemoveAll(t => t.Id == id);
        Save();
    }

    /// <summary>
    /// Computes productivity insights over all todos: totals, completion rate,
    /// a per-priority breakdown of open items, and the number of todos completed
    /// on each of the last 7 days (oldest first).
    /// </summary>
    public TodoInsights GetInsights()
    {
        var total = _cache.Count;
        var done = _cache.Count(t => t.IsCompleted);
        var open = total - done;

        var byPriority = Enum.GetValues<TodoPriority>()
            .ToDictionary(p => p, p => _cache.Count(t => !t.IsCompleted && t.Priority == p));

        var today = DateTime.Now.Date;
        var weekly = new List<DailyCount>();
        for (var offset = 6; offset >= 0; offset--)
        {
            var day = today.AddDays(-offset);
            var count = _cache.Count(t => t.CompletedAt is { } c && c.Date == day);
            weekly.Add(new DailyCount(day, count));
        }

        return new TodoInsights(
            Total: total,
            Open: open,
            Done: done,
            CompletionRate: total == 0 ? 0 : (double)done / total,
            OpenByPriority: byPriority,
            CompletedLast7Days: weekly);
    }

    private void Load()
    {
        if (!File.Exists(_filePath)) return;
        var json = File.ReadAllText(_filePath);
        _cache = JsonSerializer.Deserialize<List<TodoItem>>(json) ?? [];
    }

    private void Save()
    {
        var json = JsonSerializer.Serialize(_cache, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}
