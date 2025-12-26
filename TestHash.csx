using System;
using BCrypt.Net;

// Test BCrypt hash
var password = "Admin@123";
var hash = BCrypt.HashPassword(password);
Console.WriteLine($"New Hash: {hash}");
Console.WriteLine($"Verify Test: {BCrypt.Verify(password, hash)}");

// Test với hash cũ trong DB
var oldHash = "$2a$11$xQHvZ8yGvQvKX8LmN5.pxOYJ1ZC7fEhF.AUhLjcqvZz8pF5YqJGne";
Console.WriteLine($"Old Hash Verify: {BCrypt.Verify(password, oldHash)}");
