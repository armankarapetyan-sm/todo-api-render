using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Authorize]
[Route("api/todos")]
public class TodosController : ControllerBase
{
    private readonly AppDbContext _db;

    public TodosController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TodoView>>> List()
    {
        int userId = CurrentUserId();
        List<TodoItem> rows = await _db.Todos
            .AsNoTracking()
            .Where(todo => todo.UserId == userId)
            .OrderBy(todo => todo.Id)
            .ToListAsync();

        string name = User.Identity != null && User.Identity.Name != null ? User.Identity.Name : "";
        List<TodoView> views = new List<TodoView>();
        foreach (TodoItem row in rows)
        {
            views.Add(TodoView.From(row, name));
        }

        return Ok(views);
    }

    [HttpPost]
    public async Task<ActionResult<TodoView>> Create([FromBody] TodoForm form)
    {
        string title = (form.Title ?? "").Trim();
        if (title.Length == 0)
        {
            return BadRequest("Title is required.");
        }

        TodoItem item = new TodoItem
        {
            UserId = CurrentUserId(),
            Title = title,
            Done = form.Done
        };
        _db.Todos.Add(item);
        await _db.SaveChangesAsync();

        string name = User.Identity != null && User.Identity.Name != null ? User.Identity.Name : "";
        return CreatedAtAction(nameof(Get), new { id = item.Id }, TodoView.From(item, name));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoView>> Get(int id)
    {
        TodoItem? item = await FindMine(id);
        if (item == null)
        {
            return NotFound();
        }

        string name = User.Identity != null && User.Identity.Name != null ? User.Identity.Name : "";
        return Ok(TodoView.From(item, name));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TodoView>> Update(int id, [FromBody] TodoForm form)
    {
        TodoItem? item = await FindMine(id);
        if (item == null)
        {
            return NotFound();
        }

        string title = (form.Title ?? "").Trim();
        if (title.Length == 0)
        {
            return BadRequest("Title is required.");
        }

        item.Title = title;
        item.Done = form.Done;
        await _db.SaveChangesAsync();

        string name = User.Identity != null && User.Identity.Name != null ? User.Identity.Name : "";
        return Ok(TodoView.From(item, name));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        TodoItem? item = await FindMine(id);
        if (item == null)
        {
            return NotFound();
        }

        _db.Todos.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<TodoItem?> FindMine(int id)
    {
        int userId = CurrentUserId();
        return await _db.Todos.FirstOrDefaultAsync(todo => todo.Id == id && todo.UserId == userId);
    }

    private int CurrentUserId()
    {
        string? raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int id;
        if (int.TryParse(raw, out id))
        {
            return id;
        }

        return 0;
    }
}
