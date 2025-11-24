using Microsoft.AspNetCore.Mvc;
using SistemaGestionCitasMedicas.Models;
using SistemaGestionCitasMedicas.Services;

namespace SistemaGestionCitasMedicas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(IUsuarioService usuarioService, ILogger<UsuariosController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los usuarios del sistema
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            try
            {
                var usuarios = await _usuarioService.ObtenerTodosAsync();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene un usuario específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(Guid id)
        {
            try
            {
                var usuario = await _usuarioService.ObtenerUsuarioAsync(id);
                if (usuario == null)
                    return NotFound(new { mensaje = $"Usuario con ID {id} no encontrado" });

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario {UsuarioId}", id);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea un nuevo usuario en el sistema
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            try
            {
                if (usuario == null)
                    return BadRequest(new { mensaje = "Los datos del usuario son requeridos" });

                var nuevoUsuario = await _usuarioService.CrearUsuarioAsync(usuario);
                return CreatedAtAction(nameof(GetUsuario), new { id = nuevoUsuario.IdUsuario }, nuevoUsuario);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Asigna un rol a un usuario existente
        /// </summary>
        [HttpPut("{id}/rol")]
        public async Task<ActionResult> AsignarRol(Guid id, [FromBody] string rol)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rol))
                    return BadRequest(new { mensaje = "El rol es requerido" });

                await _usuarioService.AsignarRolAsync(id, rol);
                return Ok(new { mensaje = "Rol asignado exitosamente", rol });
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
                _logger.LogError(ex, "Error al asignar rol a usuario {UsuarioId}", id);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }
    }
}
