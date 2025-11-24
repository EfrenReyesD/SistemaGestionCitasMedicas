namespace SistemaGestionCitasMedicas.Models
{
    public class Asistente
    {
        public Guid IdAsistente { get; set; }
        public Guid IdUsuario { get; set; }
        public string? Turno { get; set; }
        public string? Area { get; set; }
        
        public Usuario? Usuario { get; set; }
    }
}
