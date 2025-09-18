using System.Security.Cryptography;
using System.Text;
using MySvelteApp.Server.Application.Common.Interfaces;

namespace MySvelteApp.Server.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    public (string Hash, string Salt) HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100_000,
            HashAlgorithmName.SHA512,
            64);
        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public bool VerifyPassword(string password, string hash, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var storedHash = Convert.FromBase64String(hash);
        var computed = Rfc2898DeriveBytes.Pbkdf2(
            password,
            saltBytes,
            100_000,
            HashAlgorithmName.SHA512,
            storedHash.Length);
        return CryptographicOperations.FixedTimeEquals(computed, storedHash);
    }
}
