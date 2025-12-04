using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace DynamicSugar {

    public class WildCard
    {
        public static bool IsMatch(string text, string patterns)
        {
            if (string.IsNullOrEmpty(patterns))
                throw new ArgumentNullException($"parameters patterns cannot be '{patterns}'");

            var patternsList = patterns.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pattern in patternsList)
            {
                if (IsMatchOne(text, pattern.Trim()))
                    return true;
            }
            return false;
        }
        public static bool IsMatchOne(string text, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentNullException($"parameters patterns cannot be '{pattern}'");

            // 1. Escape special Regex characters in the pattern (like ., +, [, etc.)
            //    so they are treated as literal characters.
            string regexPattern = Regex.Escape(pattern);

            // 2. Replace the escaped Wildcards back to Regex syntax
            //    \* becomes .* (any sequence)
            //    \? becomes .  (any single character)
            regexPattern = "^" + regexPattern.Replace("\\*", ".*").Replace("\\?", ".") + "$";

            // 3. Check match (IgnoreCase is usually preferred for wildcards)
            return Regex.IsMatch(text, regexPattern, RegexOptions.IgnoreCase);
        }
    }
}


