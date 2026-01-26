using System.Security.Cryptography;
using System.Text;

public class PinService
{
    public string HashPin(string pin)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(pin));
        return Convert.ToBase64String(bytes);
    }
}
