using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetShop.Data;
using PetShop.Models;

namespace PetShop.Web.Controllers;

public class AccountController : Controller
{
    private readonly PetShopDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AccountController(PetShopDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    // GET: Account/Login
    public IActionResult Login()
    {
        return View();
    }

    // POST: Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password, bool rememberMe = false)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ViewBag.Error = "Vui lòng nhập đầy đủ thông tin";
            return View();
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            Console.WriteLine($"User not found: {email}");
            ViewBag.Error = "Email hoặc mật khẩu không đúng";
            return View();
        }

        Console.WriteLine($"Found user: {user.Email}, Hash: {user.PasswordHash?.Substring(0, Math.Min(20, user.PasswordHash.Length))}...");
        Console.WriteLine($"Password to verify: {password}");
        
        var verifyResult = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        Console.WriteLine($"BCrypt Verify Result: {verifyResult}");

        if (!verifyResult)
        {
            ViewBag.Error = "Email hoặc mật khẩu không đúng";
            return View();
        }

        if (!user.IsActive)
        {
            ViewBag.Error = "Tài khoản đã bị khóa";
            return View();
        }

        // Set session
        _httpContextAccessor.HttpContext?.Session.SetString("UserId", user.UserId.ToString());
        _httpContextAccessor.HttpContext?.Session.SetString("UserName", user.FullName);
        _httpContextAccessor.HttpContext?.Session.SetString("UserRole", user.Role);

