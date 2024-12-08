using System.Security.Cryptography;
using System.Text;

namespace SignalR.Helpers
{
    public static class PasswordHelper
    {
        public static string Hash(string input)
        {
            // Convert the input string to a byte array
            byte[] bytes = Encoding.UTF8.GetBytes(input);

            // Compute the hash using SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(bytes);

                // Convert the hash byte array to a string
                StringBuilder result = new StringBuilder();
                foreach (byte b in hash)
                {
                    result.Append(b.ToString("x2")); // "x2" formats the byte as a hexadecimal string
                }
                return result.ToString();
            }
        }

        public static bool VerifyPasswords(string password, string hashedPassword)
        { 
            var newHashedPassword = Hash(password);
            return newHashedPassword == hashedPassword;
        }
    }
}
