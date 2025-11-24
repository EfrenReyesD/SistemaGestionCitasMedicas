namespace SistemaGestionCitasMedicas.Models
{
    public class Doctor
    {
        public Guid IdDoctor { get; set; }
        public Guid IdUsuario { get; set; }
        public string NumeroCedula { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        
        public Usuario? Usuario { get; set; }

        public NotaMedica RegistrarNota(Cita cita, NotaMedica nota)
        {
            nota.IdNota = Guid.NewGuid();
            nota.Fecha = DateTime.Now;
            cita.AgregarNota(nota);
            return nota;
        }
    }
}
