namespace Capa_Servicios;

using Capa_Abstracciones.Common;
using Capa_Abstracciones.Interfaces;

public class AuthService : IAuthService
{
    private readonly IUsuarioAdminRepository _usuarioAdminRepository;

    public AuthService(IUsuarioAdminRepository usuarioAdminRepository)
    {
        _usuarioAdminRepository = usuarioAdminRepository;
    }

    public async Task<AuthResult> AutenticarAsync(string nombreUsuario, string contraseña)
    {
        if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(contraseña))
        {
            return AuthResult.Failed("Credenciales incorrectas.");
        }

        var usuario = await _usuarioAdminRepository.ObtenerPorNombreUsuarioAsync(nombreUsuario);

        if (usuario is null)
        {
            // Generic message to avoid revealing that user doesn't exist
            return AuthResult.Failed("Credenciales incorrectas.");
        }

        // Verify password with BCrypt (cost factor >= 12 expected in stored hash)
        bool passwordValid = BCrypt.Net.BCrypt.Verify(contraseña, usuario.PasswordHash);

        if (!passwordValid)
        {
            return AuthResult.Failed("Credenciales incorrectas.");
        }

        return AuthResult.Success(usuario.NombreUsuario);
    }
}
