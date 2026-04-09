using System;
using System.Security.Cryptography;
using System.Text;

namespace IntegraCamp.Services
{
    public static class SecurityService
    {
        public static string HashString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            using (var pbkdf2 = new Rfc2898DeriveBytes(input, 32, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                byte[] salt = pbkdf2.Salt;
                byte[] hashBytes = new byte[64];
                Array.Copy(salt, 0, hashBytes, 0, 32);
                Array.Copy(hash, 0, hashBytes, 32, 32);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool ValidateRoom(string room)
        {
            if (int.TryParse(room, out int roomNumber))
            {
                if (roomNumber >= 101 && roomNumber <= 108) return true;
                if (roomNumber >= 201 && roomNumber <= 212) return true;
                if (roomNumber >= 301 && roomNumber <= 312) return true;
            }
            return false;
        }
    }
}