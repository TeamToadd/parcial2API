using Microsoft.EntityFrameworkCore;
using Parcial2.Models;

namespace Parcial2.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.HasIndex(u => u.Email).IsUnique();
        });

        model.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.Property(p => p.Price).HasPrecision(18, 2);
            e.HasOne(p => p.CompanyUser)
             .WithMany()
             .HasForeignKey(p => p.CompanyUserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        model.Entity<Order>(e =>
        {
            e.ToTable("Orders");
            e.Property(o => o.Total).HasPrecision(18, 2);
            e.HasOne(o => o.ClientUser)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.ClientUserId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(o => o.CompanyUser)
                .WithMany()
                .HasForeignKey(o => o.CompanyUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        model.Entity<OrderItem>(e =>
        {
            e.ToTable("OrderItems");
            e.Property(i => i.UnitPrice).HasPrecision(18, 2);
        });

        model.Entity<Review>(e =>
        {
            e.ToTable("Reviews");
            e.HasIndex(r => new { r.ProductId, r.ClientUserId }).IsUnique();
        });

        // SEED sin Admin
        var empHash = Hash("Passw0rd!");
        var cliHash = Hash("Passw0rd!");

        model.Entity<User>().HasData(
            new User { Id = 2, Email = "BigBoys_E1@example.com", PasswordHash = empHash, Role = Role.Empresa, CompanyName = "BigBoys" },
            new User { Id = 3, Email = "cliente@demo.com", PasswordHash = cliHash, Role = Role.Cliente, Name = "Cliente Demo" }
        );

        model.Entity<Product>().HasData(
            new Product { Id = 1, CompanyUserId = 2, Name = "Burger Clásica", Description = "200g carne", Price = 35.00m, Stock = 20 },
            new Product { Id = 2, CompanyUserId = 2, Name = "Papas Rusty", Description = "Porción", Price = 15.00m, Stock = 50 }
        );
    }

    // Hash SHA256 simple
    public static string Hash(string text)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(text)));
    }
}
