using EcommerceApp1.Models;
using EcommerceApp1.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp1.Controllers;

public class ProductsController : Controller
{
    public class ProductDisplayModel
    {
        public List<Product> Products { get; set; }
        public string SearchString { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
    
    private readonly IProductsRepository _productsRepository;
    private const int _minPrice = 0;
    private const int _maxPrice = 500;
    private const int _pageSize = 6; // Number of products per page
    private ShoppingCart _shoppingCart; 
    
    public ProductsController(IProductsRepository productsRepository)
    {
        _productsRepository = productsRepository;
        _shoppingCart = new ShoppingCart();
    }
    
    // GET
    public IActionResult Index(string searchString = "")
    {
        var products = _productsRepository.GetAll();
        var pagedProducts = _productsRepository.GetPaged(1, _pageSize, products, _minPrice, _maxPrice);
        var totalPages = (int)Math.Ceiling((decimal)products.Count / _pageSize);

        var productDisplayModel = new ProductDisplayModel
        {
            Products = pagedProducts,
            SearchString = (searchString.Length > 0)? searchString : "",
            CurrentPage = 1,
            TotalPages = totalPages,
            Category = null,
            MinPrice = _minPrice,
            MaxPrice = _maxPrice
        };
        
        return View(productDisplayModel);
    }

    public IActionResult Search(string searchString = "")
    {
        
        var searchResult = _productsRepository.GetBySearch(searchString);
        var pagedProducts = _productsRepository.GetPaged(1, _pageSize, searchResult, _minPrice, _maxPrice);
        var totalPages = (int)Math.Ceiling((decimal)searchResult.Count / _pageSize);
        
        var productDisplayModel = new ProductDisplayModel
        {
            Products = pagedProducts,
            SearchString = (searchString.Length > 0)? searchString : "",
            CurrentPage = 1,
            TotalPages = totalPages,
            Category = null,
            MinPrice = _minPrice,
            MaxPrice = _maxPrice
        };
        
        Console.WriteLine($"======================\n" +
                          $"Search Count: {searchResult.Count}\n" +
                          $"======================");
        
        return View("Index", productDisplayModel);
    }

    public IActionResult Category(string category = "", string searchString = "", decimal minPrice = _minPrice, decimal maxPrice = _maxPrice,
        int page = 1)
    {
        var products = _productsRepository.GetByCategory(category, searchString, minPrice, maxPrice);
        var pagedProducts = _productsRepository.GetPaged(page, _pageSize, products, minPrice, maxPrice);
        var totalPages = (int)Math.Ceiling((decimal)products.Count / _pageSize);
        
        var productDisplayModel = new ProductDisplayModel
        {
            Products = pagedProducts,
            CurrentPage = page,
            TotalPages = totalPages,
            Category = category,
            MinPrice = minPrice,
            MaxPrice = maxPrice
        };

        return View("Index", productDisplayModel);
    }
    
    public IActionResult ProductDetail()
    {
        return View();
    }
    
    public IActionResult ProductList()
    {
        var products = _productsRepository.GetAll();
        return View("ProductOperations", products);
    }
}