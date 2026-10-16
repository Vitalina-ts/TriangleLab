
using Microsoft.AspNetCore.Mvc;
using TriangleLab.Models;

namespace TriangleLab.Controllers;

public class TriangleController : Controller
{
    // Створення трикутника
    private Triangle Create(double a, double b, double c)
    {
        return new Triangle(a, b, c);
    }

    // Сторінка помилки
    private IActionResult Error()
    {
        return View("Error");
    }

    // Інформація про трикутник
    public IActionResult Info(double side1, double side2, double side3)
    {
        var t = Create(side1, side2, side3);

        if (!t.IsValid()) return Error();

        return View(t);
    }

    // Площа
    public IActionResult Area(double side1, double side2, double side3)
    {
        var t = Create(side1, side2, side3);

        if (!t.IsValid()) return Error();

        return Content(t.GetArea().ToString());
    }

    // Периметр
    public IActionResult Perimeter(double side1, double side2, double side3)
    {
        var t = Create(side1, side2, side3);

        if (!t.IsValid()) return Error();

        return Content(t.GetPerimeter().ToString());
    }

    // Чи прямокутний
    public IActionResult IsRightAngled(double side1, double side2, double side3)
    {
        var t = Create(side1, side2, side3);

        if (!t.IsValid()) return Error();

        return Content(t.IsRightAngled().ToString().ToLowerInvariant());
    }

    // Чи рівносторонній
    public IActionResult IsEquilateral(double side1, double side2, double side3)
    {
        var t = Create(side1, side2, side3);

        if (!t.IsValid()) return Error();

        return Content(t.IsEquilateral().ToString().ToLowerInvariant());
    }

    // Чи рівнобедрений
    public IActionResult IsIsosceles(double side1, double side2, double side3)
    {
        var t = Create(side1, side2, side3);

        if (!t.IsValid()) return Error();

        return Content(t.IsIsosceles().ToString().ToLowerInvariant());
    }

    // Чи рівні трикутники
    public IActionResult AreCongruent(Triangle tr1, Triangle tr2)
    {
        if (tr1 == null || tr2 == null ||
            !tr1.IsValid() || !tr2.IsValid())
            return Error();

        return Content(tr1.IsCongruentTo(tr2).ToString().ToLowerInvariant());
    }

    // Чи подібні трикутники
    public IActionResult AreSimilar(Triangle tr1, Triangle tr2)
    {
        if (tr1 == null || tr2 == null ||
            !tr1.IsValid() || !tr2.IsValid())
            return Error();

        return Content(tr1.IsSimilarTo(tr2).ToString().ToLowerInvariant());
    }

    // Найбільший за периметром
    public IActionResult GreatestByPerimeter(Triangle[] tr)
    {
        if (tr == null || tr.Length == 0 ||
            tr.Any(t => t == null || !t.IsValid()))
            return Error();

        var best = tr.MaxBy(t => t.GetPerimeter());

        return View("TriangleList", new List<Triangle> { best! });
    }

    // Найбільший за площею
    public IActionResult GreatestByArea(Triangle[] tr)
    {
        if (tr == null || tr.Length == 0 ||
            tr.Any(t => t == null || !t.IsValid()))
            return Error();

        var best = tr.MaxBy(t => t.GetArea());

        return View("TriangleList", new List<Triangle> { best! });
    }

    // Список попарно неподібних трикутників
    public IActionResult PairwiseNonSimilar(Triangle[] tr)
    {
        if (tr == null || tr.Length == 0 ||
            tr.Any(t => t == null || !t.IsValid()))
            return Error();

        var result = new List<Triangle>();

        foreach (var t in tr)
        {
            if (!result.Any(x => x.IsSimilarTo(t)))
                result.Add(t);
        }

        return View("TriangleList", result);
    }
}
