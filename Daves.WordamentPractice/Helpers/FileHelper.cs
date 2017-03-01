using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Daves.WordamentPractice.Helpers
{
    // Not sure where to do file access, but keeping it here seems better than doing it all throughout the app.
    public static class FileHelper
    {
        public static IEnumerable<string> ReadDictionaryFile()
            => ReadLines(ConfigurationManager.AppSettings["DictionaryFilePath"]);

        public static IEnumerable<string> ReadLines(string filePath)
            => File.ReadLines(filePath);

        public static string[] ReadAllLines(string filePath)
            => File.ReadAllLines(filePath);

        public static void WriteAllLines(string filePath, IEnumerable<string> lines)
            => File.WriteAllLines(filePath, lines);
    }
}
