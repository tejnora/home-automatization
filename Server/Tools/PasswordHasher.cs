using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Server.Tools
{
    public static class PasswordHasher
    {
        const int KeySize = 64;
        const int Iterations = 350000;
        static HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        public static string HashPassword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(KeySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                Iterations,
                hashAlgorithm,
                KeySize);
            return Convert.ToHexString(hash);
        }

        public static bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, hashAlgorithm, KeySize);
            return hashToCompare.SequenceEqual(Convert.FromHexString(hash));
        }

        public static string GeneratePermanentSessionId()
        {
            var id = RandomNumberGenerator.GetBytes(KeySize);
            return Convert.ToHexString(id);
        }
    }
}
