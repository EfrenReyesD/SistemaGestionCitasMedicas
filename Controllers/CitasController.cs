using Microsoft.AspNetCore.Mvc;
using SistemaGestionCitasMedicas.Models;
using SistemaGestionCitasMedicas.Services;
using Microsoft.Extensions.Caching.Memory;

namespace SistemaGestionCitasMedicas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CitasController : ControllerBase
    {
        private readonly ICitaService _citaService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CitasController> _logger;
        private const string CacheKey = "todas_las_citas";

        public CitasController(ICitaService citaService, IMemoryCache cache, ILogger<CitasController> logger)
        {
            _citaService = citaService;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las citas programadas
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cita>>> GetCitas()
        {
            try
            {
                if (!_cache.TryGetValue(CacheKey, out List<Cita>? citas))
                {
                    citas = await _citaService.ObtenerTodasAsync();
                    _cache.Set(CacheKey, citas, TimeSpan.FromMinutes(5));
                }
                return Ok(citas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener citas");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene una cita específica por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Cita>> GetCita(Guid id)
        {
            try
            {
                var cita = await _citaService.ObtenerCitaAsync(id);
                if (cita == null)
                    return NotFound(new { mensaje = $"Cita con ID {id} no encontrada" });

                return Ok(cita);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cita {CitaId}", id);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Programa una nueva cita médica
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Cita>> PostCita(Cita cita)
        {
            try
            {
                if (cita?.Paciente == null || cita.Doctor == null)
                    return BadRequest(new { mensaje = "La cita debe tener un paciente y un doctor asignados" });

                var citaCreada = await _citaService.ProgramarCitaAsync(cita, cita.Paciente.IdPaciente, cita.Doctor.IdUsuario);
                _cache.Remove(CacheKey);
                
                return CreatedAtAction(nameof(GetCita), new { id = citaCreada.IdCita }, citaCreada);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al programar cita");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Reprograma una cita existente
        /// </summary>
        [HttpPut("{id}/reprogramar")]
        public async Task<ActionResult> Reprogramar(Guid id, [FromBody] DateTime nuevaFecha)
        {
            try
            {
                var resultado = await _citaService.ReprogramarCitaAsync(id, nuevaFecha);
                if (resultado)
                {
                    _cache.Remove(CacheKey);
                    return Ok(new { mensaje = "Cita reprogramada exitosamente", nuevaFecha });
                }
                return BadRequest(new { mensaje = "No se puede reprogramar esta cita" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reprogramar cita {CitaId}", id);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Cancela una cita programada
        /// </summary>
        [HttpPut("{id}/cancelar")]
        public async Task<ActionResult> Cancelar(Guid id)
        {
            try
            {
                var resultado = await _citaService.CancelarCitaAsync(id);
                if (resultado)
                {
                    _cache.Remove(CacheKey);
                    return Ok(new { mensaje = "Cita cancelada exitosamente" });
                }
                return BadRequest(new { mensaje = "No se puede cancelar esta cita" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar cita {CitaId}", id);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Agrega una nota médica a una cita
        /// </summary>
        [HttpPost("{id}/nota")]
        public async Task<ActionResult<NotaMedica>> PostNota(Guid id, [FromBody] NotaMedica nota, [FromQuery] Guid idDoctorAutor)
        {
            try
            {
                if (nota == null || idDoctorAutor == Guid.Empty)
                    return BadRequest(new { mensaje = "Los datos de la nota y el ID del doctor son requeridos" });

                var notaCreada = await _citaService.AgregarNotaAsync(id, nota, idDoctorAutor);
                return CreatedAtAction(nameof(GetCita), new { id }, notaCreada);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar nota a cita {CitaId}", id);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }
    }
}
