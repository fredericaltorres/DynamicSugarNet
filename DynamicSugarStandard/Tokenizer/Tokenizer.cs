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
            IdentifierPath, // aot/backend/logs
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

            StringLiteralDQuote_FilePath,
            StringLiteralSQuote_FilePath,
            //StringLiteralDQuote_Url,
            //StringLiteralSQuote_Url,
            FilePath,
        }

        private string GetSpaces(StringBuilder sbSpaceCounter )
        {
            var r = sbSpaceCounter.ToString();
            sbSpaceCounter.Clear();
            return r;
        }

        public Tokens Tokenize(string input, bool combineArray = true)
        {
            var tokens = new Tokens();
            int i = 0;
            var sbSpaceCounter = new StringBuilder();
            while (i < input.Length)
            {
                // Skip whitespace
                if (char.IsWhiteSpace(input[i]))
                {
                    sbSpaceCounter.Append(input[i]);
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
                    tokens.Add(new Token($@"{stringBuilder}", TokenType.StringLiteralDQuote, GetSpaces(sbSpaceCounter)));
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
                    tokens.Add(new Token($"{stringBuilder}", TokenType.StringLiteralSQuote, GetSpaces(sbSpaceCounter)));
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
                    tokens.Add(new Token(stringBuilder.ToString(), TokenType.Number, GetSpaces(sbSpaceCounter)));
                    continue;
                }

                // Handle delimiters and operators
                if (IsDelimiter(input[i]))
                {
                    tokens.Add(new Token(input[i].ToString(), TokenType.Delimiter, GetSpaces(sbSpaceCounter)));
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
                    tokens.Add(new Token(identifierBuilder.ToString(), TokenType.Identifier, GetSpaces(sbSpaceCounter)));
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
            var identifierPathValidDelimiters = DS.List(".", "/", @"\", "-");
            var x = 0;
            var r = new Tokens();

            while (x < tokens.Count)
            {
                var tok = GetToken(tokens, x);

                // Detect filename c:\aa \\aa
                if (tok.IsString && ((char.IsLetter(tok.GetValueCharIndex(0)) && tok.GetValueCharIndex(1) == ':') || (tok.Value.StartsWith("\\"))))
                {
                    var quote = tok.IsDString ? @"""" : "'";
                    r.Add(new Token($"{quote}{tok.Value}{quote}", tok.IsDString ? Tokenizer.TokenType.StringLiteralDQuote_FilePath : Tokenizer.TokenType.StringLiteralSQuote_FilePath));
                    x += 1;
                }
                // Identifier .\/- Identifier become one IdentifierPath ""
                else if (GetToken(tokens, x).IsIdentifier() && GetToken(tokens, x, 1).IsDelimiter(identifierPathValidDelimiters) && GetToken(tokens, x, 2).IsIdentifier())
                {
                    var firstToken = GetToken(tokens, x);
                    var subTokens = ReadAllTokenAcceptedForIdentifierPath(tokens, x + 1, identifierPathValidDelimiters);
                    var text = GetToken(tokens, x).Value + subTokens.GetAsText();
                    r.Add(new Token(text, TokenType.IdentifierPath, "", subTokens));
                    x += subTokens.Count + 1;
                }
                // c:\a\windows\system32\cmd.exe
                else if (GetToken(tokens, x).IsIdentifier() && GetToken(tokens, x).Value.Length==1 && char.IsLetter(GetToken(tokens, x).Value[0]) &&
                         GetToken(tokens, x, 1).IsDelimiter(DS.List(":")) && GetToken(tokens, x, 2).IsDelimiter(DS.List(@"\")))
                {
                    var subTokens = ReadAllTokenAcceptedForIdentifierPath(tokens, x, DS.List(":", @"\", "."));
                    var text = subTokens.GetAsText();
                    r.Add(new Token(text, TokenType.FilePath, "", subTokens));
                    x += subTokens.Count + 1;
                }

                // name :[  or "name" :, value or 'name' ::
                else if ((GetToken(tokens, x).IsIdentifier() || GetToken(tokens, x).IsString)
                         && (GetToken(tokens, x).Value.Length > 1) && !GetToken(tokens, x).Value.ToLower().StartsWith("http") &&
                         GetToken(tokens, x, 1).IsDelimiter(DS.List(":", "=")) && GetToken(tokens, x, 2).IsDelimiter())
                {
                    var tokenName = GetToken(tokens, x);
                    var tokenDelimiter = GetToken(tokens, x, 1);
                    r.Add(new Token(tokenName, tokenDelimiter, Token.GetUndefinedToken()));
                    x += 2; // Skip the name, delimiter
                }

                // name :/= value or "name" :/= value or 'name' :/= value
                else if (( GetToken(tokens, x).IsIdentifier() || GetToken(tokens, x).IsString )
                         && (GetToken(tokens, x).Value.Length > 1) && !GetToken(tokens, x).Value.ToLower().StartsWith("http") && 
                         GetToken(tokens, x, 1).IsDelimiter(DS.List(":", "=")) && GetToken(tokens, x, 2).IsAnyValue)
                {
                    var tokenName = GetToken(tokens, x);
                    var tokenDelimiter = GetToken(tokens, x, 1);
                    var tokenVal = GetToken(tokens, x, 2);

                    var remainingTokens =  new Tokens(tokens.Skip(x + 2).ToList());

                    var remainingCombinedTokens = CombineTokens(remainingTokens, combineArray);

                    if (remainingCombinedTokens[0].Type != remainingTokens[0].Type)
                    {
                        tokenVal = remainingCombinedTokens[0];
                        x += remainingCombinedTokens[0].__internalTokens.Count - 1;
                    }

                    r.Add(new Token(tokenName, tokenDelimiter, tokenVal));
                    x += 3; // Skip the name, delimiter, and value
                }

                // http/https:// xxxx
                else if (GetToken(tokens, x).IsIdentifier() && GetToken(tokens, x).Value.ToLower().StartsWith("http"))
                {
                    var subTokens = ReadAllTokenAcceptedForUrl(tokens, x);
                    var text = subTokens.GetAsText();
                    r.Add(new Token(text, TokenType.Url, "", subTokens));
                    x += subTokens.Count; // Skip the closing bracket
                }
                // Array/List
                else if (combineArray && GetToken(tokens, x).IsDelimiter("["))
                {
                    var subTokens = ReadTokenUpTo(tokens, x + 1, "]");
                    r.Add(new Token(subTokens));
                    x += subTokens.Count + 2; // Skip the closing bracket
                }

                //  2025-05-26T22:06:11.513Z and 2025-05-26T22:06:11Z
                else if (
                        GetToken(tokens, x).IsNumber && GetToken(tokens, x, 1).IsDelimiter(DateDelimiters) && 
                        GetToken(tokens, x, 2).IsNumber && GetToken(tokens, x, 3).IsDelimiter(DateDelimiters) 
                        && GetToken(tokens, x, 4).IsNumber &&  GetToken(tokens, x, 5).IsIdentifier(/* cannot pas 'T' because the valye is T22*/) &&
                        GetToken(tokens, x, 6).IsDelimiter(TimeDelimiters) &&
                        GetToken(tokens, x, 7).IsNumber /* << minutes */ && 
                        GetToken(tokens, x, 8).IsDelimiter(TimeDelimiters) &&
                        GetToken(tokens, x, 9).IsNumber && /* << seconds */
                        GetToken(tokens, x, 10).IsIdentifier("Z")
                    )
                {
                    var (dateStr2, subTokens) = ConcatTokens(tokens, x, new Token("Z", Tokenizer.TokenType.Identifier, null));
                    r.Add(new Token(dateStr2, TokenType.DateTime, "", subTokens));
                    x += subTokens.Count;
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
                        var (subStr1, subTokens1) = ConcatTokens(tokens, x, 5);
                        var (subStr2, subTokens2) = ConcatTokens(tokens, x + 5, extraTokenCount);
                        subTokens1.AddRange(subTokens2);
                        var dateStr2 = subStr1 + " " + subStr2; 

                        if(GetToken(tokens, x, 10).IsIdentifier("AM") || GetToken(tokens, x, 10).IsIdentifier("PM"))
                        {
                            var ampmToken = GetToken(tokens, x, 10);
                            dateStr2 += $" {GetToken(tokens, x, 10).Value}"; // Add AM/PM if present
                            extraTokenCount++; // Include AM/PM in the count
                            subTokens1.Add(ampmToken);
                        }

                        x += 5 + extraTokenCount;
                        r.Add(new Token(dateStr2, TokenType.DateTime, "", subTokens1));
                    }
                    else
                    {   // Date time no time
                        var (subStr, subToken) = ConcatTokens(tokens, x, 5);
                        x += subToken.Count;
                        r.Add(new Token(subStr, TokenType.Date, "", subToken));
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

            return r;
        }

        public Tokens ReadAllTokenAcceptedForIdentifierPath(Tokens tokens, int start, List<string> delimiters)
        {
            //var delimiters = DS.List( "-", "/", @"\", ".");
            var r = new Tokens();
            for (int i = start; i < tokens.Count; i++)
            {
                if (tokens[i].IsDelimiter(delimiters) || tokens[i].IsIdentifier())
                {
                    r.Add(tokens[i]);
                }
                else break;
            }
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

        public (string str, Tokens subTokens) ConcatTokens(Tokens tokens, int start, Token token)
        {
            var subTokens = new Tokens();
            var sb = new StringBuilder();
            var i = start;
            var count = 0;
            while (i < tokens.Count && !tokens[i].Is(token))
            {
                subTokens.Add(tokens[i]);
                sb.Append(tokens[i].Value);
                count++;
                i++;
            }
            sb.Append(tokens[i].Value);
            count++;
            return (sb.ToString(), subTokens);
        }

        public (string str, Tokens subTokens) ConcatTokens(Tokens tokens, int start, int count)
        {
            var subTokens = new Tokens();
            var sb = new StringBuilder();
            for (int i = start; i < start + count && i < tokens.Count; i++)
            {
                sb.Append(tokens[i].Value);
                subTokens.Add(tokens[i]);
            }

            return (sb.ToString(), subTokens);
        }
        

        private bool IsDelimiter(char c)
        {
            return c == ',' || c == '.' || c == ';' ||
                   c == '(' || c == ')' || c == '[' || c == ']' ||
                   c == '{' || c == '}' || c == '=' || c == '!' ||
                   c == '-' || c == ':' || c == '/' || c == '\\' || c == '*' || c == '+' ||
                   c == '>' || c == '<' || c == '&' || c == '|' || c == '%' || c == '┊';
            // c == ' ' Space is not a delimiter here, 
        }
    }
}
