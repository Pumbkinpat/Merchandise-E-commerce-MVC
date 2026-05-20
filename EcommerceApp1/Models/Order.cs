using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApp1.Models;

[Table("Order")]
public class Order
{
    [Key]
    public int OrderId { get; set; }

    [Required]
    public string UserId { get; set; }     

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public double TotalPrice { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; }

    // Shipping details
    [MaxLength(50)]
    public string FirstName { get; set; }

    [MaxLength(50)]
    public string LastName { get; set; }

    [MaxLength(100)]
    public string Email { get; set; }

    [MaxLength(200)]
    public string Address { get; set; }

    [MaxLength(100)]
    public string City { get; set; }

    [MaxLength(100)]
    public string State { get; set; }

    [MaxLength(20)]
    public string Zip { get; set; }

    public List<OrderItem> OrderItems { get; set; }
}