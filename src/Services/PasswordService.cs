using System.Text;
using Athena.SDK.Crypto;
using Athena.SDK.Domain;
using Athena.SDK.Models;

namespace Athena.SDK.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly AthenaConfiguration _configuration;

        public PasswordService(AthenaConfiguration configuration)
        {
            _configuration = configuration;
        }

        public byte[] HashPassword(string password)
        {
            return Passwords.HashPassword(password);
        }

        public bool VerifyUserAccountPassword(PantheonUser pantheonUser, string password)
        {
            if (_configuration.IsSuperUserUsername(pantheonUser.Id)) return password.Equals(_configuration.SuperuserPassword);
            return VerifyUserAccountPassword(pantheonUser.Id, pantheonUser.PasswordHash!, password);
        }

        public bool VerifyUserAccountPassword(string accountId, byte[] hash, string password)
        {
            if (_configuration.IsSuperUserId(accountId)) return password.Equals(_configuration.SuperuserPassword);
            return Passwords.VerifyHashedPassword(hash, password);
        }

    
        public bool VerifyAuthorizationCode(byte[] data, string authorizationCode)
        {
            return Passwords.VerifyAuthorizationCode(data, Encoding.UTF8.GetBytes(authorizationCode));
        }

        public string DecodePassword(byte[] passwordHash)
        {
            return Passwords.DecodePassword(passwordHash);
        }
    }
}