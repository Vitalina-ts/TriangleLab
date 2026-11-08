using Microsoft.AspNetCore.Mvc;
using TriangleLab.Models;

namespace TriangleLab.Controllers;

public class TasksController : Controller
{
    private static List<string> Supermarkets() => new() { "WellMart", "Silpo", "ATB", "Furshet", "Metro" };

    private static Dictionary<string, int> Products() => new()
    {
        { "Milk", 2 }, { "Bread", 2 }, { "Cake", 1 }, { "Ice Cream", 5 }, { "Cola", 10 }
    };

    public IActionResult SprintTasks() => View();
    public IActionResult Greetings() => View();
    public IActionResult ProductInfo() => View();

    public IActionResult SuperMarkets()
    {
        ViewBag.Supermarkets = Supermarkets();
        return View();
    }

    public IActionResult ShoppingList()
    {
        var dictionary = Products();
        return View(dictionary);
    }

    [HttpGet]
    public IActionResult ShoppingCart()
    {
        SetChoices();
        return View(new ShoppingCartModel { DeliveryDate = Today() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ShoppingCart(ShoppingCartModel model)
    {
        var today = Today();
        if (!Supermarkets().Contains(model.Supermarket))
            ModelState.AddModelError(nameof(model.Supermarket), "Choose a supermarket.");
        if (model.DeliveryDate.TimeOfDay != TimeSpan.Zero || model.DeliveryDate < today || model.DeliveryDate > today.AddDays(2))
            ModelState.AddModelError(nameof(model.DeliveryDate), "Choose today, tomorrow or the day after tomorrow.");
        if (model.Products.Length == 0 || model.Products.Any(p => !Products().ContainsKey(p)))
            ModelState.AddModelError(nameof(model.Products), "Choose products from the list.");

        SetChoices();
        if (ModelState.IsValid)
        {
            model.FullName = model.FullName.Trim();
            model.Address = model.Address.Trim();
            ViewBag.Sent = true;
        }
        return View(model);
    }

    private static DateTime Today() => TimeZoneInfo.ConvertTimeFromUtc(
        DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Europe/Kyiv")).Date;

    private void SetChoices()
    {
        ViewBag.Supermarkets = Supermarkets();
        ViewBag.Products = Products().Keys.ToList();
        ViewBag.Today = Today();
    }
}