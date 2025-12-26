# HƯỚNG DẪN SỬ DỤNG HỆ THỐNG BÁN THÚ CƯNG - PET SHOP

## 🎯 Thông tin dự án

**Tên đề tài**: Ứng dụng Entity Framework xây dựng hệ thống bán thú cưng  
**Sinh viên**: Trần Chí Trường Thọ - MSSV: 110122170  
**Lớp**: DA22TTC  
**Giảng viên hướng dẫn**: Trịnh Quốc Việt

## 🚀 Khởi động nhanh

### 1. Khởi động Docker Containers

```bash
cd f:\Entity
docker-compose up -d
```

Lệnh này sẽ:
- Tải và khởi động SQL Server 2022
- Build và khởi động ứng dụng ASP.NET Core
- Tự động chạy migrations và tạo database với dữ liệu mẫu

### 2. Truy cập ứng dụng

- **Website**: http://localhost:5000
- **SQL Server**: localhost:1433
  - Username: `sa`
  - Password: `YourStrong@Passw0rd`
  - Database: `PetShopDB`

### 3. Tài khoản đăng nhập mặc định

**Admin:**
- Email: `admin@petshop.com`
- Password: `Admin@123`

## 📋 Các chức năng chính

### Người dùng (User)
✅ Đăng ký/Đăng nhập tài khoản  
✅ Xem danh sách thú cưng theo danh mục  
✅ Tìm kiếm thú cưng theo tên, giống, giá  
✅ Xem chi tiết thú cưng  
✅ Thêm vào giỏ hàng  
✅ Quản lý giỏ hàng (thêm/xóa/cập nhật số lượng)  
✅ Đặt hàng và thanh toán  
✅ Theo dõi đơn hàng  
✅ Xem lịch sử mua hàng  

### Quản trị viên (Admin)
✅ Dashboard thống kê tổng quan  
✅ Quản lý thú cưng (CRUD)  
✅ Quản lý đơn hàng (xem, cập nhật trạng thái)  
✅ Quản lý người dùng (xem, khóa/mở khóa tài khoản)  
✅ Thống kê doanh thu  

## 🗄️ Database Schema

### Các bảng chính:

1. **Users** - Thông tin người dùng
   - UserId, FullName, Email, PasswordHash, Phone, Address, Role, IsActive, CreatedAt

2. **Categories** - Danh mục thú cưng
   - CategoryId, CategoryName, Description, CreatedAt

3. **Pets** - Thông tin thú cưng
   - PetId, PetName, CategoryId, Price, Breed, Age, Gender, Description, ImageUrl, StockQuantity, IsAvailable, CreatedAt

4. **Orders** - Đơn hàng
   - OrderId, UserId, OrderDate, TotalAmount, Status, ShippingAddress, Phone, Note, CompletedAt

5. **OrderDetails** - Chi tiết đơn hàng
   - OrderDetailId, OrderId, PetId, Quantity, UnitPrice, TotalPrice

6. **Carts** - Giỏ hàng
   - CartId, UserId, CreatedAt, UpdatedAt

7. **CartItems** - Sản phẩm trong giỏ hàng
   - CartItemId, CartId, PetId, Quantity, AddedAt

8. **Payments** - Thanh toán
   - PaymentId, OrderId, PaymentMethod, Amount, Status, PaymentDate, TransactionId

## 🛠️ Công nghệ sử dụng

- **Backend**: ASP.NET Core 8.0 MVC
- **ORM**: Entity Framework Core 8.0 (Code First)
- **Database**: SQL Server 2022
- **Frontend**: Razor Pages, Bootstrap 5, Font Awesome, jQuery
- **Authentication**: Session-based Authentication
- **Password Hashing**: BCrypt.Net
- **Container**: Docker, Docker Compose

## 📁 Cấu trúc dự án

```
PetShop/
├── PetShop.Models/          # Entity Models
│   ├── User.cs
│   ├── Category.cs
│   ├── Pet.cs
│   ├── Order.cs
│   ├── OrderDetail.cs
│   ├── Cart.cs
│   ├── CartItem.cs
│   └── Payment.cs
│
├── PetShop.Data/            # Data Layer
│   ├── PetShopDbContext.cs
│   └── Migrations/
│
├── PetShop.Web/             # Web Application
│   ├── Controllers/
│   │   ├── HomeController.cs
│   │   ├── PetsController.cs
│   │   ├── CartController.cs
│   │   ├── OrdersController.cs
│   │   ├── AccountController.cs
│   │   └── AdminController.cs
│   │
│   ├── Views/
│   │   ├── Home/
│   │   ├── Pets/
│   │   ├── Cart/
│   │   ├── Orders/
│   │   ├── Account/
│   │   ├── Admin/
│   │   └── Shared/
│   │
│   ├── Program.cs
│   └── appsettings.json
│
├── Dockerfile
├── docker-compose.yml
└── README.md
```

