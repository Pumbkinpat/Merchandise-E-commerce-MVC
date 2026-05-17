using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApp1.Models;

[Table("ShoppingCart")]
public class ShoppingCart
{
    [Key]
    public int ShoppingCartId { get; set; }
    
    [Required]
    public string UserId { get; set; }
    
    public bool IsDeleted { get; set; } = false;
    
    public List<CartItem> CartItems { get; set; }
}