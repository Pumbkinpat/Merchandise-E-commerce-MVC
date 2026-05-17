using EcommerceApp1.Models;

namespace EcommerceApp1.Repositories;

public interface ICategoryRepository
{
    public List<Category> GetAll();
    public Category GetById(int id = -1);
    public List<Category> GetByName(string name);
    public void Add(Category category);
    public void Update(Category category);
    public void Delete(Category category);
    public void SaveChanges();
}