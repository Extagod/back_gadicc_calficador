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
        encargadoRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Empleado>()))
            .ReturnsAsync((Empleado e) => e);
        encargadoRepoMock.Setup(r => r.GuardarQRAsync(It.IsAny<EmpleadoQR>()))
            .Returns(Task.CompletedTask);

        var service = new EncargadoService(encargadoRepoMock.Object, qrServiceMock.Object, configMock.Object);
        var dto = new CrearEncargadoDto
        {
            CedulaRucPersona = "0102030405",
            Nombre = "Carlos",
            Apellido = "Gómez",
            Cargo = "Atención"
        };

        // Act
        var result = await service.CrearEncargadoAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("0102030405", result.Value!.CedulaRucPersona);
        Assert.Equal("Carlos", result.Value.Persona?.PrimerNombre);
        Assert.Equal("Gómez", result.Value.Persona?.PrimerApellido);
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
        encargadoRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Empleado>()))
            .ReturnsAsync((Empleado e) => e);
        encargadoRepoMock.Setup(r => r.GuardarQRAsync(It.IsAny<EmpleadoQR>()))
            .Returns(Task.CompletedTask);

        var service = new EncargadoService(encargadoRepoMock.Object, qrServiceMock.Object, configMock.Object);
        var dto = new CrearEncargadoDto { CedulaRucPersona = "0102030405", Nombre = "  ", Apellido = "López" };

        // Act - The service trims the value. In the full stack, validation would
        // happen at the UI layer (Panel_Admin) before reaching the service.
        var result = await service.CrearEncargadoAsync(dto);

        // Assert - Service processes the request; Panel_Admin handles empty validation
        Assert.True(result.IsSuccess);
        Assert.Equal("", result.Value!.Persona?.PrimerNombre); // Trimmed whitespace becomes empty
    }

    #endregion

    #region QR Generation Tests

    [Fact]
    public async Task GeneracionQR_ProduceTokenQR32HexCharsYCodigoQRNoVacio()
    {
        // Arrange
        var encargadoRepoMock = new Mock<IEncargadoRepository>();
        var qrServiceMock = new Mock<IQRService>();
        var configMock = new Mock<IConfiguration>();

        configMock.Setup(c => c["AppSettings:FrontendUrl"]).Returns("http://localhost:5173");
        qrServiceMock.Setup(q => q.GenerarQRBase64(It.IsAny<string>())).Returns("someBase64Content");
        encargadoRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Empleado>()))
            .ReturnsAsync((Empleado e) => e);

        EmpleadoQR? savedQR = null;
        encargadoRepoMock.Setup(r => r.GuardarQRAsync(It.IsAny<EmpleadoQR>()))
            .Callback<EmpleadoQR>(qr => savedQR = qr)
            .Returns(Task.CompletedTask);

        var service = new EncargadoService(encargadoRepoMock.Object, qrServiceMock.Object, configMock.Object);
        var dto = new CrearEncargadoDto { CedulaRucPersona = "0102030405", Nombre = "Ana", Apellido = "Martínez" };

        // Act
        var result = await service.CrearEncargadoAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(savedQR);
        Assert.Equal(32, savedQR!.TokenQR.Length);
        Assert.True(savedQR.TokenQR.All(c => "0123456789abcdef".Contains(c)));
        Assert.False(string.IsNullOrEmpty(savedQR.CodigoQR));
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
        encargadoRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Empleado>()))
            .ReturnsAsync((Empleado e) => e);

        var savedQRs = new List<EmpleadoQR>();
        encargadoRepoMock.Setup(r => r.GuardarQRAsync(It.IsAny<EmpleadoQR>()))
            .Callback<EmpleadoQR>(qr => savedQRs.Add(qr))
            .Returns(Task.CompletedTask);

        var service = new EncargadoService(encargadoRepoMock.Object, qrServiceMock.Object, configMock.Object);

        // Act
        await service.CrearEncargadoAsync(new CrearEncargadoDto { CedulaRucPersona = "0102030405", Nombre = "A", Apellido = "B" });
        await service.CrearEncargadoAsync(new CrearEncargadoDto { CedulaRucPersona = "0605040302", Nombre = "C", Apellido = "D" });

        // Assert
        Assert.Equal(2, savedQRs.Count);
        Assert.NotEqual(savedQRs[0].TokenQR, savedQRs[1].TokenQR);
    }

    #endregion

    #region Calificaciones Filter Tests

    [Fact]
    public async Task CalificacionesFiltro_PorRangoFechas_RetornaSoloCoincidentes()
    {
        // Arrange
        var calificacionRepoMock = new Mock<ICalificacionRepository>();
        var encargadoRepoMock = new Mock<IEncargadoRepository>();

        var cedula = "0102030405";
        var fechaInicio = new DateTime(2024, 1, 1);
        var fechaFin = new DateTime(2024, 1, 31);

        var calificacionesFiltradas = new List<Calificacion>
        {
            new() { IdCalificacion = 1, CedulaRucPersona = cedula, Valor = ValorCalificacion.Excelente, FechaHora = new DateTime(2024, 1, 15) },
            new() { IdCalificacion = 2, CedulaRucPersona = cedula, Valor = ValorCalificacion.Buena, FechaHora = new DateTime(2024, 1, 20) }
        };

        calificacionRepoMock
            .Setup(r => r.ObtenerPorEmpleadoYRangoFechasAsync(cedula, fechaInicio, fechaFin))
            .ReturnsAsync(calificacionesFiltradas);

        var service = new CalificacionService(calificacionRepoMock.Object, encargadoRepoMock.Object);

        // Act
        var result = await service.ObtenerPorEmpleadoYRangoAsync(cedula, fechaInicio, fechaFin);

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

        var cedula = "0102030405";
        var fechaInicio = new DateTime(2025, 6, 1);
        var fechaFin = new DateTime(2025, 6, 30);

        calificacionRepoMock
            .Setup(r => r.ObtenerPorEmpleadoYRangoFechasAsync(cedula, fechaInicio, fechaFin))
            .ReturnsAsync(new List<Calificacion>());

        var service = new CalificacionService(calificacionRepoMock.Object, encargadoRepoMock.Object);

        // Act
        var result = await service.ObtenerPorEmpleadoYRangoAsync(cedula, fechaInicio, fechaFin);

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
        encargadoRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Empleado>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        var service = new EncargadoService(encargadoRepoMock.Object, qrServiceMock.Object, configMock.Object);
        var dto = new CrearEncargadoDto { CedulaRucPersona = "0102030405", Nombre = "Test", Apellido = "Test" };

        // Act & Assert - Verify the exception propagates (would be caught by middleware/UI)
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CrearEncargadoAsync(dto));
    }

    #endregion
}
