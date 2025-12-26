using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.Models;
using System.Text.Json;

namespace PetShop.Web.Controllers;

public class CartController : Controller
{
    private readonly PetShopDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartController(PetShopDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private int? GetCurrentUserId()
    {
        var userIdString = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
        if (int.TryParse(userIdString, out int userId))
        {
            return userId;
        }
        return null;
    }

    // GET: Cart
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            TempData["Error"] = "Vui lòng đăng nhập để xem giỏ hàng";
            return RedirectToAction("Login", "Account");
        }

        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Pet)
            .ThenInclude(p => p!.Category)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId.Value, CartItems = new List<CartItem>() };
        }

        return View(cart);
    }

    // POST: Cart/AddToCart
    [HttpPost]
    public async Task<IActionResult> AddToCart(int petId, int quantity = 1)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Json(new { success = false, message = "Vui lòng đăng nhập" });
        }

        var pet = await _context.Pets.FindAsync(petId);
        if (pet == null || !pet.IsAvailable || pet.StockQuantity < quantity)
        {
            return Json(new { success = false, message = "Sản phẩm không có sẵn" });
        }

        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId.Value };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.PetId == petId);
        if (cartItem == null)
        {
            cartItem = new CartItem
            {
                CartId = cart.CartId,
                PetId = petId,
                Quantity = quantity
            };
            _context.CartItems.Add(cartItem);
        }
        else
        {
            cartItem.Quantity += quantity;
            if (cartItem.Quantity > pet.StockQuantity)
            {
                return Json(new { success = false, message = "Số lượng vượt quá hàng tồn kho" });
            }
        }

        cart.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return Json(new { success = true, message = "Đã thêm vào giỏ hàng" });
    }

    // POST: Cart/UpdateQuantity
    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
    {
        var cartItem = await _context.CartItems
            .Include(ci => ci.Pet)
            .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);

        if (cartItem == null)
        {
            return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
        }

        if (quantity <= 0)
        {
            _context.CartItems.Remove(cartItem);
        }
        else if (quantity > cartItem.Pet!.StockQuantity)
        {
            return Json(new { success = false, message = "Số lượng vượt quá hàng tồn kho" });
        }
        else
        {
            cartItem.Quantity = quantity;
        }

        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    // POST: Cart/RemoveItem
    [HttpPost]
    public async Task<IActionResult> RemoveItem(int cartItemId)
    {
        var cartItem = await _context.CartItems.FindAsync(cartItemId);
        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
