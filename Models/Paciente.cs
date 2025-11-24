namespace SistemaGestionCitasMedicas.Models
{
    // Paciente NO hereda de Usuario - tienen tablas separadas en la BD
    public class Paciente
    {
        public Guid IdPaciente { get; set; }
        public Guid IdUsuario { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Telefono { get; set; } = string.Empty;
        
        // Navegación a Usuario (relación 1:1)
        public Usuario? Usuario { get; set; }
        
        private List<NotaMedica> _historial = new List<NotaMedica>();

        // Propiedades de conveniencia que delegan a Usuario
        public string Nombre 
        { 
            get => Usuario?.Nombre ?? string.Empty;
            set { if (Usuario != null) Usuario.Nombre = value; }
        }
        
        public string Email 
        { 
            get => Usuario?.Email ?? string.Empty;
            set { if (Usuario != null) Usuario.Email = value; }
        }
        
        public string PasswordHash 
        { 
            get => Usuario?.PasswordHash ?? string.Empty;
            set { if (Usuario != null) Usuario.PasswordHash = value; }
        }
        
        public string Rol 
        { 
            get => Usuario?.Rol ?? string.Empty;
            set { if (Usuario != null) Usuario.Rol = value; }
        }

        public List<NotaMedica> ObtenerHistorial()
        {
            return _historial;
        }

        public void AgregarNotaHistorial(NotaMedica nota)
        {
            _historial.Add(nota);
        }
    }
}
