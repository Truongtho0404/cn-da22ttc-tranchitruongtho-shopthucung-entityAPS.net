using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.Models;

namespace PetShop.Web.Controllers;

public class PetsController : Controller
{
    private readonly PetShopDbContext _context;

    public PetsController(PetShopDbContext context)
    {
        _context = context;
    }

    // GET: Pets
    public async Task<IActionResult> Index(int? categoryId, string? searchString, decimal? minPrice, decimal? maxPrice)
    {
        var pets = _context.Pets
            .Include(p => p.Category)
            .Where(p => p.IsAvailable);

        if (categoryId.HasValue)
        {
            pets = pets.Where(p => p.CategoryId == categoryId);
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            pets = pets.Where(p => p.PetName.Contains(searchString) || 
                                   p.Breed!.Contains(searchString) ||
                                   p.Description!.Contains(searchString));
        }

        if (minPrice.HasValue)
        {
            pets = pets.Where(p => p.Price >= minPrice);
        }

        if (maxPrice.HasValue)
        {
            pets = pets.Where(p => p.Price <= maxPrice);
        }

        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.SelectedCategory = categoryId;
        ViewBag.SearchString = searchString;
        ViewBag.MinPrice = minPrice;
        ViewBag.MaxPrice = maxPrice;

        return View(await pets.ToListAsync());
    }

    // GET: Pets/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var pet = await _context.Pets
            .Include(p => p.Category)
            .Include(p => p.PetImages)
            .FirstOrDefaultAsync(m => m.PetId == id);

        if (pet == null)
        {
            return NotFound();
        }

        return View(pet);
    }
}
