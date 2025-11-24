using Microsoft.AspNetCore.Mvc;
using SistemaGestionCitasMedicas.Models;
using SistemaGestionCitasMedicas.Services;

namespace SistemaGestionCitasMedicas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PacientesController : ControllerBase
    {
        private readonly IPacienteService _pacienteService;
        private readonly ILogger<PacientesController> _logger;

        public PacientesController(IPacienteService pacienteService, ILogger<PacientesController> logger)
        {
            _pacienteService = pacienteService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los pacientes registrados
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Paciente>>> GetPacientes()
        {
            try
            {
                var pacientes = await _pacienteService.ObtenerTodosAsync();
                return Ok(pacientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pacientes");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene un paciente específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Paciente>> GetPaciente(Guid id)
        {
            try
            {
                var paciente = await _pacienteService.ObtenerPacienteAsync(id);
                if (paciente == null)
                    return NotFound(new { mensaje = $"Paciente con ID {id} no encontrado" });

                return Ok(paciente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener paciente {PacienteId}", id);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Registra un nuevo paciente en el sistema
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Paciente>> PostPaciente(Paciente paciente)
        {
            try
            {
                if (paciente == null)
                    return BadRequest(new { mensaje = "Los datos del paciente son requeridos" });

                var pacienteCreado = await _pacienteService.RegistrarPacienteAsync(paciente);
                return CreatedAtAction(nameof(GetPaciente), new { id = pacienteCreado.IdPaciente }, pacienteCreado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar paciente");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene el historial médico de un paciente
        /// </summary>
        [HttpGet("{id}/historial")]
        public async Task<ActionResult<List<NotaMedica>>> GetHistorial(Guid id)
        {
            try
            {
                var historial = await _pacienteService.ObtenerHistorialAsync(id);
                return Ok(historial);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial del paciente {PacienteId}", id);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }
    }
}
