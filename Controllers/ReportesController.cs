using Microsoft.AspNetCore.Mvc;
using SistemaGestionCitasMedicas.Models;
using SistemaGestionCitasMedicas.Services;

namespace SistemaGestionCitasMedicas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _reporteService;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(IReporteService reporteService, ILogger<ReportesController> logger)
        {
            _reporteService = reporteService;
            _logger = logger;
        }

        /// <summary>
        /// Genera un reporte de consultas realizadas en un rango de fechas
        /// </summary>
        [HttpGet("consultas")]
        public async Task<ActionResult<Reporte.ReporteConsultas>> GenerarReporteConsultas(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            try
            {
                var reporte = await _reporteService.GenerarReporteConsultasAsync(fechaInicio, fechaFin);
                return Ok(reporte);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de consultas");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Genera un reporte de citas canceladas en un rango de fechas
        /// </summary>
        [HttpGet("cancelaciones")]
        public async Task<ActionResult<Reporte.ReporteCancelaciones>> GenerarReporteCancelaciones(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            try
            {
                var reporte = await _reporteService.GenerarReporteCancelacionesAsync(fechaInicio, fechaFin);
                return Ok(reporte);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de cancelaciones");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }
    }
}
