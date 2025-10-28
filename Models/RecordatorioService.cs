namespace SistemaGestionCitasMedicas.Models
{
    public class RecordatorioService
    {
        public void ProgramarRecordatorio(Cita cita, int tiempoAntes)
        {
            var fechaRecordatorio = cita.FechaHora.AddMinutes(-tiempoAntes);
            Console.WriteLine($"Recordatorio programado para {fechaRecordatorio} - Cita: {cita.IdCita}");
        }

        public void EnviarRecordatorio(Cita cita)
        {
            if (cita.Paciente != null)
            {
                Console.WriteLine($"Enviando recordatorio a {cita.Paciente.Email} para cita del {cita.FechaHora}");
            }
        }
    }
}
