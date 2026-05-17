using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApp1.Models;

[Table("Product")]
public class Product
{
    [Key]
    public int ProductId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    [MaxLength(40)]
    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public DateTime DateCreated { get; set; }

    public string ImageUrl { get; set; } = "https://picsum.photos/300/300";

    [ForeignKey("CategoryId")]
    public Category Category { get; set; }
    
    public List<OrderItem> OrderItems { get; set; }
    
    public List<CartItem> CartItems { get; set; }
}