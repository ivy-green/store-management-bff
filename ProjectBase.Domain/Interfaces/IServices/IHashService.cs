namespace ProjectBase.Domain.Interfaces.IServices
{
    public interface IHashService
    {
        string Base64Decode(string base64EncodedData);
        string Base64Encode(string plainText);
        (string, string) CreatePasswordHashAndSalt(string password);
        bool VerifyPasswordHash(string salt, string passwordFromDb, string inputPassword);
    }
}