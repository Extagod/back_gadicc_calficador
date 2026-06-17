using QRCoder;

namespace ApiEncuestaPrototipe.Services
{
    public class QRService
    {
        /// <summary>
        /// Genera un QR en Base64 PNG a partir de una URL.
        /// La URL apuntará al formulario de encuesta del encargado.
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
}
