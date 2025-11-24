using SistemaGestionCitasMedicas.Models;

namespace SistemaGestionCitasMedicas.Services
{
    public interface IReporteService
    {
        Task<Reporte.ReporteConsultas> GenerarReporteConsultasAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<Reporte.ReporteCancelaciones> GenerarReporteCancelacionesAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}
