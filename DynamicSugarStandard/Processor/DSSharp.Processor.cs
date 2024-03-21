using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;
#if !MONOTOUCH
using System.Dynamic;
#endif

namespace DynamicSugar
{
    /// <summary>
    /// Dynamic Sharp Exception
    /// </summary>
    public class ProcessorFailedException : System.Exception {

        public ProcessorFailedException(string message) : base(message) { }
    }
    /// <summary>
    /// Dynamic Sharp Helper Class
    /// </summary>
    public static partial class DS
    {
        public class Processor
        {
            public string SourceText { get; }
            public string TextToBeProcessed { get; set; }

            public Dictionary<string, string> Macros = new Dictionary<string, string>();    

            public Processor(string text)
            {
                SourceText = text;
            }

            public Dictionary<string, string> ExtractMacros()
            {
                var linesWithNoDashDefiles = new List<string>();

                var lines = SourceText.SplitByCRLF();
                foreach (var line in lines)
                {
                    if (line.TrimStart().StartsWith("#"))
                    {
                        var m = Regex.Match(line, @"#define\s+(\w+)\s+(.*)");
                        if (m.Success)
                        {
                            var name = m.Groups[1].Value;
                            var value = m.Groups[2].Value;

                            if (!string.IsNullOrEmpty(value))
                            {
                                value = value.Trim();
                                if(value.StartsWith("\"") && value.EndsWith("\""))
                                    value = value.Substring(1, value.Length - 2);
                            }
                            Macros.Add(name, value);
                        }
                        linesWithNoDashDefiles.Add("");
                    }
                    else linesWithNoDashDefiles.Add(line);
                }
                TextToBeProcessed = string.Join(Environment.NewLine, linesWithNoDashDefiles) + Environment.NewLine;
                return Macros;
            }

            public string Process()
            {
                var text = TextToBeProcessed;
                foreach (var m in Macros)
                    text = text.Replace(m.Key, m.Value);

                return text;
            }
        }
    }
}
