namespace SistemaGestionCitasMedicas.Models
{
    public class Paciente : Usuario
    {
        public Guid IdPaciente { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Telefono { get; set; } = string.Empty;
        private List<NotaMedica> _historial = new List<NotaMedica>();

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
