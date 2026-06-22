namespace Capa_Servicios;

using Capa_Abstracciones.Interfaces;
using QRCoder;

public class QRServiceImpl : IQRService
{
    /// <summary>
    /// Genera un QR en Base64 PNG a partir de una URL/contenido.
    /// La imagen generada tiene al menos 300x300 píxeles (pixelsPerModule=20).
    /// </summary>
    public string GenerarQRBase64(string contenido)
    {
        using var qrGenerator = new QRCodeGenerator();
        QRCodeData qrData = qrGenerator.CreateQrCode(contenido, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrData);
        byte[] qrBytes = qrCode.GetGraphic(20);
        return Convert.ToBase64String(qrBytes);
    }
}
