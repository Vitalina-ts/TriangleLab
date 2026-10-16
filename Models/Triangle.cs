
using System.Globalization;

namespace TriangleLab.Models;

public class Triangle
{
    public double Side1 { get; set; }
    public double Side2 { get; set; }
    public double Side3 { get; set; }

    // Порожній конструктор
    public Triangle() { }

    // Конструктор із трьома сторонами
    public Triangle(double a, double b, double c)
    {
        Side1 = a;
        Side2 = b;
        Side3 = c;
    }

    // Порівняння чисел із похибкою
    public static bool Equal(double a, double b)
    {
        return Math.Abs(a - b) < 0.00001;
    }

    // Перевірка існування трикутника
    public bool IsValid()
    {
        return Side1 > 0 && Side2 > 0 && Side3 > 0
            && Side1 + Side2 > Side3
            && Side1 + Side3 > Side2
            && Side2 + Side3 > Side1;
    }

    // Периметр
    public double GetPerimeter()
    {
        return Side1 + Side2 + Side3;
    }

    // Площа за формулою Герона
    public double GetArea()
    {
        if (!IsValid()) return 0;

        double p = GetPerimeter() / 2;

        return Math.Sqrt(p * (p - Side1)
            * (p - Side2) * (p - Side3));
    }

    // Сортування сторін
    public double[] GetSortedSides()
    {
        double[] sides = { Side1, Side2, Side3 };
        Array.Sort(sides);
        return sides;
    }

    // Прямокутний трикутник
    public bool IsRightAngled()
    {
        if (!IsValid()) return false;

        var s = GetSortedSides();

        return Equal(s[0] * s[0] + s[1] * s[1],
                     s[2] * s[2]);
    }

    // Рівносторонній трикутник
    public bool IsEquilateral()
    {
        return IsValid()
            && Equal(Side1, Side2)
            && Equal(Side2, Side3);
    }

    // Рівнобедрений трикутник
    public bool IsIsosceles()
    {
        return IsValid()
            && (Equal(Side1, Side2)
            || Equal(Side1, Side3)
            || Equal(Side2, Side3));
    }

    // Рівність двох трикутників
    public bool IsCongruentTo(Triangle other)
    {
        if (other == null || !IsValid() || !other.IsValid())
            return false;

        var a = GetSortedSides();
        var b = other.GetSortedSides();

        return Equal(a[0], b[0])
            && Equal(a[1], b[1])
            && Equal(a[2], b[2]);
    }

    // Подібність двох трикутників
    public bool IsSimilarTo(Triangle other)
    {
        if (other == null || !IsValid() || !other.IsValid())
            return false;

        var a = GetSortedSides();
        var b = other.GetSortedSides();

        double k = a[0] / b[0];

        return Equal(a[1] / b[1], k)
            && Equal(a[2] / b[2], k);
    }

    // Текстова інформація про трикутник
    public string GetInfoText()
    {
        var s = GetSortedSides();
        double p = GetPerimeter();

        return $"Triangle: ({s[0]}, {s[1]}, {s[2]})\n"
            + $"Reduced: ({s[0] / p:F3}, {s[1] / p:F3}, {s[2] / p:F3})\n"
            + $"Area = {GetArea():F3}\n"
            + $"Perimeter = {p:F3}";
    }
}
