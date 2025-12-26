using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetShop.Models;

[Table("CartItems")]
public class CartItem
{
    [Key]
    public int CartItemId { get; set; }

    [ForeignKey("Cart")]
    public int CartId { get; set; }

    [ForeignKey("Pet")]
    public int PetId { get; set; }

    public int Quantity { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual Cart? Cart { get; set; }
    public virtual Pet? Pet { get; set; }
}
