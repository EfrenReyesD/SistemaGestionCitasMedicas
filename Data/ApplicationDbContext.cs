using Microsoft.EntityFrameworkCore;
using SistemaGestionCitasMedicas.Models;

namespace SistemaGestionCitasMedicas.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Paciente> Pacientes => Set<Paciente>();
        public DbSet<Doctor> Doctores => Set<Doctor>();
        public DbSet<Asistente> Asistentes => Set<Asistente>();
        public DbSet<Cita> Citas => Set<Cita>();
        public DbSet<NotaMedica> NotasMedicas => Set<NotaMedica>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.IdUsuario);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Rol).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.ToTable("Pacientes");
                entity.HasKey(e => e.IdPaciente);
                entity.Property(e => e.FechaNacimiento).HasColumnType("date");
                entity.Property(e => e.Telefono).IsRequired().HasMaxLength(20);
                entity.HasOne(p => p.Usuario)
                    .WithMany()
                    .HasForeignKey(p => p.IdUsuario)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Ignore(e => e.Nombre);
                entity.Ignore(e => e.Email);
                entity.Ignore(e => e.PasswordHash);
                entity.Ignore(e => e.Rol);
            });

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.ToTable("Doctores");
                entity.HasKey(e => e.IdDoctor);
                entity.Property(e => e.NumeroCedula).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Especialidad).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.NumeroCedula).IsUnique();
                entity.HasOne(d => d.Usuario)
                    .WithMany()
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Asistente>(entity =>
            {
                entity.ToTable("Asistentes");
                entity.HasKey(e => e.IdAsistente);
                entity.Property(e => e.Turno).HasMaxLength(20);
                entity.Property(e => e.Area).HasMaxLength(100);
                entity.HasOne(a => a.Usuario)
                    .WithMany()
                    .HasForeignKey(a => a.IdUsuario)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Cita>(entity =>
            {
                entity.ToTable("Citas");
                entity.HasKey(e => e.IdCita);
                entity.Property(e => e.FechaHora).HasColumnType("datetime2(7)");
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(50).HasDefaultValue("Programada");
                entity.Property(e => e.Tipo).IsRequired().HasMaxLength(100);
                entity.Property<Guid>("IdPaciente");
                entity.Property<Guid>("IdDoctor");
                entity.HasOne(c => c.Paciente)
                    .WithMany()
                    .HasForeignKey("IdPaciente")
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(c => c.Doctor)
                    .WithMany()
                    .HasForeignKey("IdDoctor")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<NotaMedica>(entity =>
            {
                entity.ToTable("NotasMedicas");
                entity.HasKey(e => e.IdNota);
                entity.Property(e => e.Fecha).HasColumnType("datetime2(7)");
                entity.Property(e => e.Texto).IsRequired();
                entity.Property(e => e.Diagnostico);
                entity.Property<Guid?>("IdCita");
                entity.Property<Guid>("IdPaciente");
                entity.Property<Guid?>("IdDoctorAutor");
            });
        }
    }
}
