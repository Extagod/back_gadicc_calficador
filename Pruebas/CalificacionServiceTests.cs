namespace Pruebas;

using Capa_Abstracciones.Common;
using Capa_Abstracciones.DTOs;
using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Enums;
using Capa_Abstracciones.Interfaces;
using Capa_Servicios;
using Moq;
using Xunit;

public class CalificacionServiceTests
{
    private readonly Mock<ICalificacionRepository> _calificacionRepoMock;
    private readonly Mock<IEncargadoRepository> _encargadoRepoMock;
    private readonly CalificacionService _service;

    public CalificacionServiceTests()
    {
        _calificacionRepoMock = new Mock<ICalificacionRepository>();
        _encargadoRepoMock = new Mock<IEncargadoRepository>();
        _service = new CalificacionService(_calificacionRepoMock.Object, _encargadoRepoMock.Object);
    }

    [Theory]
    [InlineData("Excelente")]
    [InlineData("Buena")]
    [InlineData("Regular")]
    [InlineData("Mala")]
    public async Task CrearCalificacion_ConValorValido_RetornaSuccess(string valor)
    {
        // Arrange
        var cedula = "0102030405";
        var dto = new CrearCalificacionDto { CedulaEmpleado = cedula, Calificacion = valor };
        var empleado = new Empleado
        {
            CedulaRucPersona = cedula,
            Persona = new Persona { CedulaRucPersona = cedula, PrimerNombre = "Juan", PrimerApellido = "Pérez" }
        };

        _encargadoRepoMock
            .Setup(r => r.ObtenerPorCedulaAsync(cedula))
            .ReturnsAsync(empleado);

        _calificacionRepoMock
            .Setup(r => r.AgregarAsync(It.IsAny<Calificacion>()))
            .ReturnsAsync((Calificacion c) => { c.IdCalificacion = 1; return c; });

        // Act
        var result = await _service.CrearCalificacionAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(Enum.Parse<ValorCalificacion>(valor, true), result.Value!.Valor);
    }

    [Fact]
    public async Task CrearCalificacion_ConValorInvalido_RetornaValidationError()
    {
        // Arrange
        var dto = new CrearCalificacionDto { CedulaEmpleado = "0102030405", Calificacion = "Invalido" };

        // Act
        var result = await _service.CrearCalificacionAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceErrorType.Validation, result.ErrorType);
        Assert.Contains("Invalido", result.ErrorMessage!);
        Assert.Contains("Excelente", result.ErrorMessage!);
        Assert.Contains("Buena", result.ErrorMessage!);
        Assert.Contains("Regular", result.ErrorMessage!);
        Assert.Contains("Mala", result.ErrorMessage!);
    }

    [Fact]
    public async Task CrearCalificacion_ConValorVacio_RetornaValidationErrorObligatorio()
    {
        // Arrange
        var dto = new CrearCalificacionDto { CedulaEmpleado = "0102030405", Calificacion = "" };

        // Act
        var result = await _service.CrearCalificacionAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceErrorType.Validation, result.ErrorType);
        Assert.Contains("obligatorio", result.ErrorMessage!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CrearCalificacion_ConEmpleadoInexistente_RetornaNotFound()
    {
        // Arrange
        var cedula = "9999999999";
        var dto = new CrearCalificacionDto { CedulaEmpleado = cedula, Calificacion = "Buena" };

        _encargadoRepoMock
            .Setup(r => r.ObtenerPorCedulaAsync(cedula))
            .ReturnsAsync((Empleado?)null);

        // Act
        var result = await _service.CrearCalificacionAsync(dto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }
}
