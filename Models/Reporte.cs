namespace SistemaGestionCitasMedicas.Models
{
    public class Reporte
    {
        public class ReporteConsultas
        {
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public int TotalConsultas { get; set; }
            public List<Cita> Consultas { get; set; } = new List<Cita>();
        }

        public class ReporteCancelaciones
        {
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public int TotalCancelaciones { get; set; }
            public List<Cita> Cancelaciones { get; set; } = new List<Cita>();
        }

        public ReporteConsultas GenerarReporteConsultas(DateTime fechaIni, DateTime fechaFin)
        {
            var citas = CitaManager.ObtenerTodas()
                .Where(c => c.FechaHora >= fechaIni && c.FechaHora <= fechaFin && c.Estado != "Cancelada")
                .ToList();

            return new ReporteConsultas
            {
                FechaInicio = fechaIni,
                FechaFin = fechaFin,
                TotalConsultas = citas.Count,
                Consultas = citas
            };
        }

        public ReporteCancelaciones GenerarReporteCancelaciones(DateTime fechaIni, DateTime fechaFin)
        {
            var citasCanceladas = CitaManager.ObtenerTodas()
                .Where(c => c.FechaHora >= fechaIni && c.FechaHora <= fechaFin && c.Estado == "Cancelada")
                .ToList();

            return new ReporteCancelaciones
            {
                FechaInicio = fechaIni,
                FechaFin = fechaFin,
                TotalCancelaciones = citasCanceladas.Count,
                Cancelaciones = citasCanceladas
            };
        }
    }
}
