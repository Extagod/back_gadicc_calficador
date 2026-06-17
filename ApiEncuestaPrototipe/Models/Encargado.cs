namespace ApiEncuestaPrototipe.Models
{
    public class Encargado
    {
        public int IdEncargado { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public int? Edad { get; set; }
        public string? Cargo { get; set; }
        public ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();


        // QR único generado al crear el encargado
        public string? CodigoQR { get; set; }       // imagen en Base64
        public string? TokenQR { get; set; }         // GUID único para la URL del QR
    }
}
