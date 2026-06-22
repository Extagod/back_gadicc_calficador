namespace Capa_Abstracciones.Common;

public class AuthResult
{
    public bool IsAuthenticated { get; private set; }
    public string? NombreUsuario { get; private set; }
    public string? ErrorMessage { get; private set; }

    public static AuthResult Success(string nombreUsuario) =>
        new() { IsAuthenticated = true, NombreUsuario = nombreUsuario };

    public static AuthResult Failed(string message) =>
        new() { IsAuthenticated = false, ErrorMessage = message };
}
