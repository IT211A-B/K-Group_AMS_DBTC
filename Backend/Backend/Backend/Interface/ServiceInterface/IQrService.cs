using System.Security.Cryptography;

namespace Backend.Backend.Interface.ServiceInterface
{
    /// <summary>
    /// Handles QR token generation
    /// </summary>
    public interface IQrService
    {
        /// <summary>
        /// Generates permanent QR token
        /// </summary>
        string GenerateToken();
        byte[] GenerateQr(string content);
    }
}
