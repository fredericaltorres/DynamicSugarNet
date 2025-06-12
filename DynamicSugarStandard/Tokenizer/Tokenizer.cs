using DynamicSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicSugar
{
    public partial class Tokenizer
    {
        /*
           Create a method that tokenizes a string into a list of tokens.
           the characters that are used to tokenize are:
           - " " (space)
           - "," (comma)
           - "." (dot)
           - ";" (semicolon)
           - "(" (open parenthesis)
           - ")" (close parenthesis)
           - "[" (open bracket)
           - "]" (close bracket)
           - "{" (open brace)
           - "}" (close brace)
           - "=" (equal)
           - "!" (exclamation mark)
           - ">" (greater than)
           - "<" (less than)
           - "&" (ampersand)
           - "|" (pipe)
           - "^" (caret)
           - "~" (tilde)
           - "`" (backtick)
           - "'" (single quote)
           - "/" (slash)
           - "\\" (backslash)
           - "+" (plus)
           - "-" (minus)
           - "*" (asterisk)
           - "%" (percent)
           - "@" (at)
           - "#" (hash)
           - "$" (dollar)
           - " " (space)


           - "\"" (double quote string should be tokenized as a single token)
        */

        public enum TokenType
        {
            Identifier,
            StringLiteralDQuote,
            StringLiteralSQuote,
            Number,
            Operator,
            Delimiter,
            Keyword,
            Comment,


            UndefinedToken,
            Date,
            DateTime,
            ArrayOfTokens,
            NameValuePair,
            Url,
        }

        public Tokens Tokenize(string input, bool combineArray = true)
        {
            var tokens = new Tokens();

            int i = 0;
            while (i < input.Length)
            {
                // Skip whitespace
                if (char.IsWhiteSpace(input[i]))
                {
                    i++;
                    continue;
                }

                // Handle string literals Double quotes
                if (input[i] == '"')
                {
                    var stringBuilder = new StringBuilder();
                    i++; // Skip opening quote
                    while (i < input.Length && input[i] != '"')
                    {
                        stringBuilder.Append(input[i]);
                        i++;
                    }
                    if (i < input.Length) i++; // Skip closing quote
                    tokens.Add(new Token(stringBuilder.ToString(), TokenType.StringLiteralDQuote));
                    continue;
                }

                // Handle string literals Single quotes
                if (input[i] == '\'')
                {
                    var stringBuilder = new StringBuilder();
                    i++; // Skip opening quote
                    while (i < input.Length && input[i] != '\'')
                    {
                        stringBuilder.Append(input[i]);
                        i++;
                    }
                    if (i < input.Length) i++; // Skip closing quote
                    tokens.Add(new Token(stringBuilder.ToString(), TokenType.StringLiteralSQuote));
                    continue;
                }

                // Handle numbers
                if (char.IsDigit(input[i]))
                {
                    var stringBuilder = new StringBuilder();
                    while (i < input.Length && (char.IsDigit(input[i]) || input[i] == '.'))
                    {
                        stringBuilder.Append(input[i]);
                        i++;
                    }
                    tokens.Add(new Token(stringBuilder.ToString(), TokenType.Number));
                    continue;
                }

                // Handle delimiters and operators
                if (IsDelimiter(input[i]))
                {
                    tokens.Add(new Token(input[i].ToString(), TokenType.Delimiter));
                    i++;
                    continue;
                }

                // Handle identifiers and keywords
                var identifierBuilder = new StringBuilder();
                while (i < input.Length && !IsDelimiter(input[i]) && !char.IsWhiteSpace(input[i]))
                {
                    identifierBuilder.Append(input[i]);
                    i++;
                }
                if (identifierBuilder.Length > 0)
                {
                    tokens.Add(new Token(identifierBuilder.ToString(), TokenType.Identifier));
                }
            }

            return CombineTokens(tokens, combineArray);
        }

        public Token GetToken(Tokens tokens, int x, int inc = 0)
        {
            if (x + inc < 0 || x + inc >= tokens.Count)
                return Token.GetUndefinedToken();
            return tokens[x + inc];
        }

        public Token GetPreviousToken(Tokens tokens, int x, int dec)
        {
            if (x - dec < 0 || x - dec >= tokens.Count)
                return Token.GetUndefinedToken();
            return tokens[x - dec];
        }

        static List<string> DateDelimiters = new List<string> { "-", "/"};
        static List<string> TimeDelimiters = new List<string> { ":", "-"};

        public Tokens CombineTokens(Tokens tokens, bool combineArray = true)
        {
            
            var x = 0;
            var r = new Tokens();
            var requireRerun = false;

            while (x < tokens.Count)
            {
                var tok = GetToken(tokens, x);

                // Identifier . Identifier become one Identifier ""
                if (GetToken(tokens, x).IsIdentifier() && GetToken(tokens, x, 1).IsDelimiter(".") && GetToken(tokens, x, 2).IsIdentifier())
                {
                    var t1 = GetToken(tokens, x);
                    var t2 = GetToken(tokens, x, 2);
                    r.Add(new Token($"{t1.Value}.{t2.Value}", Tokenizer.TokenType.Identifier));
                    x += 3; // Skip the name, delimiter, and value
                    requireRerun = true;
                }
                // name := value
                else if (GetToken(tokens, x).IsIdentifier() && !GetToken(tokens, x).Value.ToLower().StartsWith("http") && 
                         GetToken(tokens, x, 1).IsDelimiter(DS.List(":", "=")) && GetToken(tokens, x, 2).IsAnyValue)
                {
                    var name = GetToken(tokens, x).Value;
                    var val = GetToken(tokens, x, 2).Value;
                    r.Add(new Token(name, val));
                    x += 3; // Skip the name, delimiter, and value
                }
                // http/https:// xxxx
                else if (GetToken(tokens, x).IsIdentifier() && GetToken(tokens, x).Value.ToLower().StartsWith("http"))
                {
                    var subTokens = ReadAllTokenAcceptedForUrl(tokens, x + 1);
                    var text = GetToken(tokens, x).Value + subTokens.GetAsText();
                    r.Add(new Token(text, TokenType.Url));
                    x += subTokens.Count + 2; // Skip the closing bracket
                }
                // Array/List
                else if (combineArray && GetToken(tokens, x).IsDelimiter("["))
                {
                    var subTokens = ReadTokenUpTo(tokens, x + 1, "]");
                    r.Add(new Token(subTokens));
                    x += subTokens.Count + 2; // Skip the closing bracket
                }
                //  2025-05-26T22:06:11.513Z
                else if (
                        GetToken(tokens, x).IsNumber && GetToken(tokens, x, 1).IsDelimiter(DateDelimiters) && 
                        GetToken(tokens, x, 2).IsNumber && GetToken(tokens, x, 3).IsDelimiter(DateDelimiters) 
                        && GetToken(tokens, x, 4).IsNumber &&  GetToken(tokens, x, 5).IsIdentifier(/*Thours*/) &&
                        GetToken(tokens, x, 6).IsDelimiter(TimeDelimiters) &&
                        GetToken(tokens, x, 7).IsNumber /* << minutes */ && 
                        GetToken(tokens, x, 8).IsDelimiter(TimeDelimiters) &&
                        GetToken(tokens, x, 9).IsNumber && /* << seconds */
                        GetToken(tokens, x, 10).IsIdentifier(/*Z*/)
                    )
                {
                    var (dateStr2, countTokenConcatenated) = ConcatTokens(tokens, x, new Token("Z", Tokenizer.TokenType.Identifier));
                    r.Add(new Token(dateStr2, TokenType.DateTime));
                    x += countTokenConcatenated;
                }
                // @"2025-05-24 13:16:52.859";
                // Date YYYY:MM:DD
                else if (GetToken(tokens, x).IsNumber && GetToken(tokens, x, 1).IsDelimiter(DateDelimiters) && // YY-
                        GetToken(tokens, x, 2).IsNumber && GetToken(tokens, x, 3).IsDelimiter(DateDelimiters) &&  // MM-
                        GetToken(tokens, x, 4).IsNumber // Space is not tested as a separator
                        )
                {
                    if (
                        GetToken(tokens, x, 5).IsNumber /* << hours */&& GetToken(tokens, x, 6).IsDelimiter(TimeDelimiters) &&
                        GetToken(tokens, x, 7).IsNumber /* << minutes */ && GetToken(tokens, x, 8).IsDelimiter(TimeDelimiters) &&
                        GetToken(tokens, x, 9).IsNumber /* << seconds */
                    )
                    { // Date + time
                        var extraTokenCount = 5;
                        if (GetToken(tokens, x, 9).IsFloat)
                        {
                            // The second and milliseconds are combined are parsed as a float and one token
                            /* : 52.123 */
                        }
                        var dateStr2 = ConcatTokens(tokens, x, 5) + " " + ConcatTokens(tokens, x + 5, extraTokenCount);

                        if(GetToken(tokens, x, 10).IsIdentifier("AM") || GetToken(tokens, x, 10).IsIdentifier("PM"))
                        {
                            dateStr2 += $" {GetToken(tokens, x, 10).Value}"; // Add AM/PM if present
                            extraTokenCount++; // Include AM/PM in the count
                        }


                        x += 5 + extraTokenCount;
                        r.Add(new Token(dateStr2, TokenType.DateTime));
                    }
                    else
                    {   // Date time no time
                        var dateStr = ConcatTokens(tokens, x, 5);
                        x += 5;
                        r.Add(new Token(dateStr, TokenType.Date));
                    }
                }
                else
                {
                    r.Add(GetToken(tokens, x));
                    x++;
                }
            }

            foreach (var token in r)
                if(token.Type == TokenType.ArrayOfTokens)
                    token.ArrayValues = CombineTokens(token.ArrayValues, combineArray);

            if (requireRerun)
                return CombineTokens(r, combineArray); // Re-Run another pass of combining tokens
            else 
                return r;
        }

        public Tokens ReadAllTokenAcceptedForUrl(Tokens tokens, int start)
        {
            var delimiters = DS.List("+", "-", "/", ":", "&", ".");
            var r = new Tokens();
            for (int i = start; i < tokens.Count; i++)
            {
                if (tokens[i].IsDelimiter(delimiters) || tokens[i].IsIdentifier() || tokens[i].IsNumber)
                {
                    r.Add(tokens[i]);
                }
                else break;
            }
            return r;
        }

        public Tokens ReadTokenUpTo(Tokens tokens, int start, string delimiter)
        {
            var r = new Tokens();
            for (int i = start; i < tokens.Count; i++)
            {
                if (tokens[i].IsDelimiter(delimiter))
                    break;
                r.Add(tokens[i]);
            }
            return r;
        }

        public (string str, int count) ConcatTokens(Tokens tokens, int start, Token token)
        {
            var sb = new StringBuilder();
            var i = start;
            var count = 0;
            while (i < tokens.Count && !tokens[i].Is(token))
            {
                sb.Append(tokens[i].Value);
                count++;
                i++;
            }
            sb.Append(tokens[i].Value);
            count++;
            return (sb.ToString(), count);
        }

        public string ConcatTokens(Tokens tokens, int start, int count)
        {
            var sb = new StringBuilder();
            for (int i = start; i < start + count && i < tokens.Count; i++)
                sb.Append(tokens[i].Value);
            return sb.ToString();
        }
        

        private bool IsDelimiter(char c)
        {
            return c == ',' || c == '.' || c == ';' ||
                   c == '(' || c == ')' || c == '[' || c == ']' ||
                   c == '{' || c == '}' || c == '=' || c == '!' ||
                   c == '-' || c == ':' || c == '/' || c == '\\' || c == '*' || c == '+' ||
                   c == '>' || c == '<' || c == '&' || c == '|' || c == '%';
            // c == ' ' Space is not a delimiter here, 
        }
    }
}
