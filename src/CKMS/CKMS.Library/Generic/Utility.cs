using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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
        public static async Task<String> SerialiseData<T>(T data)
        {
            String json = String.Empty;

            using (var stream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync<T>(stream, data);
                stream.Position = 0; // Reset the stream position to read the output

                using (var reader = new StreamReader(stream))
                {
                    json = await reader.ReadToEndAsync();
                    Console.WriteLine(json);
                }
            }

            return json;
        }
        public static async Task<T?> DeserialiseData<T>(String json)
        {
            T? data = default(T);

            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
            {
                data = await JsonSerializer.DeserializeAsync<T>(stream);
            }

            return data;
        }
        public static async Task<bool> WriteToFile<T>(String fileName, T data, String filePath = "")
        {
            bool success = false;

            try
            {
                if(filePath == "")
                    filePath = GetDefaultFolderPath(fileName);

                String content = await SerialiseData<T>(data);
                await File.WriteAllTextAsync(filePath, content);
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }

            return success;
        }
        public static async Task<T?> ReadFromFile<T>(String fileName, String filePath = "")
        {
            try
            {
                if (filePath == "")
                    filePath = GetDefaultFolderPath(fileName);

                String json = await File.ReadAllTextAsync(filePath);
                if (!String.IsNullOrEmpty(json))
                {
                    return await DeserialiseData<T>(json);
                }
            }
            catch (Exception ex)
            {
                return default(T);
            }

            return default(T);
        }
        private static String GetDefaultFolderPath(String fileName)
        {
            // Get the base directory of the application
            string baseDirectory = AppContext.BaseDirectory;

            String solutionDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../.."));

            // Specify the target folder relative to the base directory
            string folderName = "SeedData"; // Change to your desired folder name
            string targetFolderPath = Path.Combine(solutionDirectory, folderName);
            // Ensure the folder exists
            Directory.CreateDirectory(targetFolderPath);
            string filePath = Path.Combine(targetFolderPath, fileName);
            return filePath;
        }
    }
}
