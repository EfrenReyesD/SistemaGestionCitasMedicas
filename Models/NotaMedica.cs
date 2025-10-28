namespace SistemaGestionCitasMedicas.Models
{
    public class NotaMedica
    {
        public Guid IdNota { get; set; }
        public DateTime Fecha { get; set; }
        public string Texto { get; set; } = string.Empty;
        public string Diagnostico { get; set; } = string.Empty;

        public void EditarTexto(string nuevoTexto)
        {
            Texto = nuevoTexto;
        }
    }
}
