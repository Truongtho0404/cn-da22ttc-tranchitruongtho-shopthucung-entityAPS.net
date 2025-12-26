using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetShop.Models;

[Table("Payments")]
public class Payment
{
    [Key]
    public int PaymentId { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }

    [Required]
    [StringLength(50)]
    public string PaymentMethod { get; set; } = "COD"; // COD, Bank Transfer, MoMo, ZaloPay, VNPay

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Completed, Failed

    public DateTime PaymentDate { get; set; } = DateTime.Now;

    [StringLength(500)]
    public string? TransactionId { get; set; }

    // Navigation properties
    public virtual Order? Order { get; set; }
}
