namespace TriangleLab.Models;

public class BulkProductPrice
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public double Price { get; set; }
}

public class BulkProductPrices
{
    public ProductType Type { get; set; }
    public List<BulkProductPrice> Products { get; set; } = new();
}
