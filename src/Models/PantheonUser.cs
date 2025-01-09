using System;
using System.Collections.Generic;

namespace Athena.SDK.Models
{
    public class PantheonUser
    {
        private Dictionary<string, object> _customProperties = new Dictionary<string, object>();
        public string Id { get; set; } = string.Empty;
        public string? DeviceId { get; set; }
        public byte[]? PasswordHash { get; set; }

        public string[] Roles { get; set; } = Array.Empty<string>();
        public string? Username { get; set; }

        public IReadOnlyDictionary<string, object> CustomProperties
        {
            get => _customProperties;
            set
            {
                if (_customProperties != null)
                    throw new InvalidOperationException("Custom properties cannot be changed after created.");
                _customProperties = new Dictionary<string, object>(value);
            }
        }

        public void SetCustomProperty(string key, object value)
        {
            if (!_customProperties.TryAdd(key, value)) _customProperties[key] = value;
        }

        public bool GetCustomProperty(string key, out object? value)
        {
            value = null;
            return _customProperties.TryGetValue(key, out value);
        }
    }
}