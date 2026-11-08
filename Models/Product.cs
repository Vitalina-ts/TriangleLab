using System.ComponentModel.DataAnnotations;

namespace TriangleLab.Models;

public enum ProductType
{
    Toy,
    Technique,
    Clothes,
    Transport
}

public class Product : IValidatableObject
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 100000)]
    public double Price { get; set; }

    public string? Description { get; set; }

    [EnumDataType(typeof(ProductType))]
    public ProductType Type { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Description == null || Description.Length == 0)
            yield break;

        var description = Description.Trim();
        var name = Name?.Trim() ?? string.Empty;

        if (description.Length <= 2)
            yield return new ValidationResult("Description must be longer than 2 characters.", new[] { nameof(Description) });

        if (!string.IsNullOrEmpty(name) &&
            string.Equals(description, name, StringComparison.OrdinalIgnoreCase))
            yield return new ValidationResult("Description cannot be identical to the product name.", new[] { nameof(Description) });

        if (!string.IsNullOrEmpty(name) &&
            !description.StartsWith(name, StringComparison.OrdinalIgnoreCase))
            yield return new ValidationResult("Description must start with the product name.", new[] { nameof(Description) });
    }
}