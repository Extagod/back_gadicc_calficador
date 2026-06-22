namespace Capa_Abstracciones.Interfaces;

using Capa_Abstracciones.Entities;

public interface ICalificacionRepository
{
    Task<Calificacion> AgregarAsync(Calificacion calificacion);
    Task<IEnumerable<Calificacion>> ObtenerPorEncargadoIdAsync(int idEncargado);
    Task<IEnumerable<Calificacion>> ObtenerPorEncargadoYRangoFechasAsync(
        int idEncargado, DateTime fechaInicio, DateTime fechaFin);
}
