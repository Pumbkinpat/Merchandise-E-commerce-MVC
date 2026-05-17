using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApp1.Models;

[Table("Order")]
public class Order
{
    [Key]
    public int OrderId { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    
    public double TotalPrice { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Status { get; set; }
    
    public List<OrderItem> OrderItems { get; set; }
}