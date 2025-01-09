using System;
using Athena.SDK.Crypto;

namespace Athena.SDK.Domain
{
    public sealed class AthenaConfiguration
    {
        public string SuperuserUsername { get; set; } = null!;
        public string SuperuserPassword { get; set; } = null!;
        public string SuperuserId { get; set; } = null!;
        public byte[] SuperuserPasswordEncoded => Passwords.EncodePassword(SuperuserPassword);

        public bool IsSuperUserId(string id) => id.Equals(SuperuserId);

        public bool IsSuperUserUsername(string username) => SuperuserUsername.Equals(username);
    }
}