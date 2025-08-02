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
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<EmpleadoEstado> EmpleadosEstado { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Empleado
            modelBuilder.Entity<Empleado>(entity =>
            {
                entity.ToTable("Empleado");
                entity.HasKey(e => e.id_empleado);
                entity.Property(e => e.nombres).IsRequired().HasMaxLength(100);
                entity.Property(e => e.apellidos).IsRequired().HasMaxLength(100);
                entity.Property(e => e.telefono).HasMaxLength(20);
                entity.Property(e => e.user).IsRequired().HasMaxLength(50);
                entity.Property(e => e.password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.fecha_contratacion).IsRequired();

                // Clave foránea hacia Cargo
                entity.HasOne(e => e.Cargo)
                      .WithMany(c => c.Empleados)
                      .HasForeignKey(e => e.id_cargo)
                      .OnDelete(DeleteBehavior.Restrict);

                // Clave foránea hacia EmpleadoEstado
                entity.HasOne(e => e.EmpleadoEstado)
                      .WithMany(est => est.Empleados)
                      .HasForeignKey(e => e.id_empleado_estado)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Cargo
            modelBuilder.Entity<Cargo>(entity =>
            {
                entity.ToTable("Cargo");
                entity.HasKey(c => c.id_cargo);
                entity.Property(c => c.cargo).IsRequired().HasMaxLength(100);
            });

            // Configuración de EmpleadoEstado
            modelBuilder.Entity<EmpleadoEstado>(entity =>
            {
                entity.ToTable("EmpleadoEstado");
                entity.HasKey(est => est.id_empleado_estado);
                entity.Property(est => est.estado).IsRequired().HasMaxLength(50);
            });
        }
    }
}