using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DynamicSugar
{
    public partial class Tokenizer
    {
        public class Tokens : List<Token>
        {
            public Tokens() : base() { }
            public Tokens(IEnumerable<Token> tokens) : base(tokens) { }

            public Tokens RemoveDelimiters()
            {
                var c = Clone();
                c.RemoveAll(token => token.IsDelimiter());

                return c;
            }

            public Tokens Clone()
            {
                var clonedTokens = new Tokens();
                foreach (var token in this)
                    clonedTokens.Add(token.Clone());

                return clonedTokens;
            }

            public string GetRawText()
            {
                var sb = new System.Text.StringBuilder();
                foreach (var token in this)
                    sb.Append(token.GetRawText());
                return sb.ToString();
            }

            public string GetAsText(string sepa = "")
            {
                var sb = new System.Text.StringBuilder();
                foreach (var token in this)
                {
                    sb.Append(token.Value);
                    sb.Append(sepa);
                }
                return sb.ToString();
            }

            public string GetTokenScript(bool addType = true)
            {
                var sb = new System.Text.StringBuilder();
                foreach (var token in this)
                {
                    sb.Append(token.ToString(addType));
                    sb.Append("; ");
                }
                return sb.ToString();
            }

            public  Dictionary<string, string> GetVariables()
            {
                var variables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var token in this)
                {
                    if (token.Type == TokenType.NameValuePair)
                    {
                        if (!variables.ContainsKey(token.Name))
                            variables.Add(token.Name, token.Value);
                    }
                }

                return variables;
            }
            
            public string GetVariableValue(string name, bool ignoreCase = true)
            {
                foreach (var token in this)
                {
                    if(token.Type == TokenType.NameValuePair && string.Equals(token.Name, name, StringComparison.OrdinalIgnoreCase))
                        return token.Value;
                }

                return null;
            }

            public bool IdentifierExists(List<string> names, bool ignoreCase = true)
            {
                foreach (var name in names)
                {
                    if (!IdentifierExists(name, ignoreCase))
                        return false;
                }
                return true;
            }

            public bool IdentifierExists(string name, bool ignoreCase = true)
            {
                foreach (var token in this)
                        if (token.IsIdentifier(name, ignoreCase))
                            return true;

                return false;
            }
        }
    }
}
