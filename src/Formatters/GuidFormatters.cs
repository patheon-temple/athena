using System;

namespace Athena.SDK.Formatters
{
    public static class GuidFormatters
    {
        public static string Stringyfi(Guid guid) => guid.ToString("N");
    }
}