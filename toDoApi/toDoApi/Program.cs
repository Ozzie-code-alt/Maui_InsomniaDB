using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using toDoApi.Data;
using toDoApi.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection")));

var app = builder.Build();



//app.UseHttpsRedirection(); //redirects to http endpoints 

app.MapGet("api/todo", async (AppDbContext context) =>
{
    var items = await context.ToDos.ToListAsync();

    return Results.Ok(items);
});

app.MapPost("api/todo", async (AppDbContext context, ToDo toDo) =>
{
await context.ToDos.AddAsync(toDo);

await context.SaveChangesAsync();
    return Results.Created($"api/todo/{toDo.Id}", toDo);
});



app.MapPut("api/todo/{id}", async (AppDbContext context, int id, ToDo todo) =>
{
    var toDoModel = await context.ToDos.FirstOrDefaultAsync(t => t.Id == id);

    if(toDoModel == null)
    {
        return Results.NotFound();
    }
    toDoModel.ToDoName= todo.ToDoName;
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("api/todo/{id}", async (AppDbContext context, int id) =>
{
    var toDoModel = await context.ToDos.FirstOrDefaultAsync(t => t.Id == id);

    if (toDoModel == null)
    {
        return Results.NotFound();
    }

    context.ToDos.Remove(toDoModel);
    await context.SaveChangesAsync();
    return Results.NoContent();

});
app.Run();

