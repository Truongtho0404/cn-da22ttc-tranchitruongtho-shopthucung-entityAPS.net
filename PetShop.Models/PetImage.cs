using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetShop.Models;

[Table("PetImages")]
public class PetImage
{
    [Key]
    public int ImageId { get; set; }

    [Required]
    public int PetId { get; set; }

    [Required]
    [StringLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsDefault { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation property
    [ForeignKey("PetId")]
    public virtual Pet? Pet { get; set; }
}
