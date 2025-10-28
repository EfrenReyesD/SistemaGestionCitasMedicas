using Microsoft.AspNetCore.Mvc;
using SistemaGestionCitasMedicas.Models;

namespace SistemaGestionCitasMedicas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsuariosController : ControllerBase
    {
        private static UsuarioManager _usuarioManager = new UsuarioManager();
        
        static UsuariosController()
        {
            _usuarioManager.CrearUsuario(new Usuario
            {
                Nombre = "Admin Sistema",
                Email = "admin@hospital.com",
                PasswordHash = "YWRtaW4xMjM=",
                Rol = "Administrador"
            });
            
            _usuarioManager.CrearUsuario(new Usuario
            {
                Nombre = "Recepcionista María",
                Email = "recepcion@hospital.com",
                PasswordHash = "cmVjZXAxMjM=",
                Rol = "Asistente"
            });
        }

        /// <summary>
        /// Obtiene todos los usuarios del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Usuario>> GetUsuarios()
        {
            return Ok(_usuarioManager.ObtenerTodos());
        }

        /// <summary>
        /// Obtiene un usuario específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Usuario> GetUsuario(Guid id)
        {
            var usuario = _usuarioManager.ObtenerUsuario(id);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        /// <summary>
        /// Crea un nuevo usuario en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<Usuario> PostUsuario(Usuario usuario)
        {
            var nuevoUsuario = _usuarioManager.CrearUsuario(usuario);
            return CreatedAtAction(nameof(GetUsuario), new { id = nuevoUsuario.IdUsuario }, nuevoUsuario);
        }

        /// <summary>
        /// Asigna un rol a un usuario existente
        /// </summary>
        [HttpPut("{id}/rol")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult AsignarRol(Guid id, [FromBody] string rol)
        {
            var usuario = _usuarioManager.ObtenerUsuario(id);
            if (usuario == null)
                return NotFound();

            _usuarioManager.AsignarRol(id, rol);
            return Ok();
        }
    }
}
