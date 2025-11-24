using SistemaGestionCitasMedicas.Models;

namespace SistemaGestionCitasMedicas.Services
{
    public interface IUsuarioService
    {
        Task<Usuario> CrearUsuarioAsync(Usuario usuario);
        Task AsignarRolAsync(Guid idUsuario, string rol);
        Task<Usuario?> ObtenerUsuarioAsync(Guid idUsuario);
        Task<List<Usuario>> ObtenerTodosAsync();
    }
}
