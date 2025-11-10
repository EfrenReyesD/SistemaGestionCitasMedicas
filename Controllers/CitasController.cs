using Microsoft.AspNetCore.Mvc;
using SistemaGestionCitasMedicas.Models;
using Microsoft.Extensions.Caching.Memory;

namespace SistemaGestionCitasMedicas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CitasController : ControllerBase
    {
        private readonly ILogger<CitasController> _logger;
        private readonly IMemoryCache _cache;

        public CitasController(ILogger<CitasController> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        static CitasController()
        {
            var pacientes = PacientesController.ObtenerPacientes();
            var doctores = DoctoresController.ObtenerDoctores();

            if (pacientes.Count >= 2 && doctores.Count >= 2)
            {
                CitaManager.AgregarCita(new Cita
                {
                    IdCita = DatosMock.IdCita1,
                    FechaHora = DateTime.Now.AddDays(5),
                    DuracionMin = 30,
                    Estado = "Programada",
                    Tipo = "Consulta General",
                    Paciente = pacientes[0],
                    Doctor = doctores[0]
                });

                CitaManager.AgregarCita(new Cita
                {
                    IdCita = DatosMock.IdCita2,
                    FechaHora = DateTime.Now.AddDays(7),
                    DuracionMin = 45,
                    Estado = "Programada",
                    Tipo = "Control Pediátrico",
                    Paciente = pacientes[1],
                    Doctor = doctores[1]
                });

                CitaManager.AgregarCita(new Cita
                {
                    IdCita = DatosMock.IdCita3,
                    FechaHora = DateTime.Now.AddDays(10),
                    DuracionMin = 30,
                    Estado = "Programada",
                    Tipo = "Revisión Cardiológica",
                    Paciente = pacientes[0],
                    Doctor = doctores[0]
                });
            }
        }

        /// <summary>
        /// Obtiene todas las citas programadas
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Cita>> GetCitas()
        {
            const string cacheKey = "todas_las_citas";
            
            if (!_cache.TryGetValue(cacheKey, out List<Cita>? citas))
            {
                citas = CitaManager.ObtenerTodas();
                _cache.Set(cacheKey, citas, TimeSpan.FromMinutes(5));
                _logger.LogInformation("Lista de citas cargada en caché");
            }
            else
            {
                _logger.LogInformation("Lista de citas obtenida desde caché");
            }

            return Ok(citas);
        }

        /// <summary>
        /// Obtiene una cita específica por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Cita> GetCita(Guid id)
        {
            var cita = CitaManager.ObtenerPorId(id);
            if (cita == null)
            {
                _logger.LogWarning("Cita con ID {CitaId} no encontrada", id);
                return NotFound();
            }

            return Ok(cita);
        }

        /// <summary>
        /// Programa una nueva cita médica
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<Cita> PostCita(Cita cita)
        {
            CitaManager.AgregarCita(cita);
            _cache.Remove("todas_las_citas");
            _logger.LogInformation("Cita creada: {CitaId} para paciente {PacienteId} con doctor {DoctorId}", 
                cita.IdCita, cita.Paciente?.IdPaciente, cita.Doctor?.IdUsuario);
            return CreatedAtAction(nameof(GetCita), new { id = cita.IdCita }, cita);
        }

        /// <summary>
        /// Reprograma una cita existente
        /// </summary>
        [HttpPut("{id}/reprogramar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Reprogramar(Guid id, [FromBody] DateTime nuevaFecha)
        {
            var cita = CitaManager.ObtenerPorId(id);
            if (cita == null)
                return NotFound();

            if (cita.Reprogramar(nuevaFecha))
            {
                _cache.Remove("todas_las_citas");
                _logger.LogInformation("Cita {CitaId} reprogramada de {FechaAnterior} a {FechaNueva}", 
                    id, cita.FechaHora, nuevaFecha);
                return Ok(new { mensaje = "Cita reprogramada exitosamente", nuevaFecha });
            }

            _logger.LogWarning("No se pudo reprogramar la cita {CitaId}", id);
            return BadRequest(new { mensaje = "No se puede reprogramar esta cita" });
        }

        /// <summary>
        /// Cancela una cita existente
        /// </summary>
        [HttpPut("{id}/cancelar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Cancelar(Guid id)
        {
            var cita = CitaManager.ObtenerPorId(id);
            if (cita == null)
                return NotFound();

            if (cita.Cancelar())
            {
                _cache.Remove("todas_las_citas");
                _logger.LogWarning("Cita {CitaId} cancelada", id);
                return Ok(new { mensaje = "Cita cancelada exitosamente" });
            }

            return BadRequest(new { mensaje = "Esta cita ya está cancelada" });
        }

        /// <summary>
        /// Agrega una nota médica a una cita
        /// </summary>
        [HttpPost("{id}/nota")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult AgregarNota(Guid id, [FromBody] NotaMedica nota)
        {
            var cita = CitaManager.ObtenerPorId(id);
            if (cita == null)
                return NotFound();

            nota.IdNota = Guid.NewGuid();
            nota.Fecha = DateTime.Now;
            cita.AgregarNota(nota);

            _logger.LogInformation("Nota médica agregada a cita {CitaId}", id);

            return Created(string.Empty, nota);
        }
    }
}
