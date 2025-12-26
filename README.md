# Hệ thống bán thú cưng - Pet Shop

## Giới thiệu
Ứng dụng web bán thú cưng được xây dựng với ASP.NET Core 8.0, Entity Framework Core và SQL Server.

## Công nghệ sử dụng
- **Backend**: ASP.NET Core 8.0 MVC
- **ORM**: Entity Framework Core (Code First)
- **Database**: SQL Server 2022
- **Frontend**: Razor Pages, Bootstrap 5
- **Container**: Docker, Docker Compose

## Cấu trúc dự án
```
PetShop/
├── PetShop.Models/          # Các entity models
├── PetShop.Data/            # DbContext và migrations
├── PetShop.Web/             # MVC Web application
├── Dockerfile               # Docker configuration
└── docker-compose.yml       # Docker Compose configuration
```

## Các chức năng chính

### Quản trị viên (Admin)
- Quản lý danh mục thú cưng
- Quản lý người dùng
- Quản lý đơn hàng
- Thống kê doanh thu

### Người dùng
- Đăng ký/Đăng nhập
- Tìm kiếm thú cưng
- Thêm vào giỏ hàng
- Đặt hàng
- Theo dõi đơn hàng

## Cài đặt và chạy

### Yêu cầu
- Docker Desktop
- .NET 8.0 SDK (nếu chạy local)

### Chạy với Docker (Khuyến nghị)

1. Clone repository và di chuyển vào thư mục dự án:
```bash
cd f:\Entity
```

2. Build và chạy containers:
```bash
docker-compose up --build -d
```

**Lần đầu chạy:** Database sẽ tự động restore từ backup `initdb/PetShopDB.bak` với đầy đủ dữ liệu mẫu và hình ảnh.

3. Truy cập ứng dụng:
- Web App: http://localhost:5000
- HTTPS: https://localhost:5001
- SQL Server: localhost:1433

4. Thông tin đăng nhập mặc định:
- **Admin**: admin@petshop.com / admin123
- **User**: user@petshop.com / user123

### Chia sẻ dự án cho người khác

Khi gửi code cho người khác, đảm bảo bao gồm:
- ✅ Toàn bộ source code
- ✅ File `initdb/PetShopDB.bak` (6MB - database backup với đầy đủ dữ liệu)
- ✅ Thư mục `PetShop.Web/wwwroot/images/` (8MB - hình ảnh sản phẩm)

Người nhận chỉ cần:
```bash
docker-compose up --build -d
```

**Điều gì sẽ xảy ra:**
1. SQL Server container khởi động
2. Script tự động kiểm tra nếu database trống → restore từ `initdb/PetShopDB.bak`
3. Web app build và copy hình ảnh từ `wwwroot/images/` vào container
4. Ứng dụng sẵn sàng với đầy đủ 5 pets, 13 images, và tài khoản admin/user!

→ **Không cần setup gì thêm, chỉ cần Docker Desktop!**

### Chạy local (không dùng Docker)

1. Cài đặt dependencies:
```bash
dotnet restore
```

2. Update connection string trong `appsettings.Development.json`

3. Chạy migrations:
```bash
cd PetShop.Web
dotnet ef database update --project ../PetShop.Data
```

4. Chạy ứng dụng:
```bash
dotnet run
```

## Database Schema

### Các bảng chính:
- **Users**: Thông tin người dùng
- **Categories**: Danh mục thú cưng
- **Pets**: Thông tin thú cưng
- **Orders**: Đơn hàng
- **OrderDetails**: Chi tiết đơn hàng
- **Carts**: Giỏ hàng
- **CartItems**: Sản phẩm trong giỏ hàng
- **Payments**: Thanh toán

## Lệnh Docker hữu ích

### Dừng containers:
```bash
docker-compose down
```

### Xóa volumes (reset database):
```bash
docker-compose down -v
```

### Xem logs:
```bash
docker-compose logs -f webapp
```

### Rebuild sau khi thay đổi code:
```bash
docker-compose up --build
```

## Thông tin kết nối SQL Server

- **Server**: localhost,1433
- **Username**: sa
- **Password**: YourStrong@Passw0rd
- **Database**: PetShopDB

## Tác giả
- Sinh viên: Trần Chí Trường Thọ
- MSSV: 110122170
- Lớp: DA22TTC
- Giảng viên hướng dẫn: Trịnh Quốc Việt

## License
Đồ án thực tập chuyên ngành
