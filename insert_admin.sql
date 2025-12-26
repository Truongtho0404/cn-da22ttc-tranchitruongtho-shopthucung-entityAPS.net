-- Insert admin user with BCrypt hash for password: 123456
INSERT INTO Users (FullName, Email, PasswordHash, Role, Phone, Address, IsActive, CreatedAt) 
VALUES (N'Admin', 'admin@petshop.com', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Admin', '0123456789', N'Hà Nội', 1, GETDATE());
