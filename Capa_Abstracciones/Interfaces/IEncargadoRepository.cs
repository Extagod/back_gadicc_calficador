namespace Capa_Abstracciones.Interfaces;

using Capa_Abstracciones.Entities;

public interface IEncargadoRepository
{
    Task<Encargado?> ObtenerPorIdAsync(int id);
    Task<Encargado?> ObtenerPorTokenQRAsync(string tokenQR);
    Task<IEnumerable<Encargado>> ObtenerTodosAsync();
    Task<Encargado> AgregarAsync(Encargado encargado);
    Task ActualizarAsync(Encargado encargado);
    Task EliminarAsync(Encargado encargado);
}
