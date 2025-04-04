using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebBanCa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
public class ApplicationDbContext : IdentityDbContext<NewUserModel>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
   base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)"); // Chỉ định kiểu dữ liệu chính xác

        base.OnModelCreating(modelBuilder);

        List<IdentityRole> roles = new List<IdentityRole>
        {
            new IdentityRole
            {
                Id = "A1D2F3E4-5678-90AB-CDEF-1234567890AB",
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {
                Id = "B2E3F4G5-6789-01AB-CDEF-2345678901BC",
                Name = "User",
                NormalizedName = "USER"
            },
            new IdentityRole
            {
                Id = "C3F4G5H6-7890-12AB-CDEF-3456789012CD",
                Name = "Employee",
                NormalizedName = "EMPLOYEE"
            }
        };
        modelBuilder.Entity<IdentityRole>().HasData(roles);

    }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

}
