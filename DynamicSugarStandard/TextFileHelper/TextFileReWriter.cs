using DynamicSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DynamicSugar.TextFileHelper
{
    public static class TextFileReWriter
    {
        const string LOG_TO_MARK = "#logto:";

        public static string ProcessLogToMacro(string text, string splitString, Func<string, string> fn)
        {
            var lines = text.Split(DS.List(splitString).ToArray(), StringSplitOptions.None).ToList();
            var newLines = new List<string>();

            foreach (var line1 in lines)
            {
                var line = line1;
                if (line.Contains(LOG_TO_MARK))
                {
                    if(fn != null)
                        line = fn(line); // preprocess the line

                    var logToFileName = line.Substring(line.IndexOf(LOG_TO_MARK) + LOG_TO_MARK.Length).Trim();
                    var logToText = line.Substring(0, line.IndexOf(LOG_TO_MARK) - 1);

                    File.AppendAllText(logToFileName, logToText + Environment.NewLine);
                }
                newLines.Add(line);
            }
            return text;
        }

        public static void Rewrite(string fileName, string splitString, Func<string, List<string>> fn)
        {
            var lines = ReadLines(fileName, splitString);
            var newLines = new List<string>();

            lines.ForEach((line) => newLines.AddRange(fn(line)));

            WriteFile(fileName, splitString, newLines);
        }

        public static void Insert(string fileName, string splitString, string line, int lineIndex)
        {
            var lines = ReadLines(fileName, splitString);

            lines.Insert(lineIndex, line);

            WriteFile(fileName, splitString, lines);
        }

        public static void Add(string fileName, string splitString, string newLine)
        {
            var lines = ReadLines(fileName, splitString);

            lines.Add(newLine);

            WriteFile(fileName, splitString, lines);
        }

        private static List<string> ReadLines(string fileName, string splitString)
        {
            var text = File.ReadAllText(fileName);
            var lines = text.Split(DS.List(splitString).ToArray(), StringSplitOptions.None).ToList();
            return lines;
        }

        public static void WriteFile(string fileName, string splitString, List<string> newLines)
        {
            if(File.Exists(fileName))
                File.Delete(fileName);
            File.WriteAllText(fileName, string.Join(splitString, newLines));
        }

        public static void WriteFile(string fileName, string text)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
            File.WriteAllText(fileName, text);
        }
    }
}
