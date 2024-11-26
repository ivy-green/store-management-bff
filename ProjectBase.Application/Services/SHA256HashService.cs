using ProjectBase.Domain.Interfaces.IServices;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace ProjectBase.Application.Services
{
    [ExcludeFromCodeCoverage]
    public class SHA256HashService : IHashService
    {
        public (string, string) CreatePasswordHashAndSalt(string password)
        {
            SHA256 sha256haser = SHA256.Create();

            byte[] passwordSaltBytes = RandomNumberGenerator.GetBytes(SHA256.HashSizeInBytes);
            byte[] hashedSalt = sha256haser.ComputeHash(passwordSaltBytes);

            IEnumerable<string> hexStringArrayFromHashedSalt = hashedSalt.Select(h => h.ToString("x2"));

            string hexStringFromHashedSalt = string.Join("", hexStringArrayFromHashedSalt);
            string saltedPassword = hexStringFromHashedSalt + password;

            byte[] saltedPasswordBytes = Encoding.UTF8.GetBytes(saltedPassword);
            byte[] hashedSaltedPassword = sha256haser.ComputeHash(saltedPasswordBytes);

            IEnumerable<string> hexStringArrayFromHashedSaltedPassword = hashedSaltedPassword.Select(h => h.ToString("x2"));

            string hexStringFromHashedSaltedPassword = string.Join("", hexStringArrayFromHashedSaltedPassword);

            return (hexStringFromHashedSaltedPassword, hexStringFromHashedSalt);
        }

        public bool VerifyPasswordHash(string salt, string passwordFromDb, string inputPassword)
        {
            SHA256 sha256haser = SHA256.Create();

            string saltedPassword = salt + inputPassword;

            byte[] saltedPasswordBytes = Encoding.UTF8.GetBytes(saltedPassword);
            byte[] hashedSaltedPassword = sha256haser.ComputeHash(saltedPasswordBytes);

            IEnumerable<string> hexStringArrayFromHashedSaltedPassword = hashedSaltedPassword.Select(h => h.ToString("x2"));

            string hexStringFromHashedSaltedPassword = string.Join("", hexStringArrayFromHashedSaltedPassword);

            return passwordFromDb.Equals(hexStringFromHashedSaltedPassword);
        }

        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
