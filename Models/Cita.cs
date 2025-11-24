namespace SistemaGestionCitasMedicas.Models
{
    public class Cita
    {
        public Guid IdCita { get; set; }
        public DateTime FechaHora { get; set; }
        public int DuracionMin { get; set; }
        public string Estado { get; set; } = "Programada";
        public string Tipo { get; set; } = string.Empty;
        public Paciente? Paciente { get; set; }
        public Doctor? Doctor { get; set; }

        public bool Reprogramar(DateTime nuevaFecha)
        {
            if (Estado == "Programada")
            {
                FechaHora = nuevaFecha;
                return true;
            }
            return false;
        }

        public bool Cancelar()
        {
            if (Estado != "Cancelada")
            {
                Estado = "Cancelada";
                return true;
            }
            return false;
        }

        public void AgregarNota(NotaMedica nota)
        {
            if (Paciente != null)
            {
                Paciente.AgregarNotaHistorial(nota);
            }
        }
    }
}
