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
            .Setup(r => r.AgregarAsync(It.IsAny<Encargado>()))
            .ReturnsAsync((Encargado e) => { e.IdEncargado = 1; return e; });

        _service = new EncargadoService(
            _encargadoRepoMock.Object,
            _qrServiceMock.Object,
            _configMock.Object);
    }

    [Fact]
    public async Task CrearEncargado_RetornaSuccessConTokenQR32HexChars()
    {
        // Arrange
        var dto = new CrearEncargadoDto { Nombre = "Juan", Apellido = "Pérez" };

        // Act
        var result = await _service.CrearEncargadoAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotNull(result.Value!.TokenQR);
        Assert.Equal(32, result.Value.TokenQR!.Length);
        Assert.True(result.Value.TokenQR.All(c => "0123456789abcdef".Contains(c)));
    }

    [Fact]
    public async Task CrearEncargado_DosCreacionesConsecutivas_ProducenTokenQRDistintos()
    {
        // Arrange
        var dto1 = new CrearEncargadoDto { Nombre = "Juan", Apellido = "Pérez" };
        var dto2 = new CrearEncargadoDto { Nombre = "María", Apellido = "López" };

        // Act
        var result1 = await _service.CrearEncargadoAsync(dto1);
        var result2 = await _service.CrearEncargadoAsync(dto2);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.NotEqual(result1.Value!.TokenQR, result2.Value!.TokenQR);
    }

    [Fact]
    public async Task ObtenerPorTokenQR_TokenInexistente_RetornaNotFound()
    {
        // Arrange
        _encargadoRepoMock
            .Setup(r => r.ObtenerPorTokenQRAsync("tokeninexistente"))
            .ReturnsAsync((Encargado?)null);

        // Act
        var result = await _service.ObtenerPorTokenQRAsync("tokeninexistente");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }
}
