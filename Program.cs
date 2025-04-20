using System.Text;
using API_WebH3.Data;
using API_WebH3.Repository;
using API_WebH3.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Lấy chuỗi kết nối từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 🔹 Đăng ký DbContext với PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 🔹 Thêm Controllers (Fix lỗi InvalidOperationException)
builder.Services.AddControllers();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<IChapterRepository, ChapterRepository>();
builder.Services.AddScoped<ChapterService>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<LessonService>();
builder.Services.AddScoped<ILessonApprovalRepository, LessonApprovalRepository>();
builder.Services.AddScoped<LessonApprovalService>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ReviewService>();

builder.Services.AddScoped<IProgressRepository, ProgressRepository>();
builder.Services.AddScoped<ProgressService>();

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<PostService>();

// 🔹 Cấu hình CORS cho React (hoặc các frontend khác)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins(builder.Configuration["Urls:Frontend"].ToString())
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// 🔹 Cấu hình JWT Authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Secret"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// 🔹 Thêm Authorization (Fix lỗi thiếu Authorization)
builder.Services.AddAuthorization();

// 🔹 Thêm bộ nhớ cache phân tán trong RAM (hỗ trợ session)
builder.Services.AddDistributedMemoryCache();

// 🔹 Đăng ký Session (hỗ trợ lưu trạng thái người dùng)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian chờ phiên
    options.Cookie.HttpOnly = true; // Bảo mật
    options.Cookie.IsEssential = true; // Tuân thủ GDPR
});


builder.Services.AddControllers();
// Cấu hình Swagger/OpenAPI

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔹 Middleware xử lý CORS
app.UseCors("AllowReactApp");

// 🔹 Cấu hình Swagger khi ở môi trường Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔹 Các Middleware quan trọng (theo thứ tự chuẩn)
app.UseHttpsRedirection();
app.UseStaticFiles(); // Cho phép truy cập wwwroot/uploads

app.UseRouting();         // 1️⃣ Định tuyến
app.UseAuthentication();  // 2️⃣ Xác thực (JWT)
app.UseAuthorization();   // 3️⃣ Phân quyền
app.UseSession();         // 4️⃣ Phiên (Session)


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // 5️⃣ Định nghĩa API controllers
});

// 🔹 Chạy ứng dụng
app.Run();
