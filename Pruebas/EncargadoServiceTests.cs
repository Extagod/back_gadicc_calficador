namespace Pruebas;

using Capa_Abstracciones.Common;
using Capa_Abstracciones.DTOs;
using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Interfaces;
using Capa_Servicios;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

public class EncargadoServiceTests
{
    private readonly Mock<IEncargadoRepository> _encargadoRepoMock;
    private readonly Mock<IQRService> _qrServiceMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly EncargadoService _service;

    public EncargadoServiceTests()
    {
        _encargadoRepoMock = new Mock<IEncargadoRepository>();
        _qrServiceMock = new Mock<IQRService>();
        _configMock = new Mock<IConfiguration>();

        _configMock.Setup(c => c["AppSettings:FrontendUrl"]).Returns("http://localhost:5173");

        _qrServiceMock
            .Setup(q => q.GenerarQRBase64(It.IsAny<string>()))
            .Returns("fakeBase64QRCode");

        _encargadoRepoMock
            .Setup(r => r.AgregarAsync(It.IsAny<Empleado>()))
            .ReturnsAsync((Empleado e) => e);

        _encargadoRepoMock
            .Setup(r => r.GuardarQRAsync(It.IsAny<EmpleadoQR>()))
            .Returns(Task.CompletedTask);

        _service = new EncargadoService(
            _encargadoRepoMock.Object,
            _qrServiceMock.Object,
            _configMock.Object);
    }

    [Fact]
    public async Task CrearEncargado_RetornaSuccessConQRGenerado()
    {
        // Arrange
        var dto = new CrearEncargadoDto
        {
            CedulaRucPersona = "0102030405",
            Nombre = "Juan",
            Apellido = "Pérez"
        };

        // Act
        var result = await _service.CrearEncargadoAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("0102030405", result.Value!.CedulaRucPersona);
        // QR was saved
        _encargadoRepoMock.Verify(r => r.GuardarQRAsync(It.Is<EmpleadoQR>(
            qr => qr.TokenQR.Length == 32 && qr.CedulaRucPersona == "0102030405")), Times.Once);
    }

    [Fact]
    public async Task CrearEncargado_DosCreacionesConsecutivas_ProducenTokenQRDistintos()
    {
        // Arrange
        var dto1 = new CrearEncargadoDto { CedulaRucPersona = "0102030405", Nombre = "Juan", Apellido = "Pérez" };
        var dto2 = new CrearEncargadoDto { CedulaRucPersona = "0605040302", Nombre = "María", Apellido = "López" };

        var savedQRs = new List<EmpleadoQR>();
        _encargadoRepoMock
            .Setup(r => r.GuardarQRAsync(It.IsAny<EmpleadoQR>()))
            .Callback<EmpleadoQR>(qr => savedQRs.Add(qr))
            .Returns(Task.CompletedTask);

        // Act
        await _service.CrearEncargadoAsync(dto1);
        await _service.CrearEncargadoAsync(dto2);

        // Assert
        Assert.Equal(2, savedQRs.Count);
        Assert.NotEqual(savedQRs[0].TokenQR, savedQRs[1].TokenQR);
    }

    [Fact]
    public async Task ObtenerPorTokenQR_TokenInexistente_RetornaNotFound()
    {
        // Arrange
        _encargadoRepoMock
            .Setup(r => r.ObtenerPorTokenQRAsync("tokeninexistente"))
            .ReturnsAsync((Empleado?)null);

        // Act
        var result = await _service.ObtenerPorTokenQRAsync("tokeninexistente");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }
}
