using System.Text;
using API_WebH3.Data;
using API_WebH3.Repository;
using API_WebH3.Service;
using API_WebH3.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Lấy chuỗi kết nối từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 🔹 Đăng ký DbContext với PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 🔹 Thêm Controllers và hỗ trợ JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// 🔹 Đăng ký các dịch vụ
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
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailService>();


builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<CommentService>();


builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<CouponService>();

builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<MessageService>();

builder.Services.AddScoped<IFollowerRepository, FollowerRepository>();
builder.Services.AddScoped<FollowerService>();

builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<EnrollmentService>();

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<NotificationService>();

builder.Services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
builder.Services.AddScoped<UserNotificationService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<VnpayService>(); 
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<EmailPaymentService>();

// 🔹 Cấu hình CORS cho React (hoặc các frontend khác)



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        var frontendUrl = builder.Configuration["Frontend:BaseUrl"];
        Console.WriteLine($"CORS Frontend URL: {frontendUrl}");
        if (!string.IsNullOrEmpty(frontendUrl))
        {
            policy.WithOrigins(frontendUrl)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
        else
        {
            Console.WriteLine("Warning: Frontend:BaseUrl is not configured!");
        }
    });
});


// 🔹 Cấu hình JWT Authentication với cookie
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
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["auth_token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    });

// 🔹 Thêm Authorization
builder.Services.AddAuthorization();

// 🔹 Thêm bộ nhớ cache phân tán và Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 🔹 Cấu hình Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔹 Middleware xử lý lỗi chi tiết trong Development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");
// 🔹 Các Middleware
app.UseHttpsRedirection(); // Bật lại HTTPS redirection
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();