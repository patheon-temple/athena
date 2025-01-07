using System;
using Athena.SDK.Models;

namespace Athena.SDK.Services
{
    public interface IPasswordService
    {
        byte[] HashPassword(string password);
        bool VerifyUserAccountPassword(PantheonIdentity pantheonIdentity, string password);
        bool VerifyUserAccountPassword(Guid accountId, byte[] hash, string password);
        bool VerifyAuthorizationCode(byte[] data, string authorizationCode);
        string DecodePassword(byte[] passwordHash);
    }
}