# Hướng dẫn kiểm tra các tính năng Admin mới

## Các tính năng đã hoàn thành

### 1. **Quản lý Danh mục (Categories)**
   - **URL:** http://localhost:5000/Admin/Categories
   - **Chức năng:**
     - Xem danh sách tất cả danh mục
     - Thêm danh mục mới (nút "Thêm danh mục mới")
     - Sửa danh mục (nút "Sửa" trên mỗi hàng)
     - Xóa danh mục (nút "Xóa" - có kiểm tra foreign key)
   
   **Các bước test:**
   1. Truy cập /Admin/Categories
   2. Click "Thêm danh mục mới"
   3. Nhập tên danh mục và mô tả
   4. Submit form
   5. Kiểm tra danh mục mới xuất hiện trong danh sách
   6. Click "Sửa" để chỉnh sửa danh mục
   7. Thử xóa danh mục (nếu danh mục có thú cưng thì sẽ báo lỗi)

### 2. **Chi tiết Đơn hàng (Order Detail)**
   - **URL:** http://localhost:5000/Admin/OrderDetail/{orderId}
   - **Chức năng:**
     - Xem thông tin chi tiết đơn hàng
     - Xem thông tin khách hàng (tên, email, phone, địa chỉ giao hàng)
     - Xem danh sách sản phẩm trong đơn
     - Xem thông tin thanh toán
     - **Cập nhật trạng thái đơn hàng** (dropdown với 5 trạng thái)
   
   **Các bước test:**
   1. Truy cập /Admin/Orders
   2. Click "Chi tiết" trên bất kỳ đơn hàng nào
   3. Xem toàn bộ thông tin đơn hàng
   4. Thử thay đổi trạng thái từ dropdown bên phải
   5. Click "Cập nhật trạng thái"
   6. Xác nhận và kiểm tra trạng thái đã thay đổi

### 3. **Danh sách Đơn hàng được cải tiến (Orders List)**
   - **URL:** http://localhost:5000/Admin/Orders
   - **Thay đổi:**
     - Trạng thái hiển thị dưới dạng badge màu sắc (thay vì dropdown)
       - Chờ xác nhận: Vàng (warning)
       - Đã xác nhận: Xanh dương nhạt (info)
       - Đang giao: Xanh dương (primary)
       - Hoàn thành: Xanh lá (success)
       - Đã hủy: Xám (secondary)
     - Nút "Chi tiết" dẫn đến trang OrderDetail mới

### 4. **Quản lý Thú cưng (Pets CRUD)** *(Đã có từ trước)*
   - **URL:** http://localhost:5000/Admin/Pets
   - **Chức năng:** Xem, thêm, sửa, xóa thú cưng

### 5. **Quản lý Người dùng (Users Management)** *(Đã có từ trước)*
   - **URL:** http://localhost:5000/Admin/Users
   - **Chức năng:** Xem danh sách người dùng, kích hoạt/vô hiệu hóa tài khoản

## Luồng test đầy đủ

### Bước 1: Đăng nhập với tài khoản Admin
```
URL: http://localhost:5000/Account/Login
Email: admin@petshop.com
Password: 123456
```

### Bước 2: Test quản lý Categories
1. Menu Admin → Categories
2. Thêm danh mục mới: "Chó Poodle"
3. Sửa mô tả của danh mục vừa tạo
4. Thử xóa danh mục có thú cưng (sẽ báo lỗi)
5. Xóa danh mục không có thú cưng (thành công)

### Bước 3: Test quản lý đơn hàng
1. Menu Admin → Orders
2. Kiểm tra badge trạng thái có màu sắc phù hợp
3. Click "Chi tiết" trên 1 đơn hàng
4. Xem đầy đủ thông tin đơn hàng
5. Thay đổi trạng thái từ "Pending" → "Confirmed"
6. Quay lại trang Orders và kiểm tra trạng thái đã update

### Bước 4: Test quản lý Pets (đã có sẵn)
1. Menu Admin → Pets
2. Thêm thú cưng mới
3. Sửa thông tin thú cưng
4. Xóa thú cưng

## API Endpoints

### Categories API
- **GET** `/Admin/Categories` - Danh sách categories
- **GET** `/Admin/CreateCategory` - Form thêm mới
- **POST** `/Admin/CreateCategory` - Xử lý thêm mới
- **GET** `/Admin/EditCategory/{id}` - Form chỉnh sửa
- **POST** `/Admin/EditCategory` - Xử lý chỉnh sửa
- **POST** `/Admin/DeleteCategory` - Xóa category (JSON response)

### Orders API
- **GET** `/Admin/Orders` - Danh sách đơn hàng
- **GET** `/Admin/OrderDetail/{id}` - Chi tiết đơn hàng
- **POST** `/Admin/UpdateOrderStatus` - Cập nhật trạng thái (JSON request/response)

## Ghi chú kỹ thuật

### Validation
- **Categories:** CategoryName là required, CreatedAt tự động set
- **Order Status:** Phải là 1 trong 5 giá trị: Pending, Confirmed, Shipping, Completed, Cancelled

### Foreign Key Protection
- Không thể xóa Category nếu có Pet đang sử dụng
- Thông báo lỗi: "Không thể xóa danh mục này vì đang có thú cưng thuộc danh mục này"

### Security
- Tất cả action Admin đều có `[HttpGet]` hoặc `[HttpPost]` attribute
- Kiểm tra IsAdmin() trước khi thực hiện bất kỳ thao tác nào
- Unauthorized users sẽ bị redirect về /Account/Login

## Các file đã tạo/sửa

### Controllers
- `Controllers/AdminController.cs` - Thêm 7 methods mới:
  - CreateCategory (GET/POST)
  - EditCategory (GET/POST)
  - DeleteCategory (POST)
  - OrderDetail (GET)

### Views
- `Views/Admin/Categories.cshtml` - Danh sách categories
- `Views/Admin/CreateCategory.cshtml` - Form thêm category
- `Views/Admin/EditCategory.cshtml` - Form sửa category
- `Views/Admin/OrderDetail.cshtml` - Chi tiết đơn hàng với form cập nhật trạng thái
- `Views/Admin/Orders.cshtml` - Cải tiến hiển thị trạng thái

## Kiến nghị phát triển tiếp

1. **Báo cáo thống kê:** Dashboard với biểu đồ doanh thu, số đơn hàng theo trạng thái
2. **Tìm kiếm/Lọc:** Thêm tìm kiếm đơn hàng theo tên khách hàng, ngày, trạng thái
3. **Xuất báo cáo:** Xuất danh sách đơn hàng ra Excel/PDF
4. **Thông báo:** Email tự động khi đơn hàng thay đổi trạng thái
5. **Phân trang:** Áp dụng pagination cho danh sách có nhiều dữ liệu
6. **Upload ảnh:** Thêm upload ảnh cho Categories và Pets
7. **Quản lý kho:** Theo dõi tồn kho cho từng Pet
