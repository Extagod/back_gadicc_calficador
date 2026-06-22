namespace Capa_Abstracciones.Interfaces;

using Capa_Abstracciones.Common;

public interface IAuthService
{
    Task<AuthResult> AutenticarAsync(string nombreUsuario, string contraseña);
}
