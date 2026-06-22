namespace Pruebas;

using Capa_Servicios;
using Xunit;

public class QRServiceTests
{
    private readonly QRServiceImpl _service;

    public QRServiceTests()
    {
        _service = new QRServiceImpl();
    }

    [Fact]
    public void GenerarQRBase64_RetornaBase64ValidoQueDecodificaAPNG()
    {
        // Arrange
        var contenido = "http://localhost:5173/encuesta/abc123";

        // Act
        var base64 = _service.GenerarQRBase64(contenido);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(base64));
        var bytes = Convert.FromBase64String(base64);
        Assert.True(bytes.Length > 4);
        // PNG signature: 0x89, 0x50, 0x4E, 0x47
        Assert.Equal(0x89, bytes[0]);
        Assert.Equal(0x50, bytes[1]);
        Assert.Equal(0x4E, bytes[2]);
        Assert.Equal(0x47, bytes[3]);
    }

    [Fact]
    public void GenerarQRBase64_ImagenTieneAlMenos300x300Pixeles()
    {
        // Arrange
        var contenido = "http://localhost:5173/encuesta/test123";

        // Act
        var base64 = _service.GenerarQRBase64(contenido);
        var bytes = Convert.FromBase64String(base64);

        // PNG format: after 8-byte signature, IHDR chunk starts at byte 8
        // IHDR chunk: 4 bytes length + 4 bytes "IHDR" + 4 bytes width + 4 bytes height
        // Width is at bytes 16-19, height at bytes 20-23 (big-endian)
        Assert.True(bytes.Length > 24);

        int width = (bytes[16] << 24) | (bytes[17] << 16) | (bytes[18] << 8) | bytes[19];
        int height = (bytes[20] << 24) | (bytes[21] << 16) | (bytes[22] << 8) | bytes[23];

        Assert.True(width >= 300, $"Width was {width}, expected at least 300");
        Assert.True(height >= 300, $"Height was {height}, expected at least 300");
    }
}
