using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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
            var startBracket = bracketType == BracketType.Curly ? "{" : "[";
            var endBracket = bracketType == BracketType.Curly ? "}" : "]";
            var searchStartIndex = 0;

            while (true)
            {
                var startBracketIndex = text.IndexOf(startBracket, searchStartIndex);
                if(startBracketIndex ==-1)
                    return null;

                var endBracketIndex = text.IndexOf(endBracket, startBracketIndex);

                if (endBracketIndex == -1 || startBracketIndex == -1)
                    return null;

                if (startBracketIndex >= endBracketIndex)
                    return null;

                var subText = text.Substring(startBracketIndex, endBracketIndex - startBracketIndex + 1);

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

        public static JsonExtractionType DetectJsonType(string text)
        {
            var curlyBraceIndex = text.IndexOf("{");
            var squareBraceIndex = text.IndexOf("[");

            if(curlyBraceIndex > -1 && squareBraceIndex == -1)
                return JsonExtractionType.Object;

            if (curlyBraceIndex == -1 && squareBraceIndex > -1)
                return JsonExtractionType.Array;

            if (curlyBraceIndex > -1 && squareBraceIndex > -1)
            {
                var rCurly = Grab(text, BracketType.Curly) != null;
                var rSquare = Grab(text, BracketType.Square) != null;

                if (rCurly && !rSquare)
                    return JsonExtractionType.Object;
                if (!rCurly && rSquare)
                    return JsonExtractionType.Array;

                if (rCurly && rSquare)
                {
                    return JsonExtractionType.Array; // most likely an array containing objects
                }
            }

            return JsonExtractionType.Unknown;
        }

        public static string Extract(string text)
        {
            var JsonType = DetectJsonType(text);
            switch (JsonType)
            {
                case JsonExtractionType.Object:
                    return Grab(text, BracketType.Curly, format: true);
                case JsonExtractionType.Array:
                    return Grab(text, BracketType.Square, format: true);
                default:
                    return null;
            }
        }





    }
}
