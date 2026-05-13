using System.Security.Cryptography;
using Backend.Backend.Interface.ServiceInterface;
using QRCoder;

namespace Backend.Backend.Service
{
    /// <summary>
    /// Handles QR token generation
    /// </summary>
    public class QRService : IQrService
    {
        /// <summary>
        /// Generates permanent QR token
        /// </summary>
        public string GenerateToken() // method
        {
            var bytes = new byte[16]; // 128-bit token

            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(bytes);

            return Convert.ToHexString(bytes);
        }

        /// <summary>
        /// Generates QR code image as byte array
        /// </summary>
        /// <param name="content">
        /// Text/content inside QR code
        /// </param>
        public byte[] GenerateQr(string content) // method
        {
            // creates QR generator
            using var qrGenerator = new QRCodeGenerator();

            // creates QR data
            var qrData = qrGenerator.CreateQrCode(
                content,
                QRCodeGenerator.ECCLevel.Q
            );

            // creates PNG QR
            var qrCode = new PngByteQRCode(qrData);

            // returns PNG bytes
            return qrCode.GetGraphic(20);
        }

    }
}
