using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Provides cryptographic services for secure PIN code management.
/// Uses SHA-256 hashing to ensure that raw PIN values are never stored directly.
/// </summary>
public class PinService
{
    /// <summary>
    /// Converts a plaintext PIN into a secure, non-reversible hash using SHA-256.
    /// This hash can be safely stored and compared for verification without exposing the original PIN.
    /// </summary>
    /// <param name="pin">The plaintext PIN to hash.</param>
    /// <returns>A Base64-encoded hash string.</returns>
    public string HashPin(string pin)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(pin));
        return Convert.ToBase64String(bytes);
    }
}
