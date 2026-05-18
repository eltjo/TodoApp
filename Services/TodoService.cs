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

    public void Add(string title)
    {
        _cache.Add(new TodoItem { Title = title.Trim() });
        Save();
    }

    public void Complete(Guid id)
    {
        var item = _cache.FirstOrDefault(t => t.Id == id);
        if (item is not null)
        {
            item.IsCompleted = true;
            Save();
        }
    }

    public void Delete(Guid id)
    {
        _cache.RemoveAll(t => t.Id == id);
        Save();
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
