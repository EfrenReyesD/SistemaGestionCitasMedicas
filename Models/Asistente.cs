namespace SistemaGestionCitasMedicas.Models
{
    public class Asistente : Usuario
    {
        public void GestionarCita(Cita cita)
        {
            CitaManager.AgregarCita(cita);
        }
    }
}
