using Microsoft.EntityFrameworkCore;
using SistemaGestionCitasMedicas.Data;
using SistemaGestionCitasMedicas.Models;

namespace SistemaGestionCitasMedicas.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(ApplicationDbContext context, ILogger<DoctorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Doctor> RegistrarDoctorAsync(Doctor doctor)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (doctor.IdDoctor == Guid.Empty)
                    doctor.IdDoctor = Guid.NewGuid();
                
                if (doctor.IdUsuario == Guid.Empty)
                    doctor.IdUsuario = Guid.NewGuid();

                if (doctor.Usuario != null)
                {
                    doctor.Usuario.IdUsuario = doctor.IdUsuario;
                    doctor.Usuario.Rol = "Doctor";
                    _context.Usuarios.Add(doctor.Usuario);
                }

                _context.Doctores.Add(doctor);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return doctor;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al registrar doctor");
                throw new InvalidOperationException("Error al registrar el doctor", ex);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al registrar doctor");
                throw;
            }
        }

        public async Task<Doctor?> ObtenerDoctorAsync(Guid idDoctor)
        {
            try
            {
                return await _context.Doctores
                    .Include(d => d.Usuario)
                    .FirstOrDefaultAsync(d => d.IdDoctor == idDoctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener doctor {DoctorId}", idDoctor);
                throw new InvalidOperationException("Error al consultar el doctor", ex);
            }
        }

        public async Task<List<Doctor>> ObtenerTodosAsync()
        {
            try
            {
                return await _context.Doctores
                    .Include(d => d.Usuario)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener doctores");
                throw new InvalidOperationException("Error al consultar los doctores", ex);
            }
        }

        public async Task<List<Cita>> ObtenerAgendaAsync(Guid idDoctor, DateTime fecha)
        {
            try
            {
                var doctor = await _context.Doctores.FindAsync(idDoctor);
                if (doctor == null)
                    throw new KeyNotFoundException($"Doctor con ID {idDoctor} no encontrado");

                return await _context.Citas
                    .FromSqlRaw("EXEC sp_ObtenerAgendaDoctor @p0, @p1", idDoctor, fecha.Date)
                    .Include(c => c.Paciente)
                    .Include(c => c.Doctor)
                    .ToListAsync();
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener agenda del doctor {DoctorId}", idDoctor);
                throw new InvalidOperationException("Error al consultar la agenda", ex);
            }
        }
    }
}
