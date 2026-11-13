using Microsoft.EntityFrameworkCore;
using KurumsalEnvanterMVC.Models.Entities;

namespace KurumsalEnvanterMVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Asset> Assets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Kategori ve Demirbaş İlişkisi
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Assets)
                .HasForeignKey(a => a.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. Personel ve Demirbaş Zimmet İlişkisi
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.Assets)
                .HasForeignKey(a => a.AssignedEmployeeID)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}