using Microsoft.EntityFrameworkCore;
using SistemaGestionCitasMedicas.Data;
using SistemaGestionCitasMedicas.Models;
using Microsoft.Data.SqlClient;

namespace SistemaGestionCitasMedicas.Services
{
    public class CitaService : ICitaService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CitaService> _logger;

        public CitaService(ApplicationDbContext context, ILogger<CitaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Cita> ProgramarCitaAsync(Cita cita, Guid idPaciente, Guid idDoctor)
        {
            try
            {
                if (cita.IdCita == Guid.Empty)
                    cita.IdCita = Guid.NewGuid();

                // Verificar que el paciente existe
                var paciente = await _context.Pacientes.FindAsync(idPaciente);
                if (paciente == null)
                    throw new KeyNotFoundException($"Paciente con ID {idPaciente} no encontrado");

                // Verificar que el doctor existe
                var doctor = await _context.Doctores.FindAsync(idDoctor);
                if (doctor == null)
                    throw new KeyNotFoundException($"Doctor con ID {idDoctor} no encontrado");

                cita.Paciente = paciente;
                cita.Doctor = doctor;

                _context.Citas.Add(cita);
                
                // El trigger TR_Citas_ValidarConflictoHorario validará automáticamente
                await _context.SaveChangesAsync();
                
                return cita;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Message.Contains("ya tiene una cita programada"))
            {
                // Capturar error del trigger de conflicto de horario
                throw new InvalidOperationException("El doctor ya tiene una cita programada en ese horario", ex);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al programar cita");
                throw;
            }
        }

        public async Task<Cita?> ObtenerCitaAsync(Guid idCita)
        {
            try
            {
                return await _context.Citas
                    .Include(c => c.Paciente)
                    .Include(c => c.Doctor)
                    .FirstOrDefaultAsync(c => c.IdCita == idCita);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cita {CitaId}", idCita);
                throw new InvalidOperationException("Error al consultar la cita", ex);
            }
        }

        public async Task<List<Cita>> ObtenerTodasAsync()
        {
            try
            {
                return await _context.Citas
                    .Include(c => c.Paciente)
                    .Include(c => c.Doctor)
                    .OrderByDescending(c => c.FechaHora)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas");
                throw new InvalidOperationException("Error al consultar las citas", ex);
            }
        }

        public async Task<bool> ReprogramarCitaAsync(Guid idCita, DateTime nuevaFecha)
        {
            try
            {
                // Usar el stored procedure sp_ReprogramarCita
                var parametros = new[]
                {
                    new SqlParameter("@IdCita", idCita),
                    new SqlParameter("@NuevaFechaHora", nuevaFecha),
                    new SqlParameter("@Usuario", (object?)null ?? DBNull.Value)
                };

                var resultado = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC @returnValue = sp_ReprogramarCita @IdCita, @NuevaFechaHora, @Usuario",
                    parametros);

                return resultado == 0;
            }
            catch (SqlException ex) when (ex.Message.Contains("no existe"))
            {
                throw new KeyNotFoundException($"Cita con ID {idCita} no encontrada");
            }
            catch (SqlException ex) when (ex.Message.Contains("Solo se pueden reprogramar"))
            {
                throw new InvalidOperationException("Solo se pueden reprogramar citas en estado Programada");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reprogramar cita {CitaId}", idCita);
                throw new InvalidOperationException("Error al reprogramar la cita", ex);
            }
        }

        public async Task<bool> CancelarCitaAsync(Guid idCita, string? usuario = null)
        {
            try
            {
                // Usar el stored procedure sp_CancelarCita
                var parametros = new[]
                {
                    new SqlParameter("@IdCita", idCita),
                    new SqlParameter("@Usuario", (object?)usuario ?? DBNull.Value)
                };

                var resultado = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC @returnValue = sp_CancelarCita @IdCita, @Usuario",
                    parametros);

                return resultado == 0;
            }
            catch (SqlException ex) when (ex.Message.Contains("no existe"))
            {
                throw new KeyNotFoundException($"Cita con ID {idCita} no encontrada");
            }
            catch (SqlException ex) when (ex.Message.Contains("ya está cancelada"))
            {
                throw new InvalidOperationException("La cita ya está cancelada");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar cita {CitaId}", idCita);
                throw new InvalidOperationException("Error al cancelar la cita", ex);
            }
        }

        public async Task<NotaMedica> AgregarNotaAsync(Guid idCita, NotaMedica nota, Guid idDoctorAutor)
        {
            try
            {
                var cita = await ObtenerCitaAsync(idCita);
                if (cita?.Paciente == null)
                    throw new KeyNotFoundException($"Cita con ID {idCita} no encontrada");

                nota.IdNota = Guid.NewGuid();
                nota.Fecha = DateTime.Now;

                // Agregar usando el contexto
                _context.NotasMedicas.Add(nota);
                
                // Configurar las propiedades de sombra
                _context.Entry(nota).Property("IdCita").CurrentValue = idCita;
                _context.Entry(nota).Property("IdPaciente").CurrentValue = cita.Paciente.IdPaciente;
                _context.Entry(nota).Property("IdDoctorAutor").CurrentValue = idDoctorAutor;

                await _context.SaveChangesAsync();

                return nota;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar nota a cita {CitaId}", idCita);
                throw new InvalidOperationException("Error al agregar la nota médica", ex);
            }
        }
    }
}
