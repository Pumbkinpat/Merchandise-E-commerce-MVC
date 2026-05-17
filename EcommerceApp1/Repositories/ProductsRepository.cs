using System.Linq.Expressions;
using EcommerceApp1.Data;
using EcommerceApp1.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp1.Repositories;

public class ProductsRepository : IProductsRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ICategoryRepository _categoryRepository;
    
    public ProductsRepository(ApplicationDbContext context, ICategoryRepository categoryRepository)
    {
        _context = context;
        _categoryRepository = categoryRepository;
    }

    private void checkNullTable()
    {
        if (_context.Products == null) throw new Exception("Entity set 'EcommerceApp1.Product'  is null.");
    }
    
    public List<Product> GetAll()
    {
        checkNullTable();
        return _context.Products.ToList();
    }
    
    public Product GetById(int id = -1)
    {
        checkNullTable();
        Product product = null;

        if (!(id < 0))
        {
            try
            {
                product = _context.Products.Single(p => p.ProductId == id);
                Console.WriteLine($"======================\n" +
                                  $"Name: {product.Name}\n" +
                                  $"======================\n");
            }
            catch (Exception)
            {
                Console.WriteLine("there is a problem:");
            }
        }
        else throw new Exception("Invalid product ID");
        Console.WriteLine($"======================\n" +
                          $"Name: {product.Name}\n" +
                          $"======================\n");
        return product;
    }
    
    [HttpGet]
    public List<Product> GetBySearch(string searchString)
    {
        checkNullTable();

        List<Product> products = GetAll();

        if (!string.IsNullOrEmpty(searchString))
        {
            products = products.Where(p => p.Name!.ToUpper().Contains(searchString.ToUpper())).ToList();
        }
        else
        {
            Console.WriteLine("No search string");
        }

        foreach (var p in products)
        {
            Console.WriteLine($"======================\n" +
                              $"Name: {p.Name!.ToUpper()} - {searchString.ToUpper()}\n" +
                              $"======================\n");
        }
        
        return products;
    }

    // Get products by category
    public List<Product> GetByCategory(string _category, string searchString, decimal minPrice, decimal maxPrice)
    {
        Console.WriteLine($"======================\n" +
                          $"Inside GetByCategory\n" +
                          $"_category: {_category} - searchString: {searchString} - minPrice: {minPrice} - maxPrice: {maxPrice}\n" +
                          $"======================\n");
        
        var category = _categoryRepository.GetByName(_category).FirstOrDefault();
        var products = GetBySearch(searchString);
        List<Product> productQuery = products;
        
        if (!string.IsNullOrEmpty(_category))
        {
            productQuery = products.Where(p => p.CategoryId == category.CategoryId).ToList();
        }
        
        productQuery = productQuery.Where(p => p.Price >= minPrice).ToList();
        productQuery = productQuery.Where(p => p.Price <= maxPrice).ToList();
        
        return productQuery;
    }
    
    // Get products with pagination
    public List<Product> GetPaged(int pageIndex, int pageSize, List<Product> products, decimal minPrice, decimal maxPrice)
    {
        Console.WriteLine($"======================\n" +
                          $"Inside GetByCategory\n" +
                          $"products: {products} - minPrice: {minPrice} - maxPrice: {maxPrice}\n" +
                          $"======================\n");
        var pagedProducts = products.Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return pagedProducts;
    }

    public void Add(Product product)
    {
        _context.Add(product);
    }

    public void Update(Product product)
    {
        throw new NotImplementedException();
    }

    public void Delete(int productId)
    {
        var product = GetById(productId);
        Console.WriteLine($"======================\n" +
                          $"Inside GetByCategory\n" +
                          $"products name: {product.Name}\n" +
                          $"======================\n");
        _context.Remove(product);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}

// Its late in my area now, I will come back to this later, thanks you for watching my stream£