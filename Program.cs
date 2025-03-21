using System.Text;
using API_WebH3.Data;
using API_WebH3.Repositories;
using API_WebH3.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 🔹 Lấy chuỗi kết nối từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 🔹 Đăng ký DbContext với PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://localhost:5173") 
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});
// Cấu hình JWT
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

// 🔹 Thêm bộ nhớ cache phân tán trong RAM
builder.Services.AddDistributedMemoryCache(); // Thêm dòng này để cung cấp IDistributedCache

// Đăng ký dịch vụ phiên (Session)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian chờ phiên
    options.Cookie.HttpOnly = true; // Bảo mật
    options.Cookie.IsEssential = true; // Tuân thủ GDPR
});

// Đăng ký các repository và service
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddControllers();
// Cấu hình Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("AllowReactApp");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Sắp xếp middleware theo thứ tự đúng
app.UseRouting(); // 1. Định tuyến
app.UseAuthentication(); // 2. Xác thực (JWT)
app.UseAuthorization(); // 3. Phân quyền
app.UseSession(); // 4. Phiên (Session)

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // 5. Định nghĩa điểm cuối
});

app.Run();