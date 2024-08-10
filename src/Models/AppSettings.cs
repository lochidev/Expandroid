namespace Expandroid.Models
{
    internal static class AppSettings
    {
        public static List<string> SupportedList = new() { "echo", "date", "clipboard", "random" };
        public static string OldDictPath = Path.Combine(FileSystem.Current.CacheDirectory, "keywords.json");
        public static string DictPath = Path.Combine(FileSystem.Current.AppDataDirectory, "keywords.json");
        public static string GlobalVarsPath = Path.Combine(FileSystem.Current.AppDataDirectory, "global.json");
    }
}
