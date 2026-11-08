using TriangleLab.Models;

namespace TriangleLab.Services;

public class ProductStore
{
    private readonly object gate = new();
    private readonly List<Product> products = new()
    {
        new() { Id = 1, Name = "Bread", Price = 10, Type = ProductType.Clothes },
        new() { Id = 2, Name = "Milk", Price = 11, Type = ProductType.Transport },
        new() { Id = 3, Name = "Cheese", Price = 140, Type = ProductType.Technique },
        new() { Id = 4, Name = "Sausage", Price = 110, Type = ProductType.Clothes },
        new() { Id = 5, Name = "Potato", Price = 7, Type = ProductType.Clothes },
        new() { Id = 6, Name = "Banana", Price = 23, Type = ProductType.Toy },
        new() { Id = 7, Name = "Tomato", Price = 25, Type = ProductType.Clothes },
        new() { Id = 8, Name = "Candy", Price = 75, Type = ProductType.Toy }
    };
    private int nextId = 9;

    // Return copies so a request cannot change stored data before validation.
    private static Product Copy(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price,
        Description = p.Description,
        Type = p.Type
    };

    public List<Product> All()
    {
        lock (gate) return products.Select(Copy).ToList();
    }

    public Product? Find(int id)
    {
        lock (gate)
        {
            var product = products.Find(p => p.Id == id);
            return product == null ? null : Copy(product);
        }
    }

    public void Add(Product product)
    {
        lock (gate)
        {
            product.Id = nextId++;
            products.Add(Copy(product));
        }
    }

    public bool Update(int id, Product product)
    {
        lock (gate)
        {
            var stored = products.Find(p => p.Id == id);
            if (stored == null) return false;
            stored.Name = product.Name;
            stored.Price = product.Price;
            stored.Description = product.Description;
            stored.Type = product.Type;
            return true;
        }
    }

    public bool UpdatePrices(ProductType type, IEnumerable<BulkProductPrice> prices)
    {
        lock (gate)
        {
            var changes = prices.ToList();
            var categoryIds = products.Where(p => p.Type == type).Select(p => p.Id).ToHashSet();
            var changeIds = changes.Select(p => p.Id).ToHashSet();

            if (changes.Count != categoryIds.Count ||
                changes.Count != changeIds.Count ||
                !changeIds.SetEquals(categoryIds))
            {
                return false;
            }

            foreach (var change in changes)
            {
                var stored = products.Find(p => p.Id == change.Id);
                if (stored != null) stored.Price = change.Price;
            }
            return true;
        }
    }

    public bool Delete(int id)
    {
        lock (gate) return products.RemoveAll(p => p.Id == id) > 0;
    }
}