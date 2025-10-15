using Microsoft.EntityFrameworkCore; 
using Hospital_San_Vicente.Models; 

namespace Hospital_San_Vicente.Data 
{
    public class AppDbContext : DbContext 
    { 
        // === CONSTRUCTOR REQUERIDO para la Inyección de Dependencias ===
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Las opciones (cadena de conexión) se pasan aquí desde Program.cs
        }

        // DbSets (Modelos)
        public DbSet<Doctor> Doctors { get; set; } 
        public DbSet<Appointment> Appointments { get; set; } 
        public DbSet<EmailHistory> EmailHistory { get; set; } 
        public DbSet<Patient> Patients { get; set; } 
    
        // ELIMINAR CUALQUIER MÉTODO OnConfiguring O CONSTANTES DE CONEXIÓN DE ESTE ARCHIVO

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        { 
            modelBuilder.Entity<Patient>().HasIndex(p => p.Document).IsUnique(); 
            modelBuilder.Entity<Doctor>().HasIndex(d => d.Document).IsUnique(); 
        } 
    } 
}