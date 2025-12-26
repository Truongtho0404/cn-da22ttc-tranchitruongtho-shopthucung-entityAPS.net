using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetShop.Models;

[Table("Pets")]
public class Pet
{
    [Key]
    public int PetId { get; set; }

    [Required]
    [StringLength(200)]
    public string PetName { get; set; } = string.Empty;

    [ForeignKey("Category")]
    public int CategoryId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [StringLength(100)]
    public string? Breed { get; set; }

    public int Age { get; set; }

    [StringLength(50)]
    public string? Gender { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public int StockQuantity { get; set; }

    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual Category? Category { get; set; }
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<PetImage> PetImages { get; set; } = new List<PetImage>();
}
