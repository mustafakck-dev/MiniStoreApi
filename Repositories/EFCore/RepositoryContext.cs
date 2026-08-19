using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Repositories.EFCore;

public class RepositoryContext : IdentityDbContext<User>
{
    public RepositoryContext(
        DbContextOptions<RepositoryContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .Property(product => product.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Order>()
    .HasMany(order => order.OrderItems)
    .WithOne(orderItem => orderItem.Order)
    .HasForeignKey(orderItem => orderItem.OrderId)
    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
    .HasOne(orderItem => orderItem.Product)
    .WithMany(product => product.OrderItems)
    .HasForeignKey(orderItem => orderItem.ProductId)
    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
    .Property(order => order.TotalPrice)
    .HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>()
            .Property(orderItem => orderItem.UnitPrice)
            .HasPrecision(18, 2);
    }
}