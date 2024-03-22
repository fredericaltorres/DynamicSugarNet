using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.IO;
using Newtonsoft.Json.Linq;
using System.ComponentModel.Design;
using static DynamicSugar.DS;
using System.Runtime.CompilerServices;
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

    public class Macro
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string ParameterName { get; set; }

        public bool HasParameter => !string.IsNullOrEmpty(ParameterName);

        public bool ApplyToText(string text)
        {
            var index = text.IndexOf($"{Name}(");
            return index != -1;
        }

        public string Replace(string text, Dictionary<string, Macro> Macros)
        {
            var index = text.IndexOf($"{Name}(");
            if(index != -1)
            {
                var indexEnd = text.IndexOf(")", index);
                var paramValue = text.Substring(index + Name.Length + 1, indexEnd - index - Name.Length - 1);
                var newValue = Processor.Process(paramValue, Macros);

                var z = Value.Replace(this.ParameterName, newValue);
                var start = text.Substring(0, index);
                var end = text.Substring(indexEnd + 1);

                var r = start + z + end;
                return r;
            }
            return text.Replace(Name, Value);
        }
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
            

            public Dictionary<string, Macro> Macros = new Dictionary<string, Macro>();    

            public Processor(string text)
            {
                SourceText = text;
            }

            public Processor ExtractMacros(string jsonIdsFile = null)
            {
                var regExNoParameter   = @"#define\s+(\w+)\s+(.*)";
                var regExWithParameter = @"#define\s+(\w+)\((\w+)\)\s+(.*)";

                var linesWithNoDashDefiles = new List<string>();

                var lines = SourceText.SplitByCRLF();
                foreach (var line in lines)
                {
                    if (line.TrimStart().StartsWith("#"))
                    {
                        var m1 = Regex.Match(line, regExWithParameter);
                        if (m1.Success)
                        {
                            var name = m1.Groups[1].Value;
                            var param = m1.Groups[2].Value;
                            var value = m1.Groups[3].Value;

                            if (!string.IsNullOrEmpty(value))
                            {
                                value = value.Trim();
                                if (value.StartsWith("\"") && value.EndsWith("\""))
                                    value = value.Substring(1, value.Length - 2);
                            }
                            Macros.Add(name, new Macro { Name = name, Value = value, ParameterName = param });
                        }
                        else
                        {
                            var m2 = Regex.Match(line, regExNoParameter);
                            if (m2.Success)
                            {
                                var name = m2.Groups[1].Value;
                                var value = m2.Groups[2].Value;

                                if (!string.IsNullOrEmpty(value))
                                {
                                    value = value.Trim();
                                    if (value.StartsWith("\"") && value.EndsWith("\""))
                                        value = value.Substring(1, value.Length - 2);
                                }
                                Macros.Add(name, new Macro { Name = name, Value = value });
                            }
                            else
                            {
                                throw new ProcessorFailedException($"Failed to parse #define {line}");
                            }
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

                        Macros.Add(key, new Macro { Name = key, Value = val });
                    }
                }
            }
            public static string Process(string text, Dictionary<string, Macro> macros)
            {
                foreach (var m in macros.Values)
                    if (!m.HasParameter)
                        text = text.Replace(m.Name, m.Value);

                foreach (var m in macros.Values)
                    if (m.HasParameter)
                        text = m.Replace(text, macros);

                return text;
            }

            public string ProcessMain()
            {
                string text = TextToBeProcessed;
                var go = true;
                while (go)
                {
                    go = false;
                    foreach (var m in Macros.Values)
                    {
                        if (!m.HasParameter && text.Contains(m.Name))
                        {
                            text = text.Replace(m.Name, m.Value);
                            go = true;
                        }
                    }

                    foreach (var m in Macros.Values)
                    {
                        if (m.HasParameter && m.ApplyToText(text))
                        {
                            text = m.Replace(text, this.Macros);
                            go = true;
                        }
                    }
                }
                return text;
            }
        }
    }
}
