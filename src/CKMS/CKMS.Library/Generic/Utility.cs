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

        public static String? GetEnumStringValue<TEnum>(int value) where TEnum : Enum 
        {
            return Enum.GetName(typeof(TEnum), value);
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
        public static (string, string) GetAccountVerificationEmailContent(string verificationUrl, string recipientName)
        {
            string subject = "Verify your account";
            string body =  $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    line-height: 1.6;
                    margin: 0;
                    padding: 0;
                    background-color: #f8f9fa;
                    color: #333;
                }}
                .email-container {{
                    max-width: 600px;
                    margin: 20px auto;
                    padding: 20px;
                    background: #ffffff;
                    border: 1px solid #e0e0e0;
                    border-radius: 8px;
                    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                }}
                .email-header {{
                    text-align: center;
                    margin-bottom: 20px;
                }}
                .email-header h1 {{
                    font-size: 24px;
                    color: #007bff;
                }}
                .email-body {{
                    text-align: center;
                }}
                .email-body p {{
                    font-size: 16px;
                    margin: 15px 0;
                }}
                .verify-button {{
                    display: inline-block;
                    padding: 10px 20px;
                    font-size: 16px;
                    color: #ffffff;
                    background-color: #007bff;
                    text-decoration: none;
                    border-radius: 5px;
                    margin-top: 20px;
                }}
                .verify-button:hover {{
                    background-color: #0056b3;
                }}
                .email-footer {{
                    text-align: center;
                    margin-top: 20px;
                    font-size: 12px;
                    color: #777;
                }}
            </style>
        </head>
        <body>
            <div class='email-container'>
                <div class='email-header'>
                    <h1>Verify Your Account</h1>
                </div>
                <div class='email-body'>
                    <p>Hi {recipientName},</p>
                    <p>Thank you for registering with us! Please verify your email address by clicking the button below:</p>
                    <a href='{verificationUrl}' class='verify-button'>Verify Account</a>
                    <p>If you didn’t create this account, you can safely ignore this email.</p>
                </div>
                <div class='email-footer'>
                    <p>&copy; {DateTime.Now.Year} Cloud Kitchen. All Rights Reserved.</p>
                </div>
            </div>
        </body>
        </html>";

            return (subject, body);
        }
    }
}
