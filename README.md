# 📚 API_WebH3 — Backend API cho Nền tảng Học trực tuyến

> **ASP.NET Core 8.0 Web API** — Hệ thống backend đầy đủ tính năng cho nền tảng e-learning, hỗ trợ quản lý khoá học, bài học, quiz, thanh toán, chat thời gian thực và nhiều hơn nữa.

---

## 🛠️ Công nghệ sử dụng

| Công nghệ | Phiên bản | Mục đích |
|---|---|---|
| **ASP.NET Core** | 8.0 | Framework backend chính |
| **Entity Framework Core** | 8.0.14 | ORM, code-first migrations |
| **PostgreSQL** (Npgsql) | 8.0.11 | Cơ sở dữ liệu chính |
| **JWT Bearer** | 8.0.14 | Xác thực & phân quyền |
| **SignalR** | 1.2.0 | Chat & thông báo thời gian thực |
| **Cloudinary** | 1.27.5 | Upload & lưu trữ ảnh |
| **AWS S3** | 4.0.0.5 | Lưu trữ video bài học |
| **MailKit** | 4.11.0 | Gửi email |
| **BCrypt.Net-Next** | 4.0.3 | Mã hoá mật khẩu |
| **Google.Apis.Auth** | 1.69.0 | Đăng nhập bằng Google |
| **NPOI** | 2.7.3 | Xuất/import file Excel |
| **Nanoid** | 3.1.0 | Tạo ID ngắn, duy nhất |
| **Swagger (Swashbuckle)** | 6.6.2 | Tài liệu API tự động |

---

## 🚀 Cài đặt & Chạy dự án

### Yêu cầu
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- PostgreSQL đang chạy (local hoặc cloud)

### 1. Clone project
```bash
git clone <repository-url>
cd api_web_h3
```

### 2. Cấu hình `appsettings.json`
Tạo file `appsettings.json` (đã bị gitignore) với nội dung:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=your_db;Username=your_user;Password=your_password"
  },
  "JwtSettings": {
    "Secret": "your-super-secret-key-at-least-32-chars",
    "Issuer": "API_WebH3",
    "Audience": "API_WebH3_Client"
  },
  "Frontend": {
    "BaseUrl": "http://localhost:5173"
  },
  "CloudinarySettings": {
    "CloudName": "your_cloud_name",
    "ApiKey": "your_api_key",
    "ApiSecret": "your_api_secret"
  }
}
```

### 3. Chạy migration & khởi động
```bash
dotnet ef database update
dotnet run
```

API sẽ chạy tại `https://localhost:5001` (hoặc port được cấu hình).  
Swagger UI: `https://localhost:5001/swagger`

---

## 📁 Cấu trúc thư mục

