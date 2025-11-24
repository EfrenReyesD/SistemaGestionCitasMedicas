using Microsoft.AspNetCore.Mvc;
using SistemaGestionCitasMedicas.Models;
using SistemaGestionCitasMedicas.Services;

namespace SistemaGestionCitasMedicas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DoctoresController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly ILogger<DoctoresController> _logger;

        public DoctoresController(IDoctorService doctorService, ILogger<DoctoresController> logger)
        {
            _doctorService = doctorService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los doctores registrados
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctores()
        {
            try
            {
                var doctores = await _doctorService.ObtenerTodosAsync();
                return Ok(doctores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener doctores");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene un doctor específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> GetDoctor(Guid id)
        {
            try
            {
                var doctor = await _doctorService.ObtenerDoctorAsync(id);
                if (doctor == null)
                    return NotFound(new { mensaje = $"Doctor con ID {id} no encontrado" });

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener doctor {DoctorId}", id);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Registra un nuevo doctor en el sistema
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Doctor>> PostDoctor(Doctor doctor)
        {
            try
            {
                if (doctor == null)
                    return BadRequest(new { mensaje = "Los datos del doctor son requeridos" });

                var doctorCreado = await _doctorService.RegistrarDoctorAsync(doctor);
                return CreatedAtAction(nameof(GetDoctor), new { id = doctorCreado.IdDoctor }, doctorCreado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar doctor");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene la agenda de citas de un doctor para una fecha específica
        /// </summary>
        [HttpGet("{id}/agenda")]
        public async Task<ActionResult<List<Cita>>> GetAgenda(Guid id, [FromQuery] DateTime fecha)
        {
            try
            {
                var agenda = await _doctorService.ObtenerAgendaAsync(id, fecha);
                return Ok(agenda);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener agenda del doctor {DoctorId}", id);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }
    }
}
