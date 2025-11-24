using Microsoft.EntityFrameworkCore;
using SistemaGestionCitasMedicas.Data;
using SistemaGestionCitasMedicas.Models;

namespace SistemaGestionCitasMedicas.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(ApplicationDbContext context, ILogger<UsuarioService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Usuario> CrearUsuarioAsync(Usuario usuario)
        {
            try
            {
                if (usuario.IdUsuario == Guid.Empty)
                {
                    usuario.IdUsuario = Guid.NewGuid();
                }

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Usuario creado exitosamente: {UsuarioId}", usuario.IdUsuario);
                return usuario;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de BD al crear usuario: {Email}", usuario.Email);
                throw new InvalidOperationException("Error al crear el usuario en la base de datos", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear usuario");
                throw;
            }
        }

        public async Task AsignarRolAsync(Guid idUsuario, string rol)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(idUsuario);
                if (usuario == null)
                {
                    throw new KeyNotFoundException($"Usuario con ID {idUsuario} no encontrado");
                }

                usuario.Rol = rol;
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Rol {Rol} asignado a usuario {UsuarioId}", rol, idUsuario);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar rol al usuario {UsuarioId}", idUsuario);
                throw new InvalidOperationException("Error al asignar el rol", ex);
            }
        }

        public async Task<Usuario?> ObtenerUsuarioAsync(Guid idUsuario)
        {
            try
            {
                return await _context.Usuarios.FindAsync(idUsuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario {UsuarioId}", idUsuario);
                throw new InvalidOperationException("Error al consultar el usuario", ex);
            }
        }

        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            try
            {
                return await _context.Usuarios.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                throw new InvalidOperationException("Error al consultar los usuarios", ex);
            }
        }
    }
}
