using System;

namespace Athena.SDK.Models
{
    public class PantheonUser
    {
        public string Id { get; set; } = string.Empty;
        public string? DeviceId { get; set; }
        public byte[]? PasswordHash { get; set; }

        public string[] Claims { get; set; } = Array.Empty<string>();
        public string? Username { get; set; }
    }
}