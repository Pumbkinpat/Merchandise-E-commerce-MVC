using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApp1.Models;

[Table("Category")]
public class Category
{
    [Key]
    public int CategoryId { get; set; }
    
    [Required]
    [MaxLength(40)]
    public string Name { get; set; }
    
    public List<Product> Products { get; set; }
}