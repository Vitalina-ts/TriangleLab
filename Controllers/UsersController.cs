using Microsoft.AspNetCore.Mvc;
using TriangleLab.Models;
using TriangleLab.Services;

namespace TriangleLab.Controllers;

public class UsersController : Controller
{
    private readonly UserStore store;
    public UsersController(UserStore store) => this.store = store;

    [HttpGet("/users/index")]
    public IActionResult Index() => AccessDenied();

    [HttpPost("/users/index")]
    public IActionResult Index([FromForm] string? accessCode)
    {
        // Educational example only: a fixed code is not secure authentication.
        if (accessCode != "df2323eoT") return AccessDenied();
        return View(store.All());
    }

    private IActionResult AccessDenied()
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        return View("Index", new List<User>());
    }

    [HttpGet("/users/create")]
    public IActionResult Create([FromQuery] int? id, [FromQuery] string? name, [FromQuery] string? email)
    {
        if (!id.HasValue || id.Value <= 0 || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            return BadRequest("id, name, and email query parameters are required.");

        var user = new User { Id = id.Value, Name = name.Trim(), Email = email.Trim() };
        if (!TryValidateModel(user))
            return View(user);

        return View(user);
    }

    [HttpPost("/users/create")]
    [ValidateAntiForgeryToken]
    public IActionResult Save([FromQuery] int? id, [FromQuery] string? name, [FromQuery] string? email)
    {
        if (!id.HasValue || id.Value <= 0 || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            return BadRequest("id, name, and email query parameters are required.");

        var user = new User { Id = id.Value, Name = name.Trim(), Email = email.Trim() };
        if (!TryValidateModel(user))
            return View("Create", user);

        if (!store.Add(user))
        {
            ModelState.AddModelError(string.Empty, "A user with this ID already exists.");
            return View("Create", user);
        }

        return RedirectToAction(nameof(Details), new { id = user.Id });
    }

    [HttpGet("/users/{id:int}")]
    public IActionResult Details(int id)
    {
        var user = store.Find(id);
        return user == null ? NotFound() : View(user);
    }
}