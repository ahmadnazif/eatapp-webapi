using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EatAppApi.Helpers
{
    public class PasswordHasher
    {
        public static string GenerateHash(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            using (var sha = new SHA256Managed())
            {
                var textData = Encoding.UTF8.GetBytes(text);
                var hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }

        public static string GeneratePasswordHash(string password, string passwordSalt)
        {
            var plain = $"{password}{passwordSalt}"; // salt at end of password
            return GenerateHash(plain);
        }

        public static bool IsEqual(string passwordHash, string passwordSalt, string password)
        {
            var saltedFront = GenerateHash($"{passwordSalt}{password}");
            var saltedBack = GenerateHash($"{password}{passwordSalt}");

            return (passwordHash == saltedFront || passwordHash == saltedBack) ? true : false;
        }
    }
}
