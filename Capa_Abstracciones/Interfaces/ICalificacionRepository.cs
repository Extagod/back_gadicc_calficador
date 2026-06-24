namespace Capa_Abstracciones.Interfaces;

using Capa_Abstracciones.Entities;

public interface ICalificacionRepository
{
    Task<Calificacion> AgregarAsync(Calificacion calificacion);
    Task<IEnumerable<Calificacion>> ObtenerPorEmpleadoAsync(string cedula);
    Task<IEnumerable<Calificacion>> ObtenerPorEmpleadoYRangoFechasAsync(
        string cedula, DateTime fechaInicio, DateTime fechaFin);
    Task<IEnumerable<Calificacion>> ObtenerTodasConEmpleadoAsync();
}
