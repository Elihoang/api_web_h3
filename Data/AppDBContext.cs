using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace API_WebH3.Data;

public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Course> Courses { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Order>  Orders { get; set; }
    public DbSet<OrderDetail>  OrderDetails { get; set; }
    public DbSet<Post>  Posts { get; set; }
    public DbSet<Progress>  Progresses { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<LessonApproval> LessonApprovals { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<UserNotification> UserNotifications { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Follower> Followers { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<UserQuizAnswer> UserQuizAnswers { get; set; }
    
    public DbSet<Assignment> Assignments { get; set; }
    
    public DbSet<Email> Emails { get; set; }
    
    public DbSet<AssignmentStudent> AssignmentStudents { get; set; }
}