using Microsoft.AspNetCore.Mvc;
using TriangleLab.Models;
using TriangleLab.Services;

namespace TriangleLab.Controllers;

public class ProductsController : Controller
{
    private readonly ProductStore store;
    public ProductsController(ProductStore store) => this.store = store;

    [HttpGet("/products")]
    [HttpGet("/products/index")]
    [HttpGet("/items")]
    [HttpGet("/items/index")]
    public IActionResult Index(string? name, double? minPrice, double? maxPrice)
    {
        var products = store.All().AsEnumerable();
        if (!string.IsNullOrWhiteSpace(name))
            products = products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        if (minPrice.HasValue) products = products.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue) products = products.Where(p => p.Price <= maxPrice.Value);
        ViewBag.Name = name;
        ViewBag.MinPrice = minPrice;
        ViewBag.MaxPrice = maxPrice;
        return View(products.ToList());
    }

    [HttpGet("/products/{id:int}")]
    public IActionResult Details(int id)
    {
        var product = store.Find(id);
        return product == null ? MissingProduct() : View(product);
    }

    [HttpGet("/products/create")]
    [HttpGet("/products/new")]
    public IActionResult Create() => View(new Product());

    [HttpPost("/products/create")]
    [HttpPost("/products/new")]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("Name,Price,Description,Type")] Product product)
    {
        ValidatePrice(product);
        ValidateType(product);
        if (!ModelState.IsValid) return View(product);
        product.Name = product.Name.Trim();
        store.Add(product);
        return RedirectToAction(nameof(Details), new { id = product.Id });
    }

    [HttpGet("/products/edit/{id:int}")]
    public IActionResult Edit(int id)
    {
        var product = store.Find(id);
        return product == null ? MissingProduct() : View(product);
    }

    [HttpPost("/products/edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public IActionResult Edit([FromRoute] int id, [Bind("Name,Price,Description,Type")] Product product)
    {
        if (store.Find(id) == null) return MissingProduct();
        product.Id = id;
        ValidatePrice(product);
        ValidateType(product);
        if (!ModelState.IsValid) return View(product);
        product.Name = product.Name.Trim();
        if (!store.Update(id, product)) return MissingProduct();
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("/products/delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        var product = store.Find(id);
        return product == null ? MissingProduct() : View(product);
    }

    [HttpPost("/products/delete/{id:int}")]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public IActionResult DeleteConfirmed([FromRoute] int id)
    {
        if (!store.Delete(id)) return MissingProduct();
        return RedirectToAction(nameof(Index));
    }

    private void ValidatePrice(Product product)
    {
        if (!double.IsFinite(product.Price))
            ModelState.AddModelError(nameof(product.Price), "Enter a finite price from 0 to 100000.");
    }

    private void ValidateType(Product product)
    {
        if (!Enum.IsDefined(typeof(ProductType), product.Type))
            ModelState.AddModelError(nameof(product.Type), "Select a valid product type.");
    }

    [HttpGet("/products/by-type/{type}")]
    public IActionResult EditPrices(string type)
    {
        if (!Enum.TryParse<ProductType>(type, true, out var productType) ||
            !Enum.IsDefined(typeof(ProductType), productType))
            return NotFound();

        var model = new BulkProductPrices
        {
            Type = productType,
            Products = store.All()
                .Where(p => p.Type == productType)
                .Select(p => new BulkProductPrice { Id = p.Id, Name = p.Name, Price = p.Price })
                .ToList()
        };
        return View(model);
    }

    [HttpPost("/products/by-type/{type}")]
    [ValidateAntiForgeryToken]
    public IActionResult EditPrices(string type, [Bind("Products")] BulkProductPrices model)
    {
        if (!Enum.TryParse<ProductType>(type, true, out var productType) ||
            !Enum.IsDefined(typeof(ProductType), productType))
            return NotFound();

        model.Type = productType;
        var categoryProducts = store.All().Where(p => p.Type == productType).ToDictionary(p => p.Id);
        var submittedIds = model.Products.Select(p => p.Id).ToList();
        var categoryIds = categoryProducts.Keys.ToHashSet();

        if (submittedIds.Count != categoryIds.Count ||
            submittedIds.Distinct().Count() != submittedIds.Count ||
            !submittedIds.ToHashSet().SetEquals(categoryIds))
        {
            ModelState.AddModelError(string.Empty, "The submitted products do not exactly match the selected category.");
        }

        foreach (var item in model.Products)
        {
            if (!categoryProducts.TryGetValue(item.Id, out var product))
            {
                ModelState.AddModelError(string.Empty, "A submitted product does not belong to this category.");
                continue;
            }

            item.Name = product.Name;
            if (!double.IsFinite(item.Price) || item.Price < 0 || item.Price > 100000)
                ModelState.AddModelError($"Products[{model.Products.IndexOf(item)}].Price", "Price must be between 0 and 100000.");
        }

        if (!ModelState.IsValid) return View(model);
        if (!store.UpdatePrices(productType, model.Products))
        {
            ModelState.AddModelError(string.Empty, "The product prices could not be saved because the category changed or a product no longer exists.");
            var savedCategoryProducts = store.All().Where(p => p.Type == productType).ToList();
            model.Products = savedCategoryProducts
                .Select(p => new BulkProductPrice { Id = p.Id, Name = p.Name, Price = p.Price })
                .ToList();
            return View(model);
        }
        return RedirectToAction(nameof(Index));
    }

    private IActionResult MissingProduct()
    {
        Response.StatusCode = StatusCodes.Status404NotFound;
        return View("ProductNotFound");
    }
}