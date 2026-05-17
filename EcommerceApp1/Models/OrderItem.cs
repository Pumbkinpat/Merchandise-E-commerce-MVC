using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApp1.Models;

[Table("OrderItem")]
public class OrderItem
{
    [Key]
    public int OrderItemId { get; set; }
    
    [Required]
    public int OrderId { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    
    public int Quantity { get; set; }
    
    public double Price { get; set; }
    
    [ForeignKey("OrderId")]
    public Order Order { get; set; }
    
    [ForeignKey("ProductId")]
    public Product Product { get; set; }
}