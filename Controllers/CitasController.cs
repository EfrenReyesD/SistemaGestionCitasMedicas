using Microsoft.AspNetCore.Mvc;
using SistemaGestionCitasMedicas.Models;

namespace SistemaGestionCitasMedicas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CitasController : ControllerBase
    {
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
            return Ok(CitaManager.ObtenerTodas());
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
                return NotFound();

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
                return Ok(new { mensaje = "Cita reprogramada exitosamente", nuevaFecha });

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
                return Ok(new { mensaje = "Cita cancelada exitosamente" });

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

            return Created(string.Empty, nota);
        }
    }
}
