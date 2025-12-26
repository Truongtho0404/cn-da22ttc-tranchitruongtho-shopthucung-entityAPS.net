using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.Models;

namespace PetShop.Web.Controllers;

public class HomeController : Controller
{
    private readonly PetShopDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(PetShopDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var featuredPets = await _context.Pets
            .Include(p => p.Category)
            .Include(p => p.PetImages)
            .Where(p => p.IsAvailable && p.StockQuantity > 0)
            .OrderByDescending(p => p.CreatedAt)
            .Take(8)
            .ToListAsync();

        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View(featuredPets);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
