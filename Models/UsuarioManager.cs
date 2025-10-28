namespace SistemaGestionCitasMedicas.Models
{
    public class UsuarioManager
    {
        private static List<Usuario> _usuarios = new List<Usuario>();

        public Usuario CrearUsuario(Usuario usuario)
        {
            usuario.IdUsuario = Guid.NewGuid();
            _usuarios.Add(usuario);
            return usuario;
        }

        public void AsignarRol(Guid idUsuario, string rol)
        {
            var usuario = _usuarios.FirstOrDefault(u => u.IdUsuario == idUsuario);
            if (usuario != null)
            {
                usuario.Rol = rol;
            }
        }

        public Usuario? ObtenerUsuario(Guid idUsuario)
        {
            return _usuarios.FirstOrDefault(u => u.IdUsuario == idUsuario);
        }

        public List<Usuario> ObtenerTodos()
        {
            return _usuarios;
        }
    }
}