```
api_web_h3/
│
├── 📄 Program.cs                  # Entry point: đăng ký DI, middleware, CORS, JWT
├── 📄 API_WebH3.csproj            # Cấu hình project & NuGet packages
│
├── 📂 Configurations/             # Các class cấu hình (settings)
│   └── CloudinarySettings.cs      # Model cấu hình Cloudinary
│
├── 📂 Data/                       # Database context
│   └── AppDBContext.cs            # EF Core DbContext, định nghĩa các DbSet
│
├── 📂 Migrations/                 # EF Core migrations (lịch sử thay đổi schema DB)
│
├── 📂 Models/                     # Các entity (bảng cơ sở dữ liệu)
│   ├── User.cs
│   ├── Course.cs
│   ├── Chapter.cs
│   ├── Lesson.cs
│   ├── Quiz.cs
│   ├── Enrollment.cs
│   ├── Order.cs / OrderDetail.cs
│   ├── Payment.cs
│   ├── Review.cs
│   ├── Comment.cs
│   ├── Post.cs
│   ├── Notification.cs
│   ├── Chat.cs / Message.cs
│   ├── Follower.cs
│   ├── Coupon.cs
│   ├── Progress.cs
│   ├── Category.cs
│   ├── LessonApproval.cs
│   ├── Email.cs
│   └── ...
│
├── 📂 DTO/                        # Data Transfer Objects (request/response models)
│   ├── User/                      # DTO liên quan đến người dùng (16 files)
│   ├── Course/                    # DTO khoá học
│   ├── Lesson/                    # DTO bài học
│   ├── Quiz/                      # DTO quiz
│   ├── Order/                     # DTO đơn hàng
│   ├── Enrollment/                # DTO đăng ký khoá học
│   ├── Comment/                   # DTO bình luận
│   ├── Review/                    # DTO đánh giá
│   ├── Notification/              # DTO thông báo
│   └── ...                        # (21 nhóm DTO tổng cộng)
│
├── 📂 Repository/                 # Tầng truy cập dữ liệu (Repository Pattern)
│   ├── I*Repository.cs            # Interface definitions (25 interfaces)
│   └── *Repository.cs             # Implementations (25 implementations)
│
├── 📂 Service/                    # Business logic layer
│   ├── AuthService.cs             # Xác thực, JWT, Google OAuth
│   ├── CourseService.cs           # Logic quản lý khoá học
│   ├── LessonService.cs           # Logic quản lý bài học
│   ├── QuizService.cs             # Logic quiz & chấm điểm
│   ├── OrderService.cs            # Xử lý đơn hàng
│   ├── VnpayService.cs            # Tích hợp VNPay
│   ├── MomoService.cs             # Tích hợp MoMo
│   ├── EnrollmentService.cs       # Logic đăng ký khoá học
│   ├── NotificationService.cs     # Gửi & quản lý thông báo
│   ├── EmailService.cs            # Gửi email thông thường
│   ├── EmailPaymentService.cs     # Email xác nhận thanh toán
│   ├── ContactEmailService.cs     # Email liên hệ
│   ├── FollowerService.cs         # Follow/Unfollow instructor
│   ├── CommentService.cs          # Bình luận bài học/post
│   ├── PostService.cs             # Bài đăng cộng đồng
│   ├── ReviewService.cs           # Đánh giá khoá học
│   ├── ProgressService.cs         # Theo dõi tiến độ học
│   ├── ChatService.cs             # Chat giữa users
│   ├── ImageService.cs            # Upload ảnh (Cloudinary)
│   ├── S3Service.cs               # Upload video (AWS S3)
│   ├── CouponService.cs           # Mã giảm giá
│   ├── InstructorService.cs       # Quản lý giảng viên
│   ├── StudentService.cs          # Quản lý học viên
│   ├── FilterService.cs           # Lọc & tìm kiếm
│   └── ...
│
├── 📂 Controller/                 # API Endpoints (29 controllers)
│   ├── AuthController.cs          # POST /api/auth/login, /register, /google-login
│   ├── CourseController.cs        # CRUD khoá học
│   ├── LessonController.cs        # CRUD bài học
│   ├── ChapterController.cs       # CRUD chương học
│   ├── QuizController.cs          # Quiz & kết quả
│   ├── EnrollmentController.cs    # Đăng ký khoá học
│   ├── OrderController.cs         # Tạo & xem đơn hàng
│   ├── PaymentController.cs       # Xử lý thanh toán
│   ├── MomoController.cs          # Callback MoMo
│   ├── ReviewController.cs        # Đánh giá khoá học
│   ├── CommentController.cs       # Bình luận
│   ├── PostController.cs          # Bài đăng cộng đồng
│   ├── NotificationController.cs  # Thông báo
│   ├── ChatController.cs          # Chat
│   ├── MessageController.cs       # Tin nhắn
│   ├── FollowerController.cs      # Follow instructor
│   ├── ProgressController.cs      # Tiến độ học tập
│   ├── UserController.cs          # Quản lý người dùng
│   ├── InstructorController.cs    # Thông tin giảng viên
│   ├── StudentController.cs       # Thông tin học viên
│   ├── CategoryController.cs      # Danh mục khoá học
│   ├── CouponController.cs        # Mã giảm giá
│   ├── SearchController.cs        # Tìm kiếm
│   ├── FilterController.cs        # Lọc khoá học
│   ├── ImageUploadController.cs   # Upload ảnh
│   ├── LessonApprovalController.cs# Duyệt bài học
│   ├── EmailController.cs         # Gửi email
│   ├── ContactController.cs       # Form liên hệ
│   └── UserNotificationController.cs
│
├── 📂 Hubs/                       # SignalR Hubs
│   └── ChatHub.cs                 # Hub cho chat thời gian thực (/chatHub)
│
├── 📂 Middleware/                 # Custom middleware
│   └── ErrorHandlingMiddleware.cs # Xử lý lỗi toàn cục
│
├── 📂 Helpers/                    # Utility classes
│   ├── AppLogger.cs               # Logging tập trung
│   ├── ExcelExportHelper.cs       # Xuất báo cáo Excel (NPOI)
│   └── IdGenerator.cs             # Tạo ID ngắn (Nanoid)
│
├── 📂 MomoPayment/                # Module tích hợp MoMo
│   └── models/                    # Models cho MoMo API
│
└── 📂 wwwroot/                    # Static files (ảnh, video tĩnh)
```

