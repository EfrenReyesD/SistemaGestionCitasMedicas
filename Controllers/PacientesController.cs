using Microsoft.AspNetCore.Mvc;
using SistemaGestionCitasMedicas.Models;

namespace SistemaGestionCitasMedicas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PacientesController : ControllerBase
    {
        private static List<Paciente> _pacientes = new List<Paciente>
        {
            new Paciente
            {
                IdPaciente = DatosMock.IdPaciente1,
                IdUsuario = DatosMock.IdPaciente1,
                Nombre = "María González",
                Email = "maria.gonzalez@email.com",
                PasswordHash = "cGFjMTIz",
                FechaNacimiento = new DateTime(1990, 5, 15),
                Telefono = "+34 600 123 456",
                Rol = "Paciente"
            },
            new Paciente
            {
                IdPaciente = DatosMock.IdPaciente2,
                IdUsuario = DatosMock.IdPaciente2,
                Nombre = "Carlos Rodríguez",
                Email = "carlos.rodriguez@email.com",
                PasswordHash = "cGFjMTIz",
                FechaNacimiento = new DateTime(1979, 8, 22),
                Telefono = "+34 600 234 567",
                Rol = "Paciente"
            }
        };

        public static List<Paciente> ObtenerPacientes() => _pacientes;

        /// <summary>
        /// Obtiene todos los pacientes registrados
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Paciente>> GetPacientes()
        {
            return Ok(_pacientes);
        }

        /// <summary>
        /// Obtiene un paciente específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Paciente> GetPaciente(Guid id)
        {
            var paciente = _pacientes.FirstOrDefault(p => p.IdPaciente == id);
            if (paciente == null)
                return NotFound();

            return Ok(paciente);
        }

        /// <summary>
        /// Registra un nuevo paciente en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<Paciente> PostPaciente(Paciente paciente)
        {
            paciente.IdPaciente = Guid.NewGuid();
            paciente.IdUsuario = Guid.NewGuid();
            paciente.Rol = "Paciente";
            _pacientes.Add(paciente);
            return CreatedAtAction(nameof(GetPaciente), new { id = paciente.IdPaciente }, paciente);
        }

        /// <summary>
        /// Obtiene el historial médico de un paciente
        /// </summary>
        [HttpGet("{id}/historial")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<NotaMedica>> GetHistorial(Guid id)
        {
            var paciente = _pacientes.FirstOrDefault(p => p.IdPaciente == id);
            if (paciente == null)
                return NotFound();

            return Ok(paciente.ObtenerHistorial());
        }
    }
}
