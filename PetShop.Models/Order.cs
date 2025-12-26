using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetShop.Models;

[Table("Orders")]
public class Order
{
    [Key]
    public int OrderId { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Shipping, Completed, Cancelled

    [StringLength(500)]
    public string? ShippingAddress { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(1000)]
    public string? Note { get; set; }

    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public virtual Payment? Payment { get; set; }
}
