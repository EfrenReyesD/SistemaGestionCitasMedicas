namespace SistemaGestionCitasMedicas.Models
{
    public class Doctor : Usuario
    {
        public string NumeroCedula { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;

        public List<Cita> VerAgenda(DateTime fecha)
        {
            return CitaManager.ObtenerCitasPorDoctor(IdUsuario, fecha);
        }

        public NotaMedica RegistrarNota(Cita cita, NotaMedica nota)
        {
            nota.IdNota = Guid.NewGuid();
            nota.Fecha = DateTime.Now;
            cita.AgregarNota(nota);
            return nota;
        }
    }
}
