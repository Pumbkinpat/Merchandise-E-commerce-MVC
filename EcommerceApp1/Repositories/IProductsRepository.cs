using EcommerceApp1.Models;

namespace EcommerceApp1.Repositories;

public interface IProductsRepository
{
    List<Product> GetAll();
    Product GetById(int id);
    public List<Product> GetBySearch(string searchTerm);
    public List<Product> GetByCategory(string _category, string searchString, decimal minPrice, decimal maxPrice);
    public List<Product> GetPaged(int pageIndex, int pageSize, List<Product> products, decimal minPrice, decimal maxPrice);
    void Add(Product product);
    void Update(Product product);
    void Delete(int productId);
    void SaveChanges();
}