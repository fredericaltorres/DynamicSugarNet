using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DynamicSugar
{
    public class JsonExtractor
    {
        public static JObject ParseObject(string json)
        {
            try
            {
                var o = JsonConvert.DeserializeObject(json);
                return o as JObject;
            }
            catch
            {
                return null;
            }
        }

        public static JArray ParseArray(string json)
        {
            try
            {
                var o = JsonConvert.DeserializeObject(json);
                return o as JArray;
            }
            catch
            {
                return null;
            }
        }

        public static bool IsValidJson(string json)
        {
            try
            {
                var o = JsonConvert.DeserializeObject(json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public enum JsonExtractionType
        {
            Array,
            Object,
            Unknown
        }

        public enum BracketType
        {
            Curly, Square
        }

        public static string Format(object o)
        {
            return JsonConvert.SerializeObject(o, Formatting.Indented);
        }

        public static string Format(string json)
        {
            var o = JsonConvert.DeserializeObject(json);
            return Format(o);
        }

        public static string Grab(string text, BracketType bracketType, bool format = false)
        {
            var startBracketChar = bracketType == BracketType.Curly ? "{" : "[";
            var endBracketChar   = bracketType == BracketType.Curly ? "}" : "]";
            var searchStartIndex = 0;

            while (true)
            {
                var startBracketIndex = text.IndexOf(startBracketChar, searchStartIndex);
                if(startBracketIndex ==-1)
                    return null;

                var endBracketIndex = text.IndexOf(endBracketChar, startBracketIndex);

                if (endBracketIndex == -1 || startBracketIndex == -1)
                    return null;

                if (startBracketIndex >= endBracketIndex)
                    return null;

                var subText = text.Substring(startBracketIndex, endBracketIndex - startBracketIndex + 1);

                if (!IsValidJson(subText))
                {
                    var subText2 = TryGrabNextClosingBracket(text, startBracketIndex, endBracketIndex, endBracketChar, newStrategy: true);
                    if(subText2 != null)
                        subText = subText2;
                }

                if (IsValidJson(subText))
                {
                    if (format)
                    {
                        subText = Format(subText);
                    }
                    return subText;
                }
                else
                {
                    searchStartIndex = startBracketIndex + 1;
                }
            }
            return null;
        }

        private static string TryGrabNextClosingBracket(string text, int startBracketIndex, int endBracketIndex, string endBracket, bool newStrategy = false, int recursionIndex = 0)
        {
            var startBracket = endBracket == "]" ? "[" : "{";
            //var endBracketIndex0 = text.IndexOf(endBracket, endBracketIndex + 1);
            var endBracketIndex0 = text.LastIndexOf(endBracket);
            if (endBracketIndex0 == -1)
                return null;

            if(startBracketIndex < 0)
                return null;

            var subText = text.Substring(startBracketIndex, endBracketIndex0 - startBracketIndex + 1);
            if (IsValidJson(subText))
                return subText;
            else
            {
                if(recursionIndex > 10)
                    return null;

                var nextStartBracketIndex = startBracketIndex;

                if (newStrategy)
                {
                    nextStartBracketIndex = text.IndexOf(startBracket, startBracketIndex + 1);
                    if (nextStartBracketIndex == -1)
                        return null;
                }

                return TryGrabNextClosingBracket(text, nextStartBracketIndex, endBracketIndex + 1, endBracket, recursionIndex: recursionIndex + 1);
            }
        }

        public static JsonExtractionType DetectJsonType(string text)
        {
            var curlyBraceIndex = text.IndexOf("{");
            var squareBraceIndex = text.IndexOf("[");

            if(curlyBraceIndex > -1 && squareBraceIndex == -1)
                return JsonExtractionType.Object;

            if (curlyBraceIndex == -1 && squareBraceIndex > -1)
                return JsonExtractionType.Array;

            if (curlyBraceIndex > -1 && squareBraceIndex > -1) // case we found both [ and {
            {
                //if(curlyBraceIndex < squareBraceIndex)
                //    return JsonExtractionType.Object;
                //else
                //    return JsonExtractionType.Array;

                var rCurlyJson = Grab(text, BracketType.Curly);
                var rSquareJson = Grab(text, BracketType.Square);
                var rCurly = rCurlyJson != null;
                var rSquare = rSquareJson != null;

                if (rCurly && !rSquare)
                    return JsonExtractionType.Object;
                if (!rCurly && rSquare)
                    return JsonExtractionType.Array;

                var rCurlyJsonIndex = rCurlyJson == null ? 0 : text.IndexOf(rCurlyJson);
                var rSquareJsonIndex = rSquareJson == null ? 0 : text.IndexOf(rSquareJson);

                if (rCurly && rSquare)
                {
                    if (rSquareJsonIndex < rCurlyJsonIndex) // The '[' was detected before the '{'
                        return JsonExtractionType.Array; // most likely an array containing objects
                    else
                        return JsonExtractionType.Object; // most likely an object containing an array
                }
            }

            return JsonExtractionType.Unknown;
        }

        public static List<string> ExtractJsonBlobs_Claude_2026_07(string input)
        {
            var results = new List<string>();
            int i = 0;

            while (i < input.Length)
            {
                if (input[i] == '{' || input[i] == '[')
                {
                    int start = i;
                    bool inString = false;
                    bool escaped = false;
                    var stack = new Stack<char>();

                    for (; i < input.Length; i++)
                    {
                        char c = input[i];

                        if (escaped) { escaped = false; continue; }
                        if (c == '\\' && inString) { escaped = true; continue; }
                        if (c == '"') { inString = !inString; continue; }
                        if (inString) continue;

                        if (c == '{' || c == '[')
                        {
                            stack.Push(c);
                        }
                        else if (c == '}' || c == ']')
                        {
                            // Mismatched or unexpected closer → not valid JSON, abandon this candidate
                            if (stack.Count == 0 ||
                                (c == '}' && stack.Peek() != '{') ||
                                (c == ']' && stack.Peek() != '['))
                            {
                                break;
                            }

                            stack.Pop();

                            if (stack.Count == 0)
                            {
                                results.Add(input.Substring(start, i - start + 1));
                                break;
                            }
                        }
                    }

                    if (stack.Count > 0) 
                    {
                        throw new InvalidOperationException($"Unmatched opening bracket at position { start } in input string.");
                    }
                }
                i++;
            }

            results = results.Where(x => IsValidJson(x)).ToList();

            return results.Select(x => IsValidJson(x) ? Format(x): x).ToList();
        }

    public static List<string> Extract(string text, JsonExtractionType JsonType = JsonExtractionType.Unknown)
        {
            if (JsonType == JsonExtractionType.Unknown) // Detect if we have a {} or [], note there issue detecting the wrong type
                JsonType = DetectJsonType(text);

            switch (JsonType)
            {
                case JsonExtractionType.Object:
                    //return Grab(text, BracketType.Curly, format: true);
                    return ExtractJsonBlobs_Claude_2026_07(text);

                case JsonExtractionType.Array:
                    //return  new List<string> { Grab(text, BracketType.Square, format: true) }   ;
                    return ExtractJsonBlobs_Claude_2026_07(text);
                default:
                    return null;
            }
        }
    }
}
