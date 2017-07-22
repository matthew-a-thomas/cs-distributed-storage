namespace DistributedStorage.Storage.FileSystem
{
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    public sealed class FileSystemSafeStringAdapter
    {
        public const char EscapeChar = 'Z';
        private static readonly Regex EscapeRegex = new Regex($"{EscapeChar}([0-9]+){EscapeChar}");
        private static readonly char[] UnsafeCharacters = Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars()).Distinct().ToArray();
        
        public static string MakeRaw(string safe)
        {
            var raw = EscapeRegex.Replace(safe, match =>
            {
                var integer = int.Parse(match.Groups[1].Value);
                return ((char)integer).ToString();
            });
            return raw;
        }

        public static string MakeSafe(string raw)
        {
            var safe = raw.Replace(EscapeChar.ToString(), $"{EscapeChar}{(int) EscapeChar}{EscapeChar}");
            return UnsafeCharacters.Aggregate(safe, (current, unsafeCharacter) => current.Replace(unsafeCharacter.ToString(), $"{EscapeChar}{(int) unsafeCharacter}{EscapeChar}"));
        }
    }
}
