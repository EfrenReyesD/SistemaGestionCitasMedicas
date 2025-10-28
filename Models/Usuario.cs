namespace SistemaGestionCitasMedicas.Models
{
    public class Usuario
    {
        public Guid IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;

        public bool Autenticar(string email, string password)
        {
            return Email == email && PasswordHash == HashPassword(password);
        }

        public bool CambiarPassword(string oldPass, string newPass)
        {
            if (PasswordHash == HashPassword(oldPass))
            {
                PasswordHash = HashPassword(newPass);
                return true;
            }
            return false;
        }

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}
