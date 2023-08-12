#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
namespace _6_Products_and_Categories.Models;
public class MyContext : DbContext 
{   
    public MyContext(DbContextOptions options) : base(options) { }

    public DbSet<Product> Products { get; set; } 
    public DbSet<Category> Categories { get; set; } 
    public DbSet<Association> Associations { get; set; } 
}