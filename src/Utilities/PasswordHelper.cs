using System.Security.Cryptography;
using System.Text;



namespace BrainThrust.src.Utilities
{
    public class PasswordHelper
    {
        // Hash the password with a generated salt
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(); // Generates a strong cryptographic key
            passwordSalt = hmac.Key; // Store this salt for future verification
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        // Verify password against stored hash and salt
        public static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt); // Use stored salt
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash); // Compare hashes
        }
    }

}