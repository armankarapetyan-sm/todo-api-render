using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Auth;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin/todos")]
public class AdminTodosController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminTodosController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TodoView>>> ListAll()
    {
        List<TodoItem> rows = await _db.Todos
            .AsNoTracking()
            .OrderBy(todo => todo.UserId)
            .ThenBy(todo => todo.Id)
            .ToListAsync();

        Dictionary<int, string> names = await _db.Users
            .AsNoTracking()
            .ToDictionaryAsync(user => user.Id, user => user.UserName);

        List<TodoView> views = new List<TodoView>();
        foreach (TodoItem row in rows)
        {
            string owner = "";
            if (names.ContainsKey(row.UserId))
            {
                owner = names[row.UserId];
            }

            views.Add(TodoView.From(row, owner));
        }

        return Ok(views);
    }
}
