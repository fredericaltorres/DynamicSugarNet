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
            TimeZoneOffset, // -05:00 or +02:00
            Time, // 13:45:30 or 13:45:30.123
            NameValuePair,
            Url,
            CommandLineParameter, // --name value or -n value or /name

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

        public Tokens Tokenize(string input)
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

            return CombineTokens(tokens);
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

        public Tokens CombineTokensPhase2(Tokens tokens)
        {
            var identifierPathValidDelimiters = DS.List(".", "/", @"\", "-");
            var x = 0;
            var r = new Tokens();

            while (x < tokens.Count)
            {
                var tok = GetToken(tokens, x);

                // Detect filename c:\aa \\aa
                if (tok.IsDelimiter(@"\") && GetToken(tokens, x + 1).IsIdentifierPath)
                {
                    var filePathToken = GetToken(tokens, x + 1);
                    var newToken = new Token( tok.Value + filePathToken.Value, TokenType.FilePath, tok.PreSpaces + filePathToken.PreSpaces);
                    r.Add(newToken);
                    x += 2;
                }
                else
                {
                    r.Add(GetToken(tokens, x));
                    x++;
                }
            }

            return r;
        }

        public Tokens CombineTokens(Tokens tokens)
        {
            var identifierPathValidDelimiters = DS.List(".", "/", @"\", "-");
            var x = 0;
            var r = new Tokens();
            var requireReRun = false;

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

                // Time 
                else if ( tok.IsNumber && GetToken(tokens, x, 1).IsDelimiter(":") && GetToken(tokens, x, 2).IsNumber
                    && GetToken(tokens, x, 3).IsDelimiter(":") && GetToken(tokens, x, 4).IsNumber)
                {
                    var hourToken = tok;
                    var delimiter1 = GetToken(tokens, x, 1);
                    var minuteToken = GetToken(tokens, x, 2);
                    var delimiter2 = GetToken(tokens, x, 3);
                    var secondToken = GetToken(tokens, x, 4);

                    var newToken = new Token(tok.Value + delimiter1.Value + minuteToken.Value + delimiter2.Value + secondToken.Value, TokenType.Time, tok.PreSpaces + delimiter1.PreSpaces + minuteToken.PreSpaces + delimiter2.PreSpaces + secondToken.PreSpaces);
                    r.Add(newToken);
                    x += 5; // Skip the delimiter and the number
                }

                // -+ Timzone offset
                else if (tok.IsDelimiter(DS.List("-", "+")) && GetToken(tokens, x, 1).IsNumber && GetToken(tokens, x, 2).IsDelimiter(":") && GetToken(tokens, x, 3).IsNumber)
                {
                    var numberToken1 = GetToken(tokens, x, 1);
                    var seminColorToken = GetToken(tokens, x, 2);
                    var numberToken2 = GetToken(tokens, x, 3);
                    var newToken = new Token(tok.Value + numberToken1.Value + seminColorToken.Value + numberToken2.Value, TokenType.TimeZoneOffset,  tok.PreSpaces + numberToken1.PreSpaces + seminColorToken.PreSpaces + numberToken2.PreSpaces);
                    r.Add(newToken);
                    x += 4; // Skip the delimiter and the number
                }

                // - number
                else if (tok.IsDelimiter("-") && GetToken(tokens, x, 1).IsNumber)
                {
                    var numberToken = GetToken(tokens, x, 1);
                    var newToken = new Token(tok.Value + numberToken.Value, TokenType.Number, tok.PreSpaces + numberToken.PreSpaces);
                    r.Add(newToken);
                    x += 2;
                }
                // --identifier
                else if (tok.IsDelimiter("-") && tok.HasPreSpace &&  GetToken(tokens, x, 1).IsDelimiter("-") && GetToken(tokens, x, 1).HasEmptyPreSpace && GetToken(tokens, x, 2).IsIdentifier() && GetToken(tokens, x, 2).HasEmptyPreSpace)
                {
                    var minus1Token = GetToken(tokens, x, 0);
                    var minus2Token = GetToken(tokens, x, 1);
                    var idToken = GetToken(tokens, x, 2);
                    var possibleValueToken = GetToken(tokens, x, 3);
                    if (possibleValueToken.IsString || possibleValueToken.IsIdentifier() || possibleValueToken.IsNumber || possibleValueToken.IsDate || possibleValueToken.IsDateTime)
                    {
                        var newToken = new Token(minus1Token.Value + minus2Token.Value + idToken.Value, TokenType.CommandLineParameter, minus1Token.PreSpaces + minus2Token.PreSpaces + idToken.PreSpaces);
                        newToken.__internalTokens = new Tokens() { possibleValueToken }; // Add the value token as an internal token}
                        r.Add(newToken);
                        x += 4;
                    }
                    else
                    {
                        var newToken = new Token(minus1Token.Value + minus2Token.Value + idToken.Value, TokenType.CommandLineParameter, minus1Token.PreSpaces + minus2Token.PreSpaces + idToken.PreSpaces);
                        r.Add(newToken);
                        x += 3;
                    }
                }

                // -identifier or /identifier
                else if (tok.IsDelimiter(DS.List("-", "/")) && tok.HasPreSpace  && GetToken(tokens, x, 1).IsIdentifier() && GetToken(tokens, x, 1).HasEmptyPreSpace)
                {
                    var minus1Token = GetToken(tokens, x, 0);
                    var idToken = GetToken(tokens, x, 1);
                    var newToken = new Token(minus1Token.Value + idToken.Value, TokenType.CommandLineParameter, minus1Token.PreSpaces + idToken.PreSpaces);
                    r.Add(newToken);
                    x += 2;
                }

                // Identifier .\/- Identifier become one IdentifierPath ""
                else if (GetToken(tokens, x).IsIdentifier() && GetToken(tokens, x, 1).IsDelimiter(identifierPathValidDelimiters) && GetToken(tokens, x, 2).IsIdentifier())
                {
                    var subTokens = ReadAllTokenAcceptedForIdentifierPath(tokens, x + 1, identifierPathValidDelimiters);
                    if (subTokens.Count == 0)
                    {
                        r.Add(GetToken(tokens, x));
                        x += 1;
                    }
                    else
                    {
                        var text = GetToken(tokens, x).Value + subTokens.GetAsText();
                        subTokens.Insert(0, GetToken(tokens, x)); // Add the first token to the subTokens
                        r.Add(new Token(text, TokenType.IdentifierPath, "", subTokens));
                        x += subTokens.Count;
                        requireReRun = true; //  for IdentifierPath: We need to re-run the loop to check for more identifiers 
                    }
                }
                // c:\a\windows\system32\cmd.exe
                else if (GetToken(tokens, x).IsIdentifier() && GetToken(tokens, x).Value.Length==1 && char.IsLetter(GetToken(tokens, x).Value[0]) &&
                         GetToken(tokens, x, 1).IsDelimiter(DS.List(":")) && GetToken(tokens, x, 2).IsDelimiter(DS.List(@"\")))
                {
                    var subTokens = ReadAllTokenAcceptedForIdentifierPath(tokens, x, DS.List(":", @"\", "."));
                    var text = subTokens.GetAsText();
                    r.Add(new Token(text, TokenType.FilePath, "", subTokens));
                    x += subTokens.Count;
                }

                // name :[  or "name" :, value or 'name' ::
                else if ((GetToken(tokens, x).IsIdentifier() || GetToken(tokens, x).IsString)
                         && (GetToken(tokens, x).Value.Length > 1) && !GetToken(tokens, x).Value.ToLower().StartsWith("http") &&
                         GetToken(tokens, x, 1).IsDelimiter(DS.List(":", "=")) && GetToken(tokens, x, 2).IsDelimiter())
                {
                    var tokenName = GetToken(tokens, x);
                    var tokenDelimiter = GetToken(tokens, x, 1);
                    r.Add(new Token(tokenName, tokenDelimiter, Token.GetUndefinedToken()));
                    x += 2;
                }

                // name :/= value or "name" :/= value or 'name' :/= value
                else if (( GetToken(tokens, x).IsIdentifier() || GetToken(tokens, x).IsString )
                         && (GetToken(tokens, x).Value.Length > 1) && !GetToken(tokens, x).Value.ToLower().StartsWith("http") && 
                         GetToken(tokens, x, 1).IsDelimiter(DS.List(":", "=")) && GetToken(tokens, x, 2).IsAnyValue)
                {
                    var tokenName = GetToken(tokens, x);
                    var tokenDelimiter = GetToken(tokens, x, 1);
                    var tokenVal = GetToken(tokens, x, 2);

                    r.Add(new Token(tokenName, tokenDelimiter, tokenVal));
                    x += 3;
                }

                // http/https:// xxxx
                else if (GetToken(tokens, x).IsIdentifier() && GetToken(tokens, x).Type != TokenType.IdentifierPath && 
                         GetToken(tokens, x).Value.ToLower().StartsWith("http"))
                {
                    var subTokens = ReadAllTokenAcceptedForUrl(tokens, x);
                    var text = subTokens.GetAsText();
                    r.Add(new Token(text, TokenType.Url, "", subTokens));
                    x += subTokens.Count; // Skip the closing bracket
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
                    x += subTokens.Count + 1;
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

            if(requireReRun)
            {
                r = CombineTokens(r);
            }

            return CombineTokensPhase2(r);
        }

        public Tokens ReadAllTokenAcceptedForIdentifierPath(Tokens tokens, int start, List<string> delimiters)
        {
            //var delimiters = DS.List( "-", "/", @"\", ".");
            var r = new Tokens();
            for (int i = start; i < tokens.Count; i++)
            {
                if ((tokens[i].IsDelimiter(delimiters) && tokens[i].HasEmptyPreSpace) || 
                    (tokens[i].IsIdentifier() && (tokens[i].HasEmptyPreSpace || i == start /* accept previous space for the first word*/)))
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

        public enum AnalyzedJsonLineType  {
            StartObject,
            StartArray,
            EndObject,
            EndArray,
            StartPropertyObject,
            StartPropertyArray,
            PropertyDate,
            PropertyNumber,
            PropertyString,
            PropertyBool,
            PropertyNull,
            Unknown,
        };

        public class AnalysedJsonLine
        {
            public AnalyzedJsonLineType Type { get; set; }
            public string Line { get; set; }
            public object Tag { get; set; }
            public AnalysedJsonLine(AnalyzedJsonLineType type, string line)
            {
                Type = type;
                Line = line;
            }
        }

        public  List<AnalysedJsonLine> AnalyzeFormattedJson(string formattedJson)
        {
            var r = new List<AnalysedJsonLine>();
            var jsonLines = formattedJson.SplitByCRLF();

            foreach (var jsonLine in jsonLines)
            {
                var x = 0;
                var tokens = new Tokenizer().Tokenize(jsonLine);

                Token curToken = tokens[x];
                Token nextToken = Token.GetUndefinedToken();
                if(x + 1 < tokens.Count)
                    nextToken = tokens[x + 1];

                if(curToken.AssertDelimiter("{", throwEx: false) && tokens.Count == 1)
                    r.Add(new AnalysedJsonLine(AnalyzedJsonLineType.StartObject, jsonLine));

                else if (curToken.AssertDelimiter("}", throwEx: false) && tokens.Count == 1)
                    r.Add(new AnalysedJsonLine(AnalyzedJsonLineType.EndObject, jsonLine));

                else if (curToken.AssertDelimiter("[", throwEx: false) && tokens.Count == 1)
                    r.Add(new AnalysedJsonLine(AnalyzedJsonLineType.StartArray, jsonLine));

                else if (curToken.AssertDelimiter("]", throwEx: false) && tokens.Count == 1)
                    r.Add(new AnalysedJsonLine(AnalyzedJsonLineType.EndArray, jsonLine));

                else if (curToken.Type == TokenType.NameValuePair && nextToken.IsDelimiter("["))
                    r.Add(new AnalysedJsonLine(AnalyzedJsonLineType.StartPropertyArray, jsonLine));

                else if (curToken.Type == TokenType.NameValuePair && nextToken.IsDelimiter("{"))
                    r.Add(new AnalysedJsonLine(AnalyzedJsonLineType.StartPropertyObject, jsonLine));

                else if (curToken.Type == TokenType.NameValuePair && curToken.__internalTokens.Last().IsNumber)
                    r.Add(new AnalysedJsonLine(AnalyzedJsonLineType.PropertyNumber, jsonLine));

                else if (curToken.Type == TokenType.NameValuePair && curToken.__internalTokens.Last().IsDString)
                {
                    var stringValue = curToken.__internalTokens.Last();
                    var tmpTokens = this.Tokenize(stringValue.Value);
                    if(tmpTokens.Count == 1 && (tmpTokens[0].Type == TokenType.Date|| tmpTokens[0].Type == TokenType.DateTime))
                    {
                        r.Add(new AnalysedJsonLine(AnalyzedJsonLineType.PropertyDate, jsonLine));
                    }
                    else
                    {
                        r.Add(new AnalysedJsonLine(AnalyzedJsonLineType.PropertyString, jsonLine));
                    }
                }
                else if (curToken.Type == TokenType.NameValuePair && curToken.__internalTokens.Last().IsIdentifier("null"))
                    r.Add(new AnalysedJsonLine(AnalyzedJsonLineType.PropertyNull, jsonLine));

                else if (curToken.Type == TokenType.NameValuePair && curToken.__internalTokens.Last().IsIdentifier("true"))
                    r.Add(new AnalysedJsonLine(AnalyzedJsonLineType.PropertyBool, jsonLine));

                else if (curToken.Type == TokenType.NameValuePair && curToken.__internalTokens.Last().IsIdentifier("false"))
                    r.Add(new AnalysedJsonLine(AnalyzedJsonLineType.PropertyBool, jsonLine));
                else
                    r.Add(new AnalysedJsonLine(AnalyzedJsonLineType.Unknown, jsonLine));
            }

            return r;
        }
    }
}

