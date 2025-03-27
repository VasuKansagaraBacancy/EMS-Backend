using System;
using System.Security.Cryptography;
using System.Text;

namespace EMS.EMS.Shared.Helpers
{
    public static class PasswordHelper
    {
        public static string GeneratePassword(int length = 6)
        {
            try
            {
                const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                StringBuilder res = new StringBuilder();
                using (var rng = RandomNumberGenerator.Create())
                {
                    byte[] uintBuffer = new byte[sizeof(uint)];
                    while (length-- > 0)
                    {
                        rng.GetBytes(uintBuffer);
                        uint num = BitConverter.ToUInt32(uintBuffer, 0);
                        res.Append(valid[(int)(num % (uint)valid.Length)]);
                    }
                }
                return res.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating password: {ex.Message}");
                return string.Empty; 
            }
        }
        public static string HashPassword(string password)
        {
            try
            {
                using var sha256 = SHA256.Create();
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error hashing password: {ex.Message}");
                return string.Empty; 
            }
        }
        public static bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            try
            {
                var hashedInput = HashPassword(inputPassword);
                return hashedInput == storedHashedPassword;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying password: {ex.Message}");
                return false; 
            }
        }
    }
}