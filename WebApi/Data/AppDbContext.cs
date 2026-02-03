using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebApi.Models.Empleados;

namespace WebApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSets para tus entidades
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeRoles> EmployeeRoles { get; set; }
        public DbSet<EmployeeStatuses> EmployeeStatuses { get; set; }

        public DbSet<Brands> Brands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");
                entity.HasKey(e => e.EmployeeId);
                entity.Property(e => e.names).IsRequired().HasMaxLength(100);
                entity.Property(e => e.lastnames).IsRequired().HasMaxLength(100);
                entity.Property(e => e.phone).HasMaxLength(20);
                entity.Property(e => e.user).IsRequired().HasMaxLength(50);
                entity.Property(e => e.password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.hiring_date).IsRequired();
                entity.Property(e => e.email).HasMaxLength(100);
                entity.Property(e => e.url_photo).HasMaxLength(255);

                entity.HasOne(e => e.EmployeeRole)
                      .WithMany(c => c.Employees)
                      .HasForeignKey(e => e.EmployeeRoleId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EmployeeStatus)
                      .WithMany(est => est.Employees)
                      .HasForeignKey(e => e.EmployeeStatusId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<EmployeeRoles> (entity =>
            {
                entity.ToTable("EmployeeRoles");
                entity.HasKey(c => c.EmployeeRoleId);
                entity.Property(c => c.name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.status).HasDefaultValue(true);
            });

            modelBuilder.Entity<EmployeeStatuses>(entity =>
            {
                entity.ToTable("EmployeeStatuses");
                entity.HasKey(est => est.EmployeeStatusId);
                entity.Property(est => est.name).IsRequired().HasMaxLength(100);
                entity.Property(est => est.status).HasDefaultValue(true);
            });

            modelBuilder.Entity<Brands>(entity =>
            {
                entity.ToTable("brands");
                entity.HasKey(b => b.id_brand);
                entity.Property(b => b.name).IsRequired().HasMaxLength(100);
                entity.Property(b => b.name).IsRequired(); 
                entity.Property(b => b.logo_url).HasMaxLength(250);
                entity.Property(b => b.contact_phone).HasMaxLength(15);
                entity.Property(b => b.contact_email).HasMaxLength(100);
            });
        }
    }
}
