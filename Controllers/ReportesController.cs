using Microsoft.AspNetCore.Mvc;
using SistemaGestionCitasMedicas.Models;

namespace SistemaGestionCitasMedicas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ReportesController : ControllerBase
    {
        private static Reporte _reporteService = new Reporte();

        /// <summary>
        /// Genera un reporte de consultas realizadas en un rango de fechas
        /// </summary>
        [HttpGet("consultas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<Reporte.ReporteConsultas> GenerarReporteConsultas(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            var reporte = _reporteService.GenerarReporteConsultas(fechaInicio, fechaFin);
            return Ok(reporte);
        }

        /// <summary>
        /// Genera un reporte de citas canceladas en un rango de fechas
        /// </summary>
        [HttpGet("cancelaciones")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<Reporte.ReporteCancelaciones> GenerarReporteCancelaciones(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin)
        {
            var reporte = _reporteService.GenerarReporteCancelaciones(fechaInicio, fechaFin);
            return Ok(reporte);
        }
    }
}
