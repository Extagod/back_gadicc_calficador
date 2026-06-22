namespace Pruebas;

using Capa_Abstracciones.Common;
using Capa_Abstracciones.DTOs;
using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Enums;
using Capa_Abstracciones.Interfaces;
using Capa_Servicios;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

/// <summary>
/// Tests for the logic used in Panel_Admin (through the service layer).
/// Validates login, funcionario creation, QR generation, and calificaciones filtering.
/// </summary>
public class PanelLogicTests
{
    #region Login Tests

    [Fact]
    public async Task Login_CredencialesValidas_RetornaAutenticado()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123", workFactor: 12);
        var usuario = new UsuarioAdmin
        {
            IdUsuario = 1,
            NombreUsuario = "encargado1",
            PasswordHash = passwordHash
        };

        var repoMock = new Mock<IUsuarioAdminRepository>();
        repoMock.Setup(r => r.ObtenerPorNombreUsuarioAsync("encargado1"))
            .ReturnsAsync(usuario);

        var authService = new AuthService(repoMock.Object);

        // Act
        var result = await authService.AutenticarAsync("encargado1", "password123");

        // Assert
        Assert.True(result.IsAuthenticated);
        Assert.Equal("encargado1", result.NombreUsuario);
    }

    [Fact]
    public async Task Login_CredencialesInvalidas_RetornaFailed()
    {
        // Arrange
        var repoMock = new Mock<IUsuarioAdminRepository>();
        repoMock.Setup(r => r.ObtenerPorNombreUsuarioAsync("usuario"))
            .ReturnsAsync((UsuarioAdmin?)null);

        var authService = new AuthService(repoMock.Object);

        // Act
        var result = await authService.AutenticarAsync("usuario", "wrongpassword");

        // Assert
        Assert.False(result.IsAuthenticated);
        Assert.NotNull(result.ErrorMessage);
    }

    #endregion

    #region Funcionario Creation Tests

    [Fact]
    public async Task CrearFuncionario_DatosValidos_RetornaSuccess()
    {
        // Arrange
        var encargadoRepoMock = new Mock<IEncargadoRepository>();
        var qrServiceMock = new Mock<IQRService>();
        var configMock = new Mock<IConfiguration>();

        configMock.Setup(c => c["AppSettings:FrontendUrl"]).Returns("http://localhost:5173");
        qrServiceMock.Setup(q => q.GenerarQRBase64(It.IsAny<string>())).Returns("base64QR");
        encargadoRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Encargado>()))
            .ReturnsAsync((Encargado e) => { e.IdEncargado = 1; return e; });

        var service = new EncargadoService(encargadoRepoMock.Object, qrServiceMock.Object, configMock.Object);
        var dto = new CrearEncargadoDto { Nombre = "Carlos", Apellido = "Gómez", Cargo = "Atención" };

        // Act
        var result = await service.CrearEncargadoAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Carlos", result.Value!.Nombre);
        Assert.Equal("Gómez", result.Value.Apellido);
    }

    [Fact]
    public async Task CrearFuncionario_NombreVacio_ServiceProcessesWithTrim()
    {
        // Arrange
        var encargadoRepoMock = new Mock<IEncargadoRepository>();
        var qrServiceMock = new Mock<IQRService>();
        var configMock = new Mock<IConfiguration>();

        configMock.Setup(c => c["AppSettings:FrontendUrl"]).Returns("http://localhost:5173");
        qrServiceMock.Setup(q => q.GenerarQRBase64(It.IsAny<string>())).Returns("base64QR");
        encargadoRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Encargado>()))
            .ReturnsAsync((Encargado e) => { e.IdEncargado = 1; return e; });

        var service = new EncargadoService(encargadoRepoMock.Object, qrServiceMock.Object, configMock.Object);
        var dto = new CrearEncargadoDto { Nombre = "  ", Apellido = "López" };

        // Act - The service trims the value. In the full stack, validation would
        // happen at the UI layer (Panel_Admin) before reaching the service.
        var result = await service.CrearEncargadoAsync(dto);

        // Assert - Service processes the request; Panel_Admin handles empty validation
        Assert.True(result.IsSuccess);
        Assert.Equal("", result.Value!.Nombre); // Trimmed whitespace becomes empty
    }

    #endregion

    #region QR Generation Tests

    [Fact]
    public async Task GeneracionQR_Produce32HexCharTokenQRYCodigoQRNoVacio()
    {
        // Arrange
        var encargadoRepoMock = new Mock<IEncargadoRepository>();
        var qrServiceMock = new Mock<IQRService>();
        var configMock = new Mock<IConfiguration>();

        configMock.Setup(c => c["AppSettings:FrontendUrl"]).Returns("http://localhost:5173");
        qrServiceMock.Setup(q => q.GenerarQRBase64(It.IsAny<string>())).Returns("someBase64Content");
        encargadoRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Encargado>()))
            .ReturnsAsync((Encargado e) => { e.IdEncargado = 1; return e; });

        var service = new EncargadoService(encargadoRepoMock.Object, qrServiceMock.Object, configMock.Object);
        var dto = new CrearEncargadoDto { Nombre = "Ana", Apellido = "Martínez" };

        // Act
        var result = await service.CrearEncargadoAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value!.TokenQR);
        Assert.Equal(32, result.Value.TokenQR!.Length);
        Assert.True(result.Value.TokenQR.All(c => "0123456789abcdef".Contains(c)));
        Assert.False(string.IsNullOrEmpty(result.Value.CodigoQR));
    }

    [Fact]
    public async Task GeneracionQR_DosGeneraciones_ProducenTokenQRDistintos()
    {
        // Arrange
        var encargadoRepoMock = new Mock<IEncargadoRepository>();
        var qrServiceMock = new Mock<IQRService>();
        var configMock = new Mock<IConfiguration>();

        configMock.Setup(c => c["AppSettings:FrontendUrl"]).Returns("http://localhost:5173");
        qrServiceMock.Setup(q => q.GenerarQRBase64(It.IsAny<string>())).Returns("base64QR");
        encargadoRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Encargado>()))
            .ReturnsAsync((Encargado e) => { e.IdEncargado = 1; return e; });

        var service = new EncargadoService(encargadoRepoMock.Object, qrServiceMock.Object, configMock.Object);

        // Act
        var result1 = await service.CrearEncargadoAsync(new CrearEncargadoDto { Nombre = "A", Apellido = "B" });
        var result2 = await service.CrearEncargadoAsync(new CrearEncargadoDto { Nombre = "C", Apellido = "D" });

        // Assert
        Assert.NotEqual(result1.Value!.TokenQR, result2.Value!.TokenQR);
    }

    #endregion

    #region Calificaciones Filter Tests

    [Fact]
    public async Task CalificacionesFiltro_PorRangoFechas_RetornaSoloCoincidentes()
    {
        // Arrange
        var calificacionRepoMock = new Mock<ICalificacionRepository>();
        var encargadoRepoMock = new Mock<IEncargadoRepository>();

        var encargado = new Encargado { IdEncargado = 1, Nombre = "Test", Apellido = "User" };
        encargadoRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(encargado);

        var fechaInicio = new DateTime(2024, 1, 1);
        var fechaFin = new DateTime(2024, 1, 31);

        var calificacionesFiltradas = new List<Calificacion>
        {
            new() { IdCalificacion = 1, IdEncargado = 1, Valor = ValorCalificacion.Excelente, FechaHora = new DateTime(2024, 1, 15) },
            new() { IdCalificacion = 2, IdEncargado = 1, Valor = ValorCalificacion.Buena, FechaHora = new DateTime(2024, 1, 20) }
        };

        calificacionRepoMock
            .Setup(r => r.ObtenerPorEncargadoYRangoFechasAsync(1, fechaInicio, fechaFin))
            .ReturnsAsync(calificacionesFiltradas);

        var service = new CalificacionService(calificacionRepoMock.Object, encargadoRepoMock.Object);

        // Act
        var result = await service.ObtenerPorEncargadoYRangoAsync(1, fechaInicio, fechaFin);

        // Assert
        Assert.True(result.IsSuccess);
        var calificaciones = result.Value!.ToList();
        Assert.Equal(2, calificaciones.Count);
        Assert.All(calificaciones, c =>
        {
            Assert.True(c.FechaHora >= fechaInicio && c.FechaHora <= fechaFin);
        });
    }

    [Fact]
    public async Task CalificacionesFiltro_SinResultados_RetornaColeccionVacia()
    {
        // Arrange
        var calificacionRepoMock = new Mock<ICalificacionRepository>();
        var encargadoRepoMock = new Mock<IEncargadoRepository>();

        var encargado = new Encargado { IdEncargado = 1, Nombre = "Test", Apellido = "User" };
        encargadoRepoMock.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(encargado);

        var fechaInicio = new DateTime(2025, 6, 1);
        var fechaFin = new DateTime(2025, 6, 30);

        calificacionRepoMock
            .Setup(r => r.ObtenerPorEncargadoYRangoFechasAsync(1, fechaInicio, fechaFin))
            .ReturnsAsync(new List<Calificacion>());

        var service = new CalificacionService(calificacionRepoMock.Object, encargadoRepoMock.Object);

        // Act
        var result = await service.ObtenerPorEncargadoYRangoAsync(1, fechaInicio, fechaFin);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }

    #endregion

    #region Exception Handling Tests

    [Fact]
    public async Task ServicioExcepcion_RepositorioLanzaExcepcion_PropagaSinCrash()
    {
        // Arrange
        var encargadoRepoMock = new Mock<IEncargadoRepository>();
        var qrServiceMock = new Mock<IQRService>();
        var configMock = new Mock<IConfiguration>();

        configMock.Setup(c => c["AppSettings:FrontendUrl"]).Returns("http://localhost:5173");
        encargadoRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Encargado>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        var service = new EncargadoService(encargadoRepoMock.Object, qrServiceMock.Object, configMock.Object);
        var dto = new CrearEncargadoDto { Nombre = "Test", Apellido = "Test" };

        // Act & Assert - Verify the exception propagates (would be caught by middleware/UI)
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CrearEncargadoAsync(dto));
    }

    #endregion
}
