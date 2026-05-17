using EcommerceApp1.Data;
using EcommerceApp1.Models;

namespace EcommerceApp1.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;
    
    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    private void checkNullTable()
    {
        if (_context.Products == null) throw new Exception("Entity set 'EcommerceApp1.Product'  is null.");
    }
    
    public List<Category> GetAll()
    {
        checkNullTable();
        return _context.Categories.ToList();
    }

    public Category GetById(int id = -1)
    {
        checkNullTable();
        
        if (!(id < 0))
        {
            try
            {
               return _context.Categories.Single(c => c.CategoryId == id);
            }
            catch (InvalidOperationException)
            {
                throw new Exception("Category not found");
            }
        }
        throw new Exception("Invalid category ID");
    }
    
    public List<Category> GetByName(string name = "")
    {
        checkNullTable();
        return _context.Categories.Where(c => c.Name == name).ToList();
    }
    
    public void Add(Category category)
    {
        throw new NotImplementedException();
    }

    public void Update(Category category)
    {
        throw new NotImplementedException();
    }

    public void Delete(Category category)
    {
        throw new NotImplementedException();
    }

    public void SaveChanges()
    {
        throw new NotImplementedException();
    }
}