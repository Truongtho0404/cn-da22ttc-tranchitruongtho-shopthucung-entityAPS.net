using Microsoft.EntityFrameworkCore;
using PetShop.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext with SQL Server
builder.Services.AddDbContext<PetShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add session support with extended timeout
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".PetShop.Session";
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Path = "/";
    options.IOTimeout = TimeSpan.FromMinutes(5);
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

// Auto-restore session from persistent cookie
app.Use(async (context, next) =>
{
    try
    {
        // Always try to restore from cookie if session is missing
        var sessionUserId = context.Session.GetString("UserId");
        
        if (string.IsNullOrEmpty(sessionUserId))
        {
            if (context.Request.Cookies.TryGetValue("PetShop_UserId", out var userId) &&
                !string.IsNullOrEmpty(userId))
            {
                var dbContext = context.RequestServices.GetRequiredService<PetShopDbContext>();
                
                if (int.TryParse(userId, out int userIdInt))
                {
                    var user = await dbContext.Users.FindAsync(userIdInt);
                    
                    if (user != null && user.IsActive)
                    {
                        // Restore session
                        context.Session.SetString("UserId", user.UserId.ToString());
                        context.Session.SetString("UserName", user.FullName);
                        context.Session.SetString("UserRole", user.Role);
                        
                        // Extend cookie lifetime
                        context.Response.Cookies.Append("PetShop_UserId", user.UserId.ToString(), new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddDays(7),
                            HttpOnly = true,
                            IsEssential = true,
                            SameSite = SameSiteMode.Lax,
                            Path = "/"
                        });
                        context.Response.Cookies.Append("PetShop_UserRole", user.Role, new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddDays(7),
                            HttpOnly = true,
                            IsEssential = true,
                            SameSite = SameSiteMode.Lax,
                            Path = "/"
                        });
                    }
                }
            }
        }
    }
    catch (Exception)
    {
        // Ignore errors in session restore
    }
    
    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Auto migrate database and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PetShopDbContext>();
        
        // Run migrations
        context.Database.Migrate();
        Console.WriteLine("Database migration completed successfully.");
        
        // Seed data if tables are empty
        if (!context.Categories.Any())
        {
            var seedSqlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SeedData.sql");
            if (File.Exists(seedSqlPath))
            {
                var seedSql = File.ReadAllText(seedSqlPath);
                context.Database.ExecuteSqlRaw(seedSql);
                Console.WriteLine("Seed data inserted successfully.");
            }
            else
            {
                Console.WriteLine("SeedData.sql file not found, skipping seed data.");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
}

app.Run();
