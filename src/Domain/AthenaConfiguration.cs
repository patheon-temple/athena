using System;
using Athena.SDK.Crypto;

namespace Athena.SDK.Domain
{
    public sealed class AthenaConfiguration
    {
        public string SuperuserUsername { get; set; } = null!;
        public string SuperuserPassword { get; set; } = null!;
        public Guid SuperuserId { get; set; }
        public byte[] SuperuserPasswordEncoded => Passwords.EncodePassword(SuperuserPassword);

        public bool IsSuperUser(Guid id) => id.Equals(SuperuserId);

        public bool IsSuperUser(string username) => SuperuserUsername.Equals(username);
    }
}