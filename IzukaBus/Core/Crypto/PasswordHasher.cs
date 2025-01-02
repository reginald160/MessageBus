using System.Text;
using System;
using System.Security.Cryptography;

namespace IzukaBus.Core.Crypto
{

      public   class PasswordHasher
        {
            // Hash a password using SHA-256
            public static string HashPassword(string password)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    // Convert the password string to bytes
                    byte[] bytes = Encoding.UTF8.GetBytes(password);

                    // Compute the hash of the password bytes
                    byte[] hashBytes = sha256.ComputeHash(bytes);

                    // Convert the byte array back to a string
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }
            }

            // Verify a password against the stored hash
            public static bool VerifyPassword(string enteredPassword, string storedHash)
            {
                // Hash the entered password
                string enteredHash = HashPassword(enteredPassword);

                // Compare the entered hash with the stored hash
                return enteredHash == storedHash;
            }
        }

    
}
