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
        public NotaMedica? Nota { get; set; }

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
            Nota = nota;
            if (Paciente != null)
            {
                Paciente.AgregarNotaHistorial(nota);
            }
        }
    }

    public static class CitaManager
    {
        private static List<Cita> _citas = new List<Cita>();

        public static void AgregarCita(Cita cita)
        {
            if (cita.IdCita == Guid.Empty)
            {
                cita.IdCita = Guid.NewGuid();
            }
            _citas.Add(cita);
        }

        public static List<Cita> ObtenerCitasPorDoctor(Guid idDoctor, DateTime fecha)
        {
            return _citas.Where(c => c.Doctor != null && 
                                     c.Doctor.IdUsuario == idDoctor && 
                                     c.FechaHora.Date == fecha.Date)
                         .ToList();
        }

        public static List<Cita> ObtenerTodas()
        {
            return _citas;
        }

        public static Cita? ObtenerPorId(Guid idCita)
        {
            return _citas.FirstOrDefault(c => c.IdCita == idCita);
        }
    }
}
