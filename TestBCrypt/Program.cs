using System;
using BCrypt.Net;

class Program
{
    static void Main()
    {
        var password = "123456";
        var hash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy";
        
        Console.WriteLine($"Password: {password}");
        Console.WriteLine($"Hash: {hash}");
        Console.WriteLine($"Verify Result: {BCrypt.Verify(password, hash)}");
        
        // Tạo hash mới
        var newHash = BCrypt.HashPassword(password);
        Console.WriteLine($"New Hash: {newHash}");
        Console.WriteLine($"New Hash Verify: {BCrypt.Verify(password, newHash)}");
    }
}
