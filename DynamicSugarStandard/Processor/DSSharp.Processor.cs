using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.IO;
using Newtonsoft.Json.Linq;
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

            public Processor ExtractMacros(string jsonIdsFile = null)
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
                LoadMacros(jsonIdsFile);
                TextToBeProcessed = string.Join(Environment.NewLine, linesWithNoDashDefiles) + Environment.NewLine;
                return this;
            }
            private void LoadMacros(string jsonIdsFile)
            {
                if (jsonIdsFile != null && File.Exists(jsonIdsFile))
                {
                    var json = File.ReadAllText(jsonIdsFile);
                    var jsonO = JObject.Parse(json);
                    var keys = jsonO.Properties().Select(p => p.Name);
                    foreach (var key in keys)
                    {
                        var val = jsonO[key].ToString();
                        if(Macros.ContainsKey(key))
                            Macros.Remove(key);

                        Macros.Add(key, val);
                    }
                }
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