---

## 🏗️ Kiến trúc

Project tuân theo **Layered Architecture (N-tier)**:

```
Client (React/Frontend)
        │  HTTP / WebSocket
        ▼
┌─────────────────────┐
│    Controller Layer  │  ← Nhận request, validate, trả response
└────────┬────────────┘
         │
┌────────▼────────────┐
│    Service Layer     │  ← Business logic, xử lý nghiệp vụ
└────────┬────────────┘
         │
┌────────▼────────────┐
│  Repository Layer    │  ← Truy vấn DB (Repository Pattern + Interface)
└────────┬────────────┘
         │
┌────────▼────────────┐
│  Database (PostgreSQL)│ ← Lưu trữ dữ liệu qua Entity Framework Core
└─────────────────────┘
```

---

## 🔐 Xác thực & Phân quyền

- **JWT Bearer Token**: Token được gửi qua header `Authorization: Bearer <token>` hoặc cookie `auth_token`
- **Google OAuth**: Đăng nhập bằng tài khoản Google qua `Google.Apis.Auth`
- **BCrypt**: Mã hoá mật khẩu trước khi lưu DB
- **CORS**: Chỉ cho phép origin từ `Frontend:BaseUrl` trong config

---

## 💳 Thanh toán

| Cổng thanh toán | Service | Controller |
|---|---|---|
| **VNPay** | `VnpayService.cs` | `PaymentController.cs` |
| **MoMo** | `MomoService.cs` | `MomoController.cs` |

---

## 📡 Real-time với SignalR

- **ChatHub** (`/chatHub`): Hỗ trợ chat thời gian thực giữa học viên và giảng viên
- Được đăng ký trong `Program.cs` và ánh xạ tại endpoint `/chatHub`

---

## 📧 Hệ thống Email

| Service | Mục đích |
|---|---|
| `EmailService` | Email thông thường (xác nhận tài khoản, đặt lại mật khẩu) |
| `EmailPaymentService` | Xác nhận thanh toán thành công |
| `ContactEmailService` | Xử lý form liên hệ từ người dùng |

---

## ☁️ Lưu trữ Media

| Loại file | Nơi lưu | Service |
|---|---|---|
| Ảnh (avatar, thumbnail) | **Cloudinary** | `ImageService`, `PhotoService` |
| Video bài học | **AWS S3** | `S3Service` |

---

## 📊 Các tính năng chính

- ✅ **Quản lý khoá học**: CRUD khoá học, chương, bài học với duyệt bài (approval flow)
- ✅ **Quiz & kiểm tra**: Tạo quiz, nộp bài, theo dõi kết quả
- ✅ **Đăng ký & thanh toán**: Enroll khoá học, thanh toán VNPay/MoMo, mã coupon
- ✅ **Tiến độ học tập**: Theo dõi bài học đã hoàn thành theo từng học viên
- ✅ **Đánh giá & bình luận**: Review khoá học, comment bài học
- ✅ **Cộng đồng**: Đăng bài viết, follow giảng viên
- ✅ **Chat thời gian thực**: Nhắn tin trực tiếp qua SignalR
- ✅ **Thông báo**: Hệ thống notification cho người dùng
- ✅ **Upload media**: Ảnh lên Cloudinary, video lên AWS S3
- ✅ **Tìm kiếm & lọc**: Tìm kiếm khoá học, lọc theo danh mục/giá/rating
- ✅ **Xuất Excel**: Báo cáo dữ liệu bằng NPOI
- ✅ **Gửi email**: Notification email qua MailKit

---

## 📝 API Documentation

Sau khi chạy ở môi trường Development, truy cập Swagger UI tại:

```
https://localhost:<port>/swagger
```

---

*Made with ❤️ — ASP.NET Core 8.0*
