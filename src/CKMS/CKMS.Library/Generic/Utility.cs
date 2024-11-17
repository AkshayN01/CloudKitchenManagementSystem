using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CKMS.Library.Generic
{
    public static class Utility
    {
        public static string GenerateVerificationToken(int length = 32)
        {
            // Use RNGCryptoServiceProvider to generate secure random bytes
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes)
                    .Replace("+", "")  // Remove characters that may not be URL-safe
                    .Replace("/", "")
                    .Replace("=", "");
            }
        }
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false; // Null or empty email is not valid
            }

            try
            {
                // Regular expression for validating an email address
                string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);
            }
            catch (Exception)
            {
                return false; // Any exception means validation failed
            }
        }
        public static bool IsValueInEnum<TEnum>(int value) where TEnum : Enum
        {
            return Enum.IsDefined(typeof(TEnum), value);
        }
    }
}
