using Athena.SDK.Models;

namespace Athena.SDK.Services
{
    public interface IPasswordService
    {
        byte[] HashPassword(string password);
        bool VerifyUserAccountPassword(PantheonUser pantheonUser, string password);
        bool VerifyUserAccountPassword(string accountId, byte[] hash, string password);
        bool VerifyAuthorizationCode(byte[] data, string authorizationCode);
        string DecodePassword(byte[] passwordHash);
    }
}