using System;
using System.Text.RegularExpressions;

namespace Ronin.Helpers
{
    public static class VersionParser
    {
        private static readonly Regex VersionRegex = new(@"([0-9]+\.){2}[0-9]+");

        public static Version? TryParse(string version)
        {
            if (string.IsNullOrEmpty(version))
                throw new ArgumentNullException(nameof(version));

            var match = VersionRegex.Match(version);
            if (!match.Success)
                return null;

            return Version.Parse(match.Value);
        }
    }
}
