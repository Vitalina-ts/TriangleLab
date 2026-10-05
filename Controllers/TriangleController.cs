using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using TriangleLab.Models;

namespace TriangleLab.Controllers;

public class TriangleController : Controller
{
    private IActionResult InvalidTriangle()
    {
        return View("Error");
    }

    private static bool IsTriangleValid(Triangle? triangle)
    {
        return triangle != null && triangle.IsValid();
    }

    public IActionResult Info(double side1, double side2, double side3)
    {
        var triangle = new Triangle(side1, side2, side3);

        if (!IsTriangleValid(triangle))
        {
            return InvalidTriangle();
        }

        return View(triangle);
    }

    public IActionResult Area(double side1, double side2, double side3)
    {
        var triangle = new Triangle(side1, side2, side3);

        if (!IsTriangleValid(triangle))
        {
            return InvalidTriangle();
        }

        var area = triangle.GetArea();
        return Content(area.ToString("0.######", CultureInfo.InvariantCulture));
    }

    public IActionResult Perimeter(double side1, double side2, double side3)
    {
        var triangle = new Triangle(side1, side2, side3);

        if (!IsTriangleValid(triangle))
        {
            return InvalidTriangle();
        }

        var perimeter = triangle.GetPerimeter();
        return Content(perimeter.ToString("0.######", CultureInfo.InvariantCulture));
    }

    public IActionResult IsRightAngled(double side1, double side2, double side3)
    {
        var triangle = new Triangle(side1, side2, side3);

        if (!IsTriangleValid(triangle))
        {
            return InvalidTriangle();
        }

        return Content(triangle.IsRightAngled().ToString().ToLowerInvariant());
    }

    public IActionResult IsEquilateral(double side1, double side2, double side3)
    {
        var triangle = new Triangle(side1, side2, side3);

        if (!IsTriangleValid(triangle))
        {
            return InvalidTriangle();
        }

        return Content(triangle.IsEquilateral().ToString().ToLowerInvariant());
    }

    public IActionResult IsIsosceles(double side1, double side2, double side3)
    {
        var triangle = new Triangle(side1, side2, side3);

        if (!IsTriangleValid(triangle))
        {
            return InvalidTriangle();
        }

        return Content(triangle.IsIsosceles().ToString().ToLowerInvariant());
    }

    public IActionResult AreCongruent(Triangle tr1, Triangle tr2)
    {
        if (!IsTriangleValid(tr1) || !IsTriangleValid(tr2))
        {
            return InvalidTriangle();
        }

        return Content(tr1.IsCongruentTo(tr2).ToString().ToLowerInvariant());
    }

    public IActionResult AreSimilar(Triangle tr1, Triangle tr2)
    {
        if (!IsTriangleValid(tr1) || !IsTriangleValid(tr2))
        {
            return InvalidTriangle();
        }

        return Content(tr1.IsSimilarTo(tr2).ToString().ToLowerInvariant());
    }

    public IActionResult GreatesByPerimeter(Triangle[] tr)
    {
        return GreatestByPerimeter(tr);
    }

    public IActionResult GreatestByPerimeter(Triangle[] tr)
    {
        if (tr == null || tr.Length == 0)
        {
            return InvalidTriangle();
        }

        foreach (var triangle in tr)
        {
            if (!IsTriangleValid(triangle))
            {
                return InvalidTriangle();
            }
        }

        var bestTriangle = tr[0];

        for (var i = 1; i < tr.Length; i++)
        {
            if (tr[i].GetPerimeter() > bestTriangle.GetPerimeter())
            {
                bestTriangle = tr[i];
            }
        }

        return View("TriangleList", new List<Triangle> { bestTriangle });
    }

    public IActionResult GreatestByArea(Triangle[] tr)
    {
        if (tr == null || tr.Length == 0)
        {
            return InvalidTriangle();
        }

        foreach (var triangle in tr)
        {
            if (!IsTriangleValid(triangle))
            {
                return InvalidTriangle();
            }
        }

        var bestTriangle = tr[0];

        for (var i = 1; i < tr.Length; i++)
        {
            if (tr[i].GetArea() > bestTriangle.GetArea())
            {
                bestTriangle = tr[i];
            }
        }

        return View("TriangleList", new List<Triangle> { bestTriangle });
    }

    public IActionResult PairwiseNonSimilar(Triangle[] tr)
    {
        if (tr == null || tr.Length == 0)
        {
            return InvalidTriangle();
        }

        foreach (var triangle in tr)
        {
            if (!IsTriangleValid(triangle))
            {
                return InvalidTriangle();
            }
        }

        var result = new List<Triangle>();

        foreach (var triangle in tr)
        {
            var isSimilarToExisting = false;

            foreach (var selectedTriangle in result)
            {
                if (triangle.IsSimilarTo(selectedTriangle))
                {
                    isSimilarToExisting = true;
                    break;
                }
            }

            if (!isSimilarToExisting)
            {
                result.Add(triangle);
            }
        }

        return View("TriangleList", result);
    }
}
