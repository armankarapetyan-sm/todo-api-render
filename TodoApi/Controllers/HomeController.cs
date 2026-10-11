using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TodoApi.Controllers;

[ApiController]
[Route("")]
[AllowAnonymous]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Redirect("/swagger");
    }
}
