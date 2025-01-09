using System;

namespace Athena.SDK.Models
{
    public class PantheonService
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public byte[] AuthorizationCode { get; set; } = Array.Empty<byte>();
        public string[] Claims { get; set; } = Array.Empty<string>();
    }
}