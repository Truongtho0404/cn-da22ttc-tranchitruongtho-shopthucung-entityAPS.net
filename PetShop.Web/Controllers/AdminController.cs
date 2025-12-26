using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.Models;

namespace PetShop.Web.Controllers;

public class AdminController : Controller
{
    private readonly PetShopDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AdminController(PetShopDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private bool IsAdmin()
    {
        var userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
        return userRole == "Admin";
    }

    // GET: Admin
    public async Task<IActionResult> Index()
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Account");
        }

        ViewBag.TotalPets = await _context.Pets.CountAsync();
        ViewBag.TotalOrders = await _context.Orders.CountAsync();
        ViewBag.TotalUsers = await _context.Users.Where(u => u.Role == "User").CountAsync();
        ViewBag.TotalRevenue = await _context.Orders
            .Where(o => o.Status == "Completed")
            .SumAsync(o => o.TotalAmount);

        ViewBag.PendingOrders = await _context.Orders
            .Where(o => o.Status == "Pending")
            .CountAsync();

        var recentOrders = await _context.Orders
            .Include(o => o.User)
            .OrderByDescending(o => o.OrderDate)
            .Take(10)
            .ToListAsync();

        return View(recentOrders);
    }

    // GET: Admin/Pets
    public async Task<IActionResult> Pets()
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Account");
        }

        var pets = await _context.Pets
            .Include(p => p.Category)
            .Include(p => p.PetImages)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return View(pets);
    }

    // GET: Admin/CreatePet
    public IActionResult CreatePet()
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Account");
        }

        ViewBag.Categories = _context.Categories.ToList();
        return View();
    }

    // POST: Admin/CreatePet
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePet(Pet pet, List<IFormFile>? imageFiles)
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Account");
        }

        // Debug logging
        Console.WriteLine($"CreatePet called. ImageFiles count: {imageFiles?.Count ?? 0}");
        if (imageFiles != null)
        {
            foreach (var file in imageFiles)
            {
                Console.WriteLine($"File: {file?.FileName}, Size: {file?.Length ?? 0}");
            }
        }

        pet.CreatedAt = DateTime.Now;
        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();

        // Handle multiple image uploads
        if (imageFiles != null && imageFiles.Count > 0 && imageFiles.Any(f => f != null && f.Length > 0))
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "pets");
            Directory.CreateDirectory(uploadsFolder);
            
            for (int i = 0; i < imageFiles.Count; i++)
            {
                var imageFile = imageFiles[i];
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    
                    var petImage = new PetImage
                    {
                        PetId = pet.PetId,
                        ImageUrl = $"/images/pets/{uniqueFileName}",
                        IsDefault = i == 0, // First image is default
                        CreatedAt = DateTime.Now
                    };
                    
                    _context.PetImages.Add(petImage);

                    // Set default image URL for backward compatibility
                    if (i == 0)
                    {
                        pet.ImageUrl = petImage.ImageUrl;
                    }
                }
            }
            
            await _context.SaveChangesAsync();
        }

        TempData["Success"] = "Thêm thú cưng thành công";
        return RedirectToAction(nameof(Pets));
    }

    // GET: Admin/EditPet/5
    public async Task<IActionResult> EditPet(int? id)
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Account");
        }

        if (id == null)
        {
            return NotFound();
        }

        var pet = await _context.Pets
            .Include(p => p.PetImages)
            .FirstOrDefaultAsync(p => p.PetId == id);
        if (pet == null)
        {
            return NotFound();
        }

        ViewBag.Categories = _context.Categories.ToList();
        return View(pet);
    }

    // POST: Admin/EditPet/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPet(int id, Pet pet, List<IFormFile>? imageFiles)
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Account");
        }

        // Debug logging
        Console.WriteLine($"EditPet called for PetId: {id}. ImageFiles count: {imageFiles?.Count ?? 0}");
        if (imageFiles != null)
        {
            foreach (var file in imageFiles)
            {
                Console.WriteLine($"File: {file?.FileName}, Size: {file?.Length ?? 0}");
            }
        }

        if (id != pet.PetId)
        {
            return NotFound();
        }

        var existingPet = await _context.Pets
            .Include(p => p.PetImages)
            .FirstOrDefaultAsync(p => p.PetId == id);
        
        if (existingPet == null)
        {
            return NotFound();
        }

        // Update pet properties
        existingPet.PetName = pet.PetName;
        existingPet.CategoryId = pet.CategoryId;
        existingPet.Price = pet.Price;
        existingPet.Breed = pet.Breed;
        existingPet.Age = pet.Age;
        existingPet.Gender = pet.Gender;
        existingPet.Description = pet.Description;
        existingPet.StockQuantity = pet.StockQuantity;
        existingPet.IsAvailable = pet.IsAvailable;

        // Handle new image uploads
        if (imageFiles != null && imageFiles.Count > 0 && imageFiles.Any(f => f != null && f.Length > 0))
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "pets");
            Directory.CreateDirectory(uploadsFolder);
            
            for (int i = 0; i < imageFiles.Count; i++)
            {
                var imageFile = imageFiles[i];
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    
                    // Check if this is the first image
                    var isFirstImage = existingPet.PetImages.Count == 0 && i == 0;
                    
                    var petImage = new PetImage
                    {
                        PetId = pet.PetId,
                        ImageUrl = $"/images/pets/{uniqueFileName}",
                        IsDefault = isFirstImage,
                        CreatedAt = DateTime.Now
                    };
                    
                    _context.PetImages.Add(petImage);

                    // Update default image URL for backward compatibility (if this is the first image)
                    if (isFirstImage)
                    {
                        existingPet.ImageUrl = petImage.ImageUrl;
                    }
                }
            }
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "Cập nhật thú cưng thành công";
        return RedirectToAction(nameof(Pets));
    }

    // POST: Admin/DeletePetImage
    [HttpPost]
    public async Task<IActionResult> DeletePetImage(int imageId)
    {
        if (!IsAdmin())
        {
            return Json(new { success = false, message = "Unauthorized" });
        }

        var petImage = await _context.PetImages
            .Include(pi => pi.Pet)
            .FirstOrDefaultAsync(pi => pi.ImageId == imageId);

        if (petImage == null)
        {
            return Json(new { success = false, message = "Không tìm thấy ảnh" });
        }

        // Delete physical file
        string fileStatus = "";
        if (!string.IsNullOrEmpty(petImage.ImageUrl) && petImage.ImageUrl.StartsWith("/images/"))
        {
            try
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", petImage.ImageUrl.TrimStart('/'));
                Console.WriteLine($"Attempting to delete file: {imagePath}");
                
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                    fileStatus = " (file deleted)";
                    Console.WriteLine($"File deleted successfully: {imagePath}");
                }
                else
                {
                    fileStatus = " (file not found, skipped)";
                    Console.WriteLine($"File not found: {imagePath}");
                }
            }
            catch (Exception ex)
            {
                fileStatus = " (file delete failed, but record removed)";
                Console.WriteLine($"Error deleting file: {ex.Message}");
                // Continue with database deletion even if file deletion fails
            }
        }

        _context.PetImages.Remove(petImage);
        await _context.SaveChangesAsync();

        // Update default image if needed
        var remainingImages = await _context.PetImages
            .Where(pi => pi.PetId == petImage.PetId)
            .OrderBy(pi => pi.CreatedAt)
            .ToListAsync();

        if (remainingImages.Count > 0)
        {
            if (petImage.Pet != null)
            {
                petImage.Pet.ImageUrl = remainingImages[0].ImageUrl;
                remainingImages[0].IsDefault = true;
                await _context.SaveChangesAsync();
            }
        }

        return Json(new { success = true, message = $"Xóa ảnh thành công{fileStatus}" });
    }

    // POST: Admin/SetDefaultImage
    [HttpPost]
    public async Task<IActionResult> SetDefaultImage(int imageId)
    {
        if (!IsAdmin())
        {
            return Json(new { success = false, message = "Unauthorized" });
        }

        var petImage = await _context.PetImages
            .Include(pi => pi.Pet)
            .FirstOrDefaultAsync(pi => pi.ImageId == imageId);

        if (petImage == null)
        {
            return Json(new { success = false, message = "Không tìm thấy ảnh" });
        }

        // Remove default from other images
        var otherImages = await _context.PetImages
            .Where(pi => pi.PetId == petImage.PetId && pi.ImageId != imageId)
            .ToListAsync();

        foreach (var img in otherImages)
        {
            img.IsDefault = false;
        }

        petImage.IsDefault = true;
        
        if (petImage.Pet != null)
        {
            petImage.Pet.ImageUrl = petImage.ImageUrl;
        }

        await _context.SaveChangesAsync();

        return Json(new { success = true, message = "Đặt ảnh mặc định thành công" });
    }

    // POST: Admin/DeletePet/5
    [HttpPost]
    public async Task<IActionResult> DeletePet(int id)
    {
        if (!IsAdmin())
        {
            return Json(new { success = false, message = "Unauthorized" });
        }

        var pet = await _context.Pets.FindAsync(id);
        if (pet != null)
        {
            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
        }

        return Json(new { success = true });
    }

    // GET: Admin/Orders
    public async Task<IActionResult> Orders()
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Account");
        }

        var orders = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }

    // POST: Admin/UpdateOrderStatus
    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusRequest request)
    {
        if (!IsAdmin())
        {
            return Json(new { success = false, message = "Unauthorized" });
        }

        var order = await _context.Orders
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.OrderId == request.OrderId);
        
        if (order == null)
        {
            return Json(new { success = false, message = "Order not found" });
        }

        order.Status = request.Status;
        if (request.Status == "Completed")
        {
            order.CompletedAt = DateTime.Now;
            
            // Auto-complete COD payment when order is completed
            if (order.Payment != null && order.Payment.PaymentMethod == "COD" && order.Payment.Status == "Pending")
            {
                order.Payment.Status = "Completed";
            }
        }

        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    public class UpdateOrderStatusRequest
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
    }

    // POST: Admin/ConfirmPayment
    [HttpPost]
    public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequest request)
    {
        if (!IsAdmin())
        {
            return Json(new { success = false, message = "Unauthorized" });
        }

        var payment = await _context.Payments.FindAsync(request.PaymentId);
        if (payment == null)
        {
            return Json(new { success = false, message = "Payment not found" });
        }

        payment.Status = request.Status;
        if (request.Status == "Completed" && !string.IsNullOrEmpty(request.TransactionId))
        {
            payment.TransactionId = request.TransactionId;
        }

        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    public class ConfirmPaymentRequest
    {
        public int PaymentId { get; set; }
        public string Status { get; set; }
        public string? TransactionId { get; set; }
    }

    // GET: Admin/Users
    public async Task<IActionResult> Users()
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Account");
        }

        var users = await _context.Users
            .Where(u => u.Role == "User")
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return View(users);
    }

    // POST: Admin/ToggleUserStatus
    [HttpPost]
    public async Task<IActionResult> ToggleUserStatus(int userId)
    {
        if (!IsAdmin())
        {
            return Json(new { success = false, message = "Unauthorized" });
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return Json(new { success = false, message = "User not found" });
        }

        user.IsActive = !user.IsActive;
        await _context.SaveChangesAsync();

        return Json(new { success = true, isActive = user.IsActive });
    }

    // GET: Admin/Categories
    public async Task<IActionResult> Categories()
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Account");
        }

        var categories = await _context.Categories
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return View(categories);
    }

    // GET: Admin/CreateCategory
    public IActionResult CreateCategory()
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Account");
        }

        return View();
    }

    // POST: Admin/CreateCategory
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(Category category)
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Account");
        }

        category.CreatedAt = DateTime.Now;
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Thêm danh mục thành công";
        return RedirectToAction(nameof(Categories));
    }

    // GET: Admin/EditCategory/5
    public async Task<IActionResult> EditCategory(int? id)
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Account");
        }

        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // POST: Admin/EditCategory/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(int id, Category category)
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Account");
        }

        if (id != category.CategoryId)
        {
            return NotFound();
        }

        _context.Update(category);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Cập nhật danh mục thành công";
        return RedirectToAction(nameof(Categories));
    }

    // POST: Admin/DeleteCategory/5
    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        if (!IsAdmin())
        {
            return Json(new { success = false, message = "Unauthorized" });
        }

        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return Json(new { success = false, message = "Category not found" });
        }

        // Kiểm tra xem có pet nào đang dùng category này không
        var hasPets = await _context.Pets.AnyAsync(p => p.CategoryId == id);
        if (hasPets)
        {
            return Json(new { success = false, message = "Không thể xóa danh mục đang có thú cưng" });
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    // GET: Admin/OrderDetail/5
    public async Task<IActionResult> OrderDetail(int? id)
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Account");
        }

        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Pet)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
}
