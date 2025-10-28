using Microsoft.AspNetCore.Mvc;
using SistemaGestionCitasMedicas.Models;

namespace SistemaGestionCitasMedicas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DoctoresController : ControllerBase
    {
        private static List<Doctor> _doctores = new List<Doctor>
        {
            new Doctor
            {
                IdUsuario = DatosMock.IdDoctor1,
                Nombre = "Dr. Juan Pérez",
                Email = "juan.perez@hospital.com",
                PasswordHash = "ZG9jMTIz",
                NumeroCedula = "MED-12345",
                Especialidad = "Cardiología",
                Rol = "Doctor"
            },
            new Doctor
            {
                IdUsuario = DatosMock.IdDoctor2,
                Nombre = "Dra. Laura Sánchez",
                Email = "laura.sanchez@hospital.com",
                PasswordHash = "ZG9jMTIz",
                NumeroCedula = "MED-67890",
                Especialidad = "Pediatría",
                Rol = "Doctor"
            }
        };

        public static List<Doctor> ObtenerDoctores() => _doctores;

        /// <summary>
        /// Obtiene todos los doctores registrados
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Doctor>> GetDoctores()
        {
            return Ok(_doctores);
        }

        /// <summary>
        /// Obtiene un doctor específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Doctor> GetDoctor(Guid id)
        {
            var doctor = _doctores.FirstOrDefault(d => d.IdUsuario == id);
            if (doctor == null)
                return NotFound();

            return Ok(doctor);
        }

        /// <summary>
        /// Registra un nuevo doctor en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<Doctor> PostDoctor(Doctor doctor)
        {
            doctor.IdUsuario = Guid.NewGuid();
            doctor.Rol = "Doctor";
            _doctores.Add(doctor);
            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.IdUsuario }, doctor);
        }

        /// <summary>
        /// Obtiene la agenda de citas de un doctor para una fecha específica
        /// </summary>
        [HttpGet("{id}/agenda")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<Cita>> GetAgenda(Guid id, [FromQuery] DateTime fecha)
        {
            var doctor = _doctores.FirstOrDefault(d => d.IdUsuario == id);
            if (doctor == null)
                return NotFound();

            return Ok(doctor.VerAgenda(fecha));
        }
    }
}
