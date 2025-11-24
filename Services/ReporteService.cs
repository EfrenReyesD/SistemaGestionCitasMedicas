using Microsoft.EntityFrameworkCore;
using SistemaGestionCitasMedicas.Data;
using SistemaGestionCitasMedicas.Models;

namespace SistemaGestionCitasMedicas.Services
{
    public class ReporteService : IReporteService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReporteService> _logger;

        public ReporteService(ApplicationDbContext context, ILogger<ReporteService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Reporte.ReporteConsultas> GenerarReporteConsultasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                // Usar stored procedure sp_ReporteConsultas
                var consultas = await _context.Citas
                    .FromSqlRaw("EXEC sp_ReporteConsultas @p0, @p1", fechaInicio.Date, fechaFin.Date)
                    .Include(c => c.Paciente)
                    .Include(c => c.Doctor)
                    .ToListAsync();

                _logger.LogInformation("Reporte de consultas generado: {Count} registros entre {FechaInicio} y {FechaFin}", 
                    consultas.Count, fechaInicio, fechaFin);

                return new Reporte.ReporteConsultas
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    TotalConsultas = consultas.Count,
                    Consultas = consultas
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de consultas");
                throw new InvalidOperationException("Error al generar el reporte de consultas", ex);
            }
        }

        public async Task<Reporte.ReporteCancelaciones> GenerarReporteCancelacionesAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                // Usar stored procedure sp_ReporteCancelaciones
                var cancelaciones = await _context.Citas
                    .FromSqlRaw("EXEC sp_ReporteCancelaciones @p0, @p1", fechaInicio.Date, fechaFin.Date)
                    .Include(c => c.Paciente)
                    .Include(c => c.Doctor)
                    .ToListAsync();

                _logger.LogInformation("Reporte de cancelaciones generado: {Count} registros entre {FechaInicio} y {FechaFin}", 
                    cancelaciones.Count, fechaInicio, fechaFin);

                return new Reporte.ReporteCancelaciones
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    TotalCancelaciones = cancelaciones.Count,
                    Cancelaciones = cancelaciones
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de cancelaciones");
                throw new InvalidOperationException("Error al generar el reporte de cancelaciones", ex);
            }
        }
    }
}