## 🔧 Các lệnh Docker hữu ích

### Xem logs của ứng dụng:
```bash
docker logs petshop_webapp -f
```

### Xem logs của SQL Server:
```bash
docker logs petshop_sqlserver -f
```

### Dừng containers:
```bash
docker-compose down
```

### Dừng và xóa toàn bộ (bao gồm database):
```bash
docker-compose down -v
```

### Rebuild sau khi thay đổi code:
```bash
docker-compose up --build -d
```

### Kiểm tra trạng thái containers:
```bash
docker-compose ps
```

### Vào container để debug:
```bash
# Vào webapp container
docker exec -it petshop_webapp bash

# Vào SQL Server container
docker exec -it petshop_sqlserver bash
```

## 🔍 Kiểm tra Database

### Sử dụng Azure Data Studio hoặc SQL Server Management Studio:

**Connection String:**
```
Server=localhost,1433;
Database=PetShopDB;
User Id=sa;
Password=YourStrong@Passw0rd;
TrustServerCertificate=True;
```

### Hoặc sử dụng command line:
```bash
docker exec -it petshop_sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C
```

## 📝 Dữ liệu mẫu

Hệ thống đã tự động tạo dữ liệu mẫu bao gồm:

### Danh mục (6):
- Chó
- Mèo
- Chim
- Cá
- Phụ kiện
- Thức ăn

### Thú cưng (5):
- Chó Corgi - 8,000,000 đ
- Mèo Ba Tư - 5,000,000 đ
- Chó Poodle - 6,000,000 đ
- Mèo Anh Lông Ngắn - 4,000,000 đ
- Chim Vẹt - 2,000,000 đ

### Tài khoản Admin:
- Email: admin@petshop.com
- Password: Admin@123

## 🎨 Tính năng nổi bật

### 1. Giao diện thân thiện
- Responsive design với Bootstrap 5
- Gradient màu sắc đẹp mắt
- Animation và transition mượt mà
- Icon Font Awesome trực quan

### 2. Quản lý giỏ hàng thông minh
- Thêm/xóa/cập nhật số lượng real-time
- Tính toán tổng tiền tự động
- Kiểm tra tồn kho

### 3. Quy trình đặt hàng đơn giản
- Checkout nhanh chóng
- Nhiều phương thức thanh toán
- Theo dõi trạng thái đơn hàng

### 4. Admin Dashboard mạnh mẽ
- Thống kê real-time
- Quản lý đơn hàng hiệu quả
- CRUD thú cưng dễ dàng
- Quản lý người dùng

### 5. Bảo mật
- Password hashing với BCrypt
- Session-based authentication
- Role-based authorization
- SQL injection prevention với EF Core

## 🐛 Xử lý lỗi thường gặp

### Lỗi: Container không khởi động được

**Giải pháp:**
```bash
# Xóa containers và volumes cũ
docker-compose down -v

# Rebuild lại
docker-compose up --build -d
```

### Lỗi: Không kết nối được database

**Kiểm tra:**
1. SQL Server container đã healthy chưa: `docker ps`
2. Xem logs SQL Server: `docker logs petshop_sqlserver`
3. Kiểm tra port 1433 có bị chiếm không

### Lỗi: Port 5000 đã được sử dụng

**Giải pháp:** Sửa port trong `docker-compose.yml`:
```yaml
ports:
  - "5001:80"  # Đổi 5000 thành 5001
```

## 📊 Workflow phát triển

### 1. Thêm Migration mới:
```bash
cd PetShop.Web
dotnet ef migrations add TenMigration --project ../PetShop.Data
dotnet ef database update --project ../PetShop.Data
```

### 2. Rollback Migration:
```bash
dotnet ef database update TenMigrationTruoc --project ../PetShop.Data
dotnet ef migrations remove --project ../PetShop.Data
```

### 3. Xem script SQL của Migration:
```bash
dotnet ef migrations script --project ../PetShop.Data
```

## 🎓 Kết quả đạt được

✅ Xây dựng hoàn chỉnh website bán thú cưng với đầy đủ chức năng  
✅ Áp dụng thành công Entity Framework Code First  
✅ Triển khai thành công trên Docker container  
✅ Database được quản lý hiệu quả với migrations  
✅ Giao diện responsive, thân thiện người dùng  
✅ Bảo mật tốt với authentication và authorization  
✅ Code sạch, dễ bảo trì theo kiến trúc MVC  

## 📞 Liên hệ & Hỗ trợ

- **Sinh viên**: Trần Chí Trường Thọ
- **Email**: [email của bạn]
- **GitHub**: [github của bạn]

---

**© 2025 Pet Shop - Đồ án thực tập chuyên ngành**
