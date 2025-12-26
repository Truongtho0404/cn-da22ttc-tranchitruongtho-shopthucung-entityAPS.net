using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.Models;

namespace PetShop.Web.Controllers;

public class OrdersController : Controller
{
    private readonly PetShopDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OrdersController(PetShopDbContext context, IHttpContextAccessor httpContextAccessor)
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

    // GET: Orders
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var orders = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Pet)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }

    // GET: Orders/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Pet)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(m => m.OrderId == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // GET: Orders/Checkout
    public async Task<IActionResult> Checkout()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Pet)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null || !cart.CartItems.Any())
        {
            TempData["Error"] = "Giỏ hàng trống";
            return RedirectToAction("Index", "Cart");
        }

        var user = await _context.Users.FindAsync(userId);
        ViewBag.User = user;

        return View(cart);
    }

    // POST: Orders/PlaceOrder
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(string shippingAddress, string phone, string note, string paymentMethod)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Pet)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null || !cart.CartItems.Any())
        {
            TempData["Error"] = "Giỏ hàng trống";
            return RedirectToAction("Index", "Cart");
        }

        // Create order
        var order = new Order
        {
            UserId = userId.Value,
            OrderDate = DateTime.Now,
            ShippingAddress = shippingAddress,
            Phone = phone,
            Note = note,
            Status = "Pending",
            TotalAmount = cart.CartItems.Sum(ci => ci.Pet!.Price * ci.Quantity)
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Create order details
        foreach (var cartItem in cart.CartItems)
        {
            var orderDetail = new OrderDetail
            {
                OrderId = order.OrderId,
                PetId = cartItem.PetId,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.Pet!.Price,
                TotalPrice = cartItem.Pet.Price * cartItem.Quantity
            };

            _context.OrderDetails.Add(orderDetail);

            // Update stock
            var pet = await _context.Pets.FindAsync(cartItem.PetId);
            if (pet != null)
            {
                pet.StockQuantity -= cartItem.Quantity;
                if (pet.StockQuantity == 0)
                {
                    pet.IsAvailable = false;
                }
            }
        }

        // Create payment
        var payment = new Payment
        {
            OrderId = order.OrderId,
            PaymentMethod = paymentMethod,
            Amount = order.TotalAmount,
            Status = "Pending",
            PaymentDate = DateTime.Now,
            TransactionId = paymentMethod != "COD" ? $"TXN{DateTime.Now:yyyyMMddHHmmss}" : null
        };

        _context.Payments.Add(payment);

        // Clear cart
        _context.CartItems.RemoveRange(cart.CartItems);

        await _context.SaveChangesAsync();

        TempData["Success"] = "Đặt hàng thành công!";
        return RedirectToAction(nameof(Details), new { id = order.OrderId });
    }
}
