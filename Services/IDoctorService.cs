using SistemaGestionCitasMedicas.Models;

namespace SistemaGestionCitasMedicas.Services
{
    public interface IDoctorService
    {
        Task<Doctor> RegistrarDoctorAsync(Doctor doctor);
        Task<Doctor?> ObtenerDoctorAsync(Guid idDoctor);
        Task<List<Doctor>> ObtenerTodosAsync();
        Task<List<Cita>> ObtenerAgendaAsync(Guid idDoctor, DateTime fecha);
    }
}
