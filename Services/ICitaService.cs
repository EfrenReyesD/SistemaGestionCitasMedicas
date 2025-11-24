using SistemaGestionCitasMedicas.Models;

namespace SistemaGestionCitasMedicas.Services
{
    public interface ICitaService
    {
        Task<Cita> ProgramarCitaAsync(Cita cita, Guid idPaciente, Guid idDoctor);
        Task<Cita?> ObtenerCitaAsync(Guid idCita);
        Task<List<Cita>> ObtenerTodasAsync();
        Task<bool> ReprogramarCitaAsync(Guid idCita, DateTime nuevaFecha);
        Task<bool> CancelarCitaAsync(Guid idCita, string? usuario = null);
        Task<NotaMedica> AgregarNotaAsync(Guid idCita, NotaMedica nota, Guid idDoctorAutor);
    }
}
