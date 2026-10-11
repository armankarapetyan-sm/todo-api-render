using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Auth;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokens;
    private readonly PasswordHasher<AppUser> _hasher;

    public AuthController(AppDbContext db, TokenService tokens)
    {
        _db = db;
        _tokens = tokens;
        _hasher = new PasswordHasher<AppUser>();
    }

    [HttpPost("register")]
    public async Task<ActionResult<TokenResponse>> Register([FromBody] RegisterForm form)
    {
        string name = (form.UserName ?? "").Trim();
        if (name.Length < 3 || string.IsNullOrWhiteSpace(form.Password) || form.Password.Length < 6)
        {
            return BadRequest("User name min 3, password min 6.");
        }

        bool taken = await _db.Users.AnyAsync(user => user.UserName == name);
        if (taken)
        {
            return Conflict("That name is taken.");
        }

        AppUser user = new AppUser
        {
            UserName = name,
            Role = AppRoles.User
        };
        user.PasswordHash = _hasher.HashPassword(user, form.Password);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(_tokens.Create(user));
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginForm form)
    {
        string name = (form.UserName ?? "").Trim();
        AppUser? user = await _db.Users.FirstOrDefaultAsync(row => row.UserName == name);
        if (user == null)
        {
            return Unauthorized();
        }

        PasswordVerificationResult check = _hasher.VerifyHashedPassword(user, user.PasswordHash, form.Password ?? "");
        if (check == PasswordVerificationResult.Failed)
        {
            return Unauthorized();
        }

        return Ok(_tokens.Create(user));
    }
}
