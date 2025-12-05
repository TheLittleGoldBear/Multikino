using System.Security.Cryptography;
using System.Text;

namespace Multikino.Services
{
    public static class PasswordHasher
    {
        public static void CreatePasswordHash(string password, out string hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = hmac.ComputeHash(passwordBytes);
            hash = Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var computedHash = hmac.ComputeHash(passwordBytes);
            var computedHashBase64 = Convert.ToBase64String(computedHash);
            return computedHashBase64 == storedHash;
        }
    }
}
