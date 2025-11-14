using MinimalTodoApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(opt =>
{
    opt.DocumentName = "TodoApi";
    opt.Title = "TodoAPI v1";
    opt.Version = "v1";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(opt =>
    {
        opt.DocumentTitle = "TodoAPI";
        opt.Path = "/swagger";
        opt.DocumentPath = "/swagger/{documentName}/swagger.json";
        opt.DocExpansion = "list";
    });
}

RouteGroupBuilder todoItems = app.MapGroup("/todoitems");

todoItems.MapGet("/", GetAllTodo);
todoItems.MapGet("/complete", GetCompleteTodo);
todoItems.MapGet("/{id}", GetTodo);
todoItems.MapPost("/", CreateTodo);
todoItems.MapPut("/{id}", UpdateTodo);
todoItems.MapDelete("/{id}", DeleteTodo);

static async Task<IResult> GetAllTodo(TodoDb db)
{
    return TypedResults.Ok(await db.TodoItems.ToListAsync());
}

static async Task<IResult> GetCompleteTodo(TodoDb db)
{
    return TypedResults.Ok(await db.TodoItems.Where(x => x.IsComplete).Select(x => new TodoItemDto(x)).ToListAsync());
}

static async Task<IResult> GetTodo(int id, TodoDb db)
{
    return await db.TodoItems.FindAsync(id)
        is TodoItem todo
            ? TypedResults.Ok(new TodoItemDto(todo))
            : TypedResults.NotFound();
}

static async Task<IResult> CreateTodo(TodoItem todoItem, TodoDb db)
{
    db.TodoItems.Add(todoItem);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/todoitems/{todoItem.Id}", new TodoItemDto(todoItem));
}

static async Task<IResult> UpdateTodo(int id, TodoItem updatedTodo, TodoDb db)
{
    var todo = await db.TodoItems.FindAsync(id);

    if (todo is null)
    {
        return Results.NotFound();
    }

    todo.Task = updatedTodo.Task;
    todo.IsComplete = updatedTodo.IsComplete;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteTodo(int id, TodoDb db)
{
    if (await db.TodoItems.FindAsync(id) is TodoItem todo)
    {
        db.TodoItems.Remove(todo);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}

app.Run();
