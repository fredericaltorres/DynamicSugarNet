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
                    var subText2 = TryGrabNextClosingBracket(text, startBracketIndex, endBracketIndex, endBracketChar);
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

        private static string TryGrabNextClosingBracket(string text, int startBracketIndex, int endBracketIndex, string endBracket, int recursionIndex = 0)
        {
            //var endBracketIndex0 = text.IndexOf(endBracket, endBracketIndex + 1);
            var endBracketIndex0 = text.LastIndexOf(endBracket);
            if (endBracketIndex0 == -1)
                return null;

            var subText = text.Substring(startBracketIndex, endBracketIndex0 - startBracketIndex + 1);
            if (IsValidJson(subText))
                return subText;
            else
            {
                if(recursionIndex > 10)
                    return null;
                return TryGrabNextClosingBracket(text, startBracketIndex, endBracketIndex + 1, endBracket, recursionIndex + 1);
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
