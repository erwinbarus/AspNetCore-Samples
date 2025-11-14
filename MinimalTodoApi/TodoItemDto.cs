namespace MinimalTodoApi;

public class TodoItemDto
{
    public int Id { get; set; }
    public string? Task { get; set; }
    public bool IsComplete { get; set; }

    public TodoItemDto(TodoItem todo)
    {
        Id = todo.Id;
        Task = todo.Task;
        IsComplete = todo.IsComplete;
    }
}