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
    }
}
