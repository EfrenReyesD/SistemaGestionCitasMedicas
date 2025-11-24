using Microsoft.EntityFrameworkCore;
using SistemaGestionCitasMedicas.Data;
using SistemaGestionCitasMedicas.Models;

namespace SistemaGestionCitasMedicas.Services
{
    public class PacienteService : IPacienteService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PacienteService> _logger;

        public PacienteService(ApplicationDbContext context, ILogger<PacienteService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Paciente> RegistrarPacienteAsync(Paciente paciente)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Crear Usuario primero
                var usuario = new Usuario
                {
                    IdUsuario = Guid.NewGuid(),
                    Nombre = paciente.Nombre,
                    Email = paciente.Email,
                    PasswordHash = paciente.PasswordHash,
                    Rol = "Paciente"
                };
                
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Crear Paciente vinculado al Usuario
                paciente.IdUsuario = usuario.IdUsuario;
                paciente.IdPaciente = Guid.NewGuid();
                paciente.Usuario = usuario;
                
                _context.Pacientes.Add(paciente);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                
                _logger.LogInformation("Paciente registrado exitosamente: {PacienteId}", paciente.IdPaciente);
                return paciente;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error de BD al registrar paciente: {Email}", paciente.Email);
                throw new InvalidOperationException("Error al registrar el paciente en la base de datos", ex);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error inesperado al registrar paciente");
                throw;
            }
        }

        public async Task<Paciente?> ObtenerPacienteAsync(Guid idPaciente)
        {
            try
            {
                // Incluir datos del Usuario relacionado
                var paciente = await _context.Pacientes
                    .Include(p => p.Usuario)
                    .FirstOrDefaultAsync(p => p.IdPaciente == idPaciente);
                
                return paciente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener paciente {PacienteId}", idPaciente);
                throw new InvalidOperationException("Error al consultar el paciente", ex);
            }
        }

        public async Task<List<Paciente>> ObtenerTodosAsync()
        {
            try
            {
                // Incluir datos del Usuario relacionado
                var pacientes = await _context.Pacientes
                    .Include(p => p.Usuario)
                    .ToListAsync();
                
                return pacientes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los pacientes");
                throw new InvalidOperationException("Error al consultar los pacientes", ex);
            }
        }

        public async Task<List<NotaMedica>> ObtenerHistorialAsync(Guid idPaciente)
        {
            try
            {
                var paciente = await _context.Pacientes
                    .FirstOrDefaultAsync(p => p.IdPaciente == idPaciente);
                    
                if (paciente == null)
                {
                    throw new KeyNotFoundException($"Paciente con ID {idPaciente} no encontrado");
                }

                // Ejecutar el stored procedure sp_ObtenerHistorialPaciente
                var historial = await _context.NotasMedicas
                    .FromSqlRaw("EXEC sp_ObtenerHistorialPaciente @p0", idPaciente)
                    .ToListAsync();

                return historial;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial del paciente {PacienteId}", idPaciente);
                throw new InvalidOperationException("Error al consultar el historial médico", ex);
            }
        }
    }
}
