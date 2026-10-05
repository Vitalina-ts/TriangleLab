using System.Globalization;

namespace TriangleLab.Models;

public class Triangle
{
    public double Side1 { get; set; }
    public double Side2 { get; set; }
    public double Side3 { get; set; }

    public Triangle()
    {
    }

    public Triangle(double side1, double side2, double side3)
    {
        Side1 = side1;
        Side2 = side2;
        Side3 = side3;
    }

    public static bool ApproximatelyEqual(double left, double right)
    {
        var difference = Math.Abs(left - right);

        if (Math.Abs(left) < 1e-12 && Math.Abs(right) < 1e-12)
        {
            return true;
        }

        if (Math.Abs(left) < 1e-12)
        {
            return difference < 1e-8;
        }

        var tolerance = Math.Abs(left) * 0.00001;
        return difference < tolerance;
    }

    public bool IsValid()
    {
        if (Side1 <= 0 || Side2 <= 0 || Side3 <= 0)
        {
            return false;
        }

        return (Side1 + Side2 > Side3)
            && (Side1 + Side3 > Side2)
            && (Side2 + Side3 > Side1);
    }

    public double GetPerimeter()
    {
        return Side1 + Side2 + Side3;
    }

    public double GetArea()
    {
        if (!IsValid())
        {
            return 0;
        }

        var semiPerimeter = GetPerimeter() / 2.0;
        var area = Math.Sqrt(
            semiPerimeter
            * (semiPerimeter - Side1)
            * (semiPerimeter - Side2)
            * (semiPerimeter - Side3));

        return area;
    }

    public double[] GetSortedSides()
    {
        var sides = new[] { Side1, Side2, Side3 };
        Array.Sort(sides);
        return sides;
    }

    public bool IsRightAngled()
    {
        if (!IsValid())
        {
            return false;
        }

        var sides = GetSortedSides();
        var a = sides[0];
        var b = sides[1];
        var c = sides[2];

        return ApproximatelyEqual(a * a + b * b, c * c);
    }

    public bool IsEquilateral()
    {
        if (!IsValid())
        {
            return false;
        }

        return ApproximatelyEqual(Side1, Side2)
            && ApproximatelyEqual(Side2, Side3);
    }

    public bool IsIsosceles()
    {
        if (!IsValid())
        {
            return false;
        }

        if (IsEquilateral())
        {
            return true;
        }

        return ApproximatelyEqual(Side1, Side2)
            || ApproximatelyEqual(Side1, Side3)
            || ApproximatelyEqual(Side2, Side3);
    }

    public bool IsCongruentTo(Triangle other)
    {
        if (other == null || !IsValid() || !other.IsValid())
        {
            return false;
        }

        var first = GetSortedSides();
        var second = other.GetSortedSides();

        return ApproximatelyEqual(first[0], second[0])
            && ApproximatelyEqual(first[1], second[1])
            && ApproximatelyEqual(first[2], second[2]);
    }

    public bool IsSimilarTo(Triangle other)
    {
        if (other == null || !IsValid() || !other.IsValid())
        {
            return false;
        }

        var first = GetSortedSides();
        var second = other.GetSortedSides();

        return AreRatiosApproximatelyEqual(first[0], second[0], first[1], second[1])
            && AreRatiosApproximatelyEqual(first[1], second[1], first[2], second[2])
            && AreRatiosApproximatelyEqual(first[0], second[0], first[2], second[2]);
    }

    private static bool AreRatiosApproximatelyEqual(double leftA, double leftB, double rightA, double rightB)
    {
        if (ApproximatelyEqual(leftB, 0) || ApproximatelyEqual(rightB, 0))
        {
            return false;
        }

        return ApproximatelyEqual(leftA * rightB, leftB * rightA);
    }

    public string GetInfoText()
    {
        var sortedSides = GetSortedSides();
        var perimeter = GetPerimeter();
        var area = GetArea();

        var reducedFirst = FormatValue(sortedSides[0] / perimeter);
        var reducedSecond = FormatValue(sortedSides[1] / perimeter);
        var reducedThird = FormatValue(sortedSides[2] / perimeter);

        var result = string.Join(
            Environment.NewLine,
            "------------------------------------------------------------",
            "Triangle:",
            $"({FormatValue(sortedSides[0])}, {FormatValue(sortedSides[1])}, {FormatValue(sortedSides[2])})",
            "Reduced:",
            $"({FormatValue(sortedSides[0])}/{FormatValue(perimeter)}, {FormatValue(sortedSides[1])}/{FormatValue(perimeter)}, {FormatValue(sortedSides[2])}/{FormatValue(perimeter)})",
            $"Area = {FormatValue(area)}",
            $"Perimeter = {FormatValue(perimeter)}",
            "------------------------------------------------------------");

        return result;
    }

    private static string FormatValue(double value)
    {
        return value.ToString("0.######", CultureInfo.InvariantCulture);
    }
}
