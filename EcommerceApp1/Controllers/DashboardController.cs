using EcommerceApp1.Models;
using EcommerceApp1.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp1.Controllers;

public class DashboardController : Controller
{
    private readonly IProductsRepository _productsRepository;
    
    public DashboardController(IProductsRepository productsRepository)
    {
        _productsRepository = productsRepository;
    }
    
    public IActionResult Index()
    {
        var products = _productsRepository.GetAll();
        return View(products);
    }

    public IActionResult AddProduct()
    {
        if (ModelState.IsValid) {}
        return View();
    }

    [HttpPost]
    public IActionResult SaveProduct(string name, int categoryId, string description, decimal price, 
        int stockQuantity, string imageUrl)
    {
        var product = new Product
        {
            Name = name,
            CategoryId = categoryId,
            Description = description,
            Price = price,
            StockQuantity = stockQuantity,
            DateCreated = DateTime.Now,
            ImageUrl = imageUrl
        };
        
        _productsRepository.Add(product);
        _productsRepository.SaveChanges();
        
        return RedirectToAction("Index");
    }
    
    [HttpDelete("[controller]/[action]")]
    public IActionResult DeleteProduct(int productId)
    {
        _productsRepository.Delete(productId);
        _productsRepository.SaveChanges();
        
        return Redirect("Dashboard/Index");
    }
}