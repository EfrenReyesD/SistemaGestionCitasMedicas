using SistemaGestionCitasMedicas.Models;

namespace SistemaGestionCitasMedicas.Services
{
    public interface IPacienteService
    {
        Task<Paciente> RegistrarPacienteAsync(Paciente paciente);
        Task<Paciente?> ObtenerPacienteAsync(Guid idPaciente);
        Task<List<Paciente>> ObtenerTodosAsync();
        Task<List<NotaMedica>> ObtenerHistorialAsync(Guid idPaciente);
    }
}
