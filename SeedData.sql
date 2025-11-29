-- Seed Categories
IF NOT EXISTS (SELECT 1 FROM Categories)
BEGIN
    SET IDENTITY_INSERT Categories ON;
    INSERT INTO Categories (CategoryId, CategoryName, Description, CreatedAt) VALUES
    (1, N'Chó', N'Các giống chó cảnh', GETDATE()),
    (2, N'Mèo', N'Các giống mèo cảnh', GETDATE()),
    (3, N'Chim', N'Các loại chim cảnh', GETDATE()),
    (4, N'Cá', N'Các loại cá cảnh', GETDATE()),
    (5, N'Phụ kiện', N'Phụ kiện cho thú cưng', GETDATE()),
    (6, N'Thức ăn', N'Thức ăn cho thú cưng', GETDATE());
    SET IDENTITY_INSERT Categories OFF;
END

-- Seed Admin User (password: 123456)
IF NOT EXISTS (SELECT 1 FROM Users)
BEGIN
    SET IDENTITY_INSERT Users ON;
    INSERT INTO Users (UserId, FullName, Email, PasswordHash, Role, Phone, Address, IsActive, CreatedAt) VALUES
    (1, N'Admin', 'admin@petshop.com', '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Admin', '0123456789', N'Hà Nội', 1, GETDATE());
    SET IDENTITY_INSERT Users OFF;
END

-- Seed Pets
IF NOT EXISTS (SELECT 1 FROM Pets)
BEGIN
    SET IDENTITY_INSERT Pets ON;
    INSERT INTO Pets (PetId, PetName, CategoryId, Price, Breed, Age, Gender, Description, ImageUrl, StockQuantity, IsAvailable, CreatedAt) VALUES
    (1, N'Chó Corgi', 1, 8000000, 'Corgi', 3, N'Đực', N'Chó Corgi thuần chủng, khỏe mạnh, đã tiêm phòng đầy đủ', '/images/pets/corgi.jpg', 5, 1, GETDATE()),
    (2, N'Mèo Ba Tư', 2, 5000000, N'Ba Tư', 2, N'Cái', N'Mèo Ba Tư lông dài, đã tiêm phòng, thuần chủng', '/images/pets/persian-cat.jpg', 3, 1, GETDATE()),
    (3, N'Chó Poodle', 1, 6000000, 'Poodle', 4, N'Đực', N'Chó Poodle toy, size nhỏ xinh, lông xù đáng yêu', '/images/pets/poodle.jpg', 7, 1, GETDATE()),
    (4, N'Mèo Anh Lông Ngắn', 2, 4000000, 'British Shorthair', 1, N'Đực', N'Mèo Anh lông ngắn màu xám xanh, hiền lành', '/images/pets/british-shorthair.jpg', 4, 1, GETDATE()),
    (5, N'Chim Vẹt', 3, 2000000, N'Vẹt Nam Mỹ', 1, N'Không xác định', N'Chim vẹt đủ màu sắc, biết nói, thân thiện', '/images/pets/parrot.jpg', 10, 1, GETDATE());
    SET IDENTITY_INSERT Pets OFF;
END
