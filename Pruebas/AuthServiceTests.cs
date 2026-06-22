namespace Pruebas;

using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Interfaces;
using Capa_Servicios;
using Moq;
using Xunit;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioAdminRepository> _usuarioRepoMock;
    private readonly AuthService _service;

    private const string TestPassword = "MiContraseña123";
    private readonly string _testPasswordHash;

    public AuthServiceTests()
    {
        _usuarioRepoMock = new Mock<IUsuarioAdminRepository>();
        _service = new AuthService(_usuarioRepoMock.Object);
        _testPasswordHash = BCrypt.Net.BCrypt.HashPassword(TestPassword, workFactor: 12);
    }

    [Fact]
    public async Task AutenticarAsync_CredencialesValidas_RetornaSuccess()
    {
        // Arrange
        var usuario = new UsuarioAdmin
        {
            IdUsuario = 1,
            NombreUsuario = "admin",
            PasswordHash = _testPasswordHash
        };

        _usuarioRepoMock
            .Setup(r => r.ObtenerPorNombreUsuarioAsync("admin"))
            .ReturnsAsync(usuario);

        // Act
        var result = await _service.AutenticarAsync("admin", TestPassword);

        // Assert
        Assert.True(result.IsAuthenticated);
        Assert.Equal("admin", result.NombreUsuario);
    }

    [Fact]
    public async Task AutenticarAsync_ContraseñaIncorrecta_RetornaFailed()
    {
        // Arrange
        var usuario = new UsuarioAdmin
        {
            IdUsuario = 1,
            NombreUsuario = "admin",
            PasswordHash = _testPasswordHash
        };

        _usuarioRepoMock
            .Setup(r => r.ObtenerPorNombreUsuarioAsync("admin"))
            .ReturnsAsync(usuario);

        // Act
        var result = await _service.AutenticarAsync("admin", "contraseñaIncorrecta");

        // Assert
        Assert.False(result.IsAuthenticated);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public async Task AutenticarAsync_UsuarioInexistente_RetornaFailed()
    {
        // Arrange
        _usuarioRepoMock
            .Setup(r => r.ObtenerPorNombreUsuarioAsync("noexiste"))
            .ReturnsAsync((UsuarioAdmin?)null);

        // Act
        var result = await _service.AutenticarAsync("noexiste", "cualquierPassword");

        // Assert
        Assert.False(result.IsAuthenticated);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public async Task AutenticarAsync_MensajesErrorIdenticos_ParaContraseñaIncorrectaYUsuarioInexistente()
    {
        // Arrange
        var usuario = new UsuarioAdmin
        {
            IdUsuario = 1,
            NombreUsuario = "admin",
            PasswordHash = _testPasswordHash
        };

        _usuarioRepoMock
            .Setup(r => r.ObtenerPorNombreUsuarioAsync("admin"))
            .ReturnsAsync(usuario);

        _usuarioRepoMock
            .Setup(r => r.ObtenerPorNombreUsuarioAsync("noexiste"))
            .ReturnsAsync((UsuarioAdmin?)null);

        // Act
        var resultWrongPassword = await _service.AutenticarAsync("admin", "contraseñaIncorrecta");
        var resultNonExistentUser = await _service.AutenticarAsync("noexiste", "cualquierPassword");

        // Assert - messages must be identical for security
        Assert.Equal(resultWrongPassword.ErrorMessage, resultNonExistentUser.ErrorMessage);
    }
}