        // Set persistent cookie
        var cookieOptions = new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Path = "/"
        };
        Response.Cookies.Append("PetShop_UserId", user.UserId.ToString(), cookieOptions);
        Response.Cookies.Append("PetShop_UserRole", user.Role, cookieOptions);

        if (user.Role == "Admin")
        {
            return RedirectToAction("Index", "Admin");
        }

        return RedirectToAction("Index", "Home");
    }

    // GET: Account/Register
    public IActionResult Register()
    {
        return View();
    }

    // POST: Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(User user, string confirmPassword)
    {
        if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.PasswordHash))
        {
            ViewBag.Error = "Vui lòng nhập đầy đủ thông tin";
            return View(user);
        }

        if (user.PasswordHash != confirmPassword)
        {
            ViewBag.Error = "Mật khẩu xác nhận không khớp";
            return View(user);
        }

        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        if (existingUser != null)
        {
            ViewBag.Error = "Email đã được sử dụng";
            return View(user);
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
        user.Role = "User";
        user.IsActive = true;
        user.CreatedAt = DateTime.Now;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
        return RedirectToAction(nameof(Login));
    }

    // GET: Account/Logout
    public IActionResult Logout()
    {
        _httpContextAccessor.HttpContext?.Session.Clear();
        
        // Clear persistent cookies
        Response.Cookies.Delete("PetShop_UserId");
        Response.Cookies.Delete("PetShop_UserRole");
        
        return RedirectToAction("Index", "Home");
    }

    // GET: Account/Profile
    public async Task<IActionResult> Profile()
    {
        var userIdString = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
        if (!int.TryParse(userIdString, out int userId))
        {
            return RedirectToAction(nameof(Login));
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return RedirectToAction(nameof(Login));
        }

        return View(user);
    }

    // GET: Account/ChangePassword
    public IActionResult ChangePassword()
    {
        var userIdString = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
        if (!int.TryParse(userIdString, out int userId))
        {
            return RedirectToAction(nameof(Login));
        }

        return View();
    }

    // POST: Account/ChangePassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
    {
        var userIdString = _httpContextAccessor.HttpContext?.Session.GetString("UserId");
        if (!int.TryParse(userIdString, out int userId))
        {
            return RedirectToAction(nameof(Login));
        }

        if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
        {
            ViewBag.Error = "Vui lòng nhập đầy đủ thông tin";
            return View();
        }

        if (newPassword != confirmPassword)
        {
            ViewBag.Error = "Mật khẩu xác nhận không khớp";
            return View();
        }

        if (newPassword.Length < 6)
        {
            ViewBag.Error = "Mật khẩu mới phải có ít nhất 6 ký tự";
            return View();
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return RedirectToAction(nameof(Login));
        }

        // Verify old password
        if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
        {
            ViewBag.Error = "Mật khẩu cũ không đúng";
            return View();
        }

        // Hash and update new password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        ViewBag.Success = "Đổi mật khẩu thành công";
        return View();
    }

    // GET: Account/ForgotPassword
    public IActionResult ForgotPassword()
    {
        return View();
    }

    // POST: Account/ForgotPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            ViewBag.Error = "Vui lòng nhập email";
            return View();
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            ViewBag.Error = "Email không tồn tại trong hệ thống";
            return View();
        }

        // Generate reset token (simple approach: 6-digit code)
        var resetCode = new Random().Next(100000, 999999).ToString();
        
        // Store reset code in session (expires with session)
        _httpContextAccessor.HttpContext?.Session.SetString("ResetCode", resetCode);
        _httpContextAccessor.HttpContext?.Session.SetString("ResetEmail", email);
        _httpContextAccessor.HttpContext?.Session.SetString("ResetCodeTime", DateTime.Now.ToString());

        ViewBag.Success = $"Mã xác nhận của bạn là: {resetCode} (Lưu ý: Trong thực tế mã này sẽ được gửi qua email)";
        return View();
    }

    // GET: Account/ResetPassword
    public IActionResult ResetPassword()
    {
        return View();
    }

    // POST: Account/ResetPassword
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(string code, string newPassword, string confirmPassword)
    {
        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(newPassword))
        {
            ViewBag.Error = "Vui lòng nhập đầy đủ thông tin";
            return View();
        }

        if (newPassword != confirmPassword)
        {
            ViewBag.Error = "Mật khẩu xác nhận không khớp";
            return View();
        }

        if (newPassword.Length < 6)
        {
            ViewBag.Error = "Mật khẩu mới phải có ít nhất 6 ký tự";
            return View();
        }

        var resetCode = _httpContextAccessor.HttpContext?.Session.GetString("ResetCode");
        var resetEmail = _httpContextAccessor.HttpContext?.Session.GetString("ResetEmail");
        var resetCodeTime = _httpContextAccessor.HttpContext?.Session.GetString("ResetCodeTime");

        if (string.IsNullOrEmpty(resetCode) || string.IsNullOrEmpty(resetEmail))
        {
            ViewBag.Error = "Phiên đặt lại mật khẩu đã hết hạn. Vui lòng thực hiện lại từ đầu";
            return View();
        }

        // Check if code expired (15 minutes)
        if (DateTime.TryParse(resetCodeTime, out DateTime codeTime))
        {
            if ((DateTime.Now - codeTime).TotalMinutes > 15)
            {
                _httpContextAccessor.HttpContext?.Session.Remove("ResetCode");
                _httpContextAccessor.HttpContext?.Session.Remove("ResetEmail");
                _httpContextAccessor.HttpContext?.Session.Remove("ResetCodeTime");
                ViewBag.Error = "Mã xác nhận đã hết hạn (15 phút). Vui lòng thực hiện lại";
                return View();
            }
        }

        if (code != resetCode)
        {
            ViewBag.Error = "Mã xác nhận không đúng";
            return View();
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == resetEmail);
        if (user == null)
        {
            ViewBag.Error = "Không tìm thấy người dùng";
            return View();
        }

        // Update password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        // Clear reset session
        _httpContextAccessor.HttpContext?.Session.Remove("ResetCode");
        _httpContextAccessor.HttpContext?.Session.Remove("ResetEmail");
        _httpContextAccessor.HttpContext?.Session.Remove("ResetCodeTime");

        ViewBag.Success = "Đặt lại mật khẩu thành công. Bạn có thể đăng nhập ngay bây giờ";
        return View();
    }
}
