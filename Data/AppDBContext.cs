using API_WebH3.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace API_WebH3.Data;

public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
}