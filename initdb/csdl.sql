/* ================================
   TẠO DATABASE
================================ */
CREATE DATABASE PetShopDB;
GO
USE PetShopDB;
GO

/* ================================
   BẢNG USERS
================================ */
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(20),
    Address NVARCHAR(500),
    Role NVARCHAR(20) NOT NULL,
    IsActive BIT NOT NULL,
    CreatedAt DATETIME2 NOT NULL
);
GO

/* ================================
   BẢNG CATEGORIES
================================ */
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL
);
GO

/* ================================
   BẢNG PETS
================================ */
CREATE TABLE Pets (
    PetId INT IDENTITY(1,1) PRIMARY KEY,
    PetName NVARCHAR(200) NOT NULL,
    CategoryId INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Breed NVARCHAR(100),
    Age INT NOT NULL,
    Gender NVARCHAR(50),
    Description NVARCHAR(1000),
    ImageUrl NVARCHAR(500),
    StockQuantity INT NOT NULL,
    IsAvailable BIT NOT NULL,
    CreatedAt DATETIME2 NOT NULL
);
GO

/* ================================
   BẢNG PET IMAGES
================================ */
CREATE TABLE PetImages (
    ImageId INT IDENTITY(1,1) PRIMARY KEY,
    PetId INT NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    IsDefault BIT NOT NULL,
    CreatedAt DATETIME2 NOT NULL
);
GO

/* ================================
   BẢNG CARTS
================================ */
CREATE TABLE Carts (
    CartId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL
);
GO

/* ================================
   BẢNG CART ITEMS
================================ */
CREATE TABLE CartItems (
    CartItemId INT IDENTITY(1,1) PRIMARY KEY,
    CartId INT NOT NULL,
    PetId INT NOT NULL,
    Quantity INT NOT NULL,
    AddedAt DATETIME2 NOT NULL
);
GO

/* ================================
   BẢNG ORDERS
================================ */
CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    OrderDate DATETIME2 NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    ShippingAddress NVARCHAR(500),
    Phone NVARCHAR(20),
    Note NVARCHAR(1000),
    CompletedAt DATETIME2
);
GO

/* ================================
   BẢNG ORDER DETAILS
================================ */
CREATE TABLE OrderDetails (
    OrderDetailId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    PetId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    TotalPrice DECIMAL(18,2) NOT NULL
);
GO

/* ================================
   BẢNG PAYMENTS
================================ */
CREATE TABLE Payments (
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL UNIQUE,
    PaymentMethod NVARCHAR(50) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    PaymentDate DATETIME2 NOT NULL,
    TransactionId NVARCHAR(500)
);
GO

/* ================================
   KHOÁ NGOẠI (FOREIGN KEYS)
================================ */
ALTER TABLE Pets
ADD CONSTRAINT FK_Pets_Categories
FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId);
GO

ALTER TABLE PetImages
ADD CONSTRAINT FK_PetImages_Pets
FOREIGN KEY (PetId) REFERENCES Pets(PetId)
ON DELETE CASCADE;
GO

ALTER TABLE Carts
ADD CONSTRAINT FK_Carts_Users
FOREIGN KEY (UserId) REFERENCES Users(UserId)
ON DELETE CASCADE;
GO

ALTER TABLE CartItems
ADD CONSTRAINT FK_CartItems_Carts
FOREIGN KEY (CartId) REFERENCES Carts(CartId)
ON DELETE CASCADE;
GO

ALTER TABLE CartItems
ADD CONSTRAINT FK_CartItems_Pets
FOREIGN KEY (PetId) REFERENCES Pets(PetId);
GO

ALTER TABLE Orders
ADD CONSTRAINT FK_Orders_Users
FOREIGN KEY (UserId) REFERENCES Users(UserId);
GO

ALTER TABLE OrderDetails
ADD CONSTRAINT FK_OrderDetails_Orders
FOREIGN KEY (OrderId) REFERENCES Orders(OrderId)
ON DELETE CASCADE;
GO

ALTER TABLE OrderDetails
ADD CONSTRAINT FK_OrderDetails_Pets
FOREIGN KEY (PetId) REFERENCES Pets(PetId);
GO

ALTER TABLE Payments
ADD CONSTRAINT FK_Payments_Orders
FOREIGN KEY (OrderId) REFERENCES Orders(OrderId)
ON DELETE CASCADE;
GO

/* ================================
   INDEX TỐI ƯU
================================ */
CREATE INDEX IX_Pets_CategoryId ON Pets(CategoryId);
CREATE INDEX IX_CartItems_CartId ON CartItems(CartId);
CREATE INDEX IX_CartItems_PetId ON CartItems(PetId);
CREATE INDEX IX_Orders_UserId ON Orders(UserId);
GO
