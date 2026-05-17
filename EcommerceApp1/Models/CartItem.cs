using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApp1.Models;

[Table("CartItem")]
public class CartItem
{
    [Key]
    public int CartItemId { get; set; }

    [Required]
    public int ShoppingCartId { get; set; }

    [Required]
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }

    [ForeignKey("ShoppingCartId")]
    public ShoppingCart ShoppingCart { get; set; }
}