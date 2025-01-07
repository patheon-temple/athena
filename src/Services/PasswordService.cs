using System;
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

        public bool VerifyUserAccountPassword(PantheonIdentity pantheonIdentity, string password)
        {
            if (_configuration.IsSuperUser(pantheonIdentity.Id)) return password.Equals(_configuration.SuperuserPassword);
            return VerifyUserAccountPassword(pantheonIdentity.Id, pantheonIdentity.PasswordHash!, password);
        }

        public bool VerifyUserAccountPassword(Guid accountId, byte[] hash, string password)
        {
            if (_configuration.IsSuperUser(accountId)) return password.Equals(_configuration.SuperuserPassword);
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