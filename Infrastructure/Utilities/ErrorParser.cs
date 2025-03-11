using System.Text.RegularExpressions;

namespace Infrastructure.Utilities
{
    public static partial class ErrorParser
    {
        public static string ExtractObjectName(string message)
        {
            Match match = ObjectNameRegex().Match(message);
            return match.Success ? match.Groups[1].Value : "Desconocido";
        }

        public static string ExtractDuplicateValue(string message)
        {
            Match match = DuplicateValueRegex().Match(message);
            return match.Success ? match.Groups[1].Value : "Desconocido";
        }

        [GeneratedRegex(@"object '(.+?)'")]
        private static partial Regex ObjectNameRegex();

        [GeneratedRegex(@"The duplicate key value is \((.+?)\)")]
        private static partial Regex DuplicateValueRegex();
    }
}
