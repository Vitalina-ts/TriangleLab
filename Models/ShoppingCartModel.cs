using System.ComponentModel.DataAnnotations;

namespace TriangleLab.Models;

public class ShoppingCartModel
{
    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string Address { get; set; } = string.Empty;

    [Required]
    public string Supermarket { get; set; } = string.Empty;

    public DateTime DeliveryDate { get; set; }
    public string[] Products { get; set; } = Array.Empty<string>();
}