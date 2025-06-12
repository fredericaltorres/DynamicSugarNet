using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DynamicSugar
{
    public partial class Tokenizer
    {
        public class Tokens : List<Token>
        {
            public Tokens RemoveDelimiters()
            {
                var c = Clone();
                c.RemoveAll(token => token.IsDelimiter());

                foreach (var token in c)
                    if (token.Type == TokenType.ArrayOfTokens)
                        token.ArrayValues = token.ArrayValues.RemoveDelimiters();

                return c;
            }

            public Tokens Clone()
            {
                var clonedTokens = new Tokens();
                foreach (var token in this)
                    clonedTokens.Add(token.Clone());

                return clonedTokens;
            }


            public string GetTokenScript(bool addType = true)
            {
                var sb = new System.Text.StringBuilder();
                foreach (var token in this)
                {
                    if (token.Type == TokenType.ArrayOfTokens)
                    {
                        sb.Append("List [");
                        sb.Append(token.ArrayValues.GetTokenScript(addType));
                        sb.Append("]");
                    }
                    else
                    {
                        sb.Append(token.ToString(addType));
                    }
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
                    else if (token.Type == TokenType.ArrayOfTokens)
                    {
                        var arrayVariables = token.ArrayValues.GetVariables();
                        foreach (var kvp in arrayVariables)
                        {
                            if (!variables.ContainsKey(kvp.Key))
                                variables.Add(kvp.Key, kvp.Value);
                        }
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

                    if (token.Type == TokenType.ArrayOfTokens)
                    {
                        var r = token.ArrayValues.GetVariableValue(name, ignoreCase);
                        if(r != null)
                            return r;
                    }
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
                {
                    if (token.Type == TokenType.ArrayOfTokens)
                    {
                        var r = token.ArrayValues.IdentifierExists(name, ignoreCase);
                        if (r)
                            return true;
                    }
                    else
                    {
                        if (token.IsIdentifier(name, ignoreCase))
                            return true;
                    }
                }

                return false;
            }
        }
    }
}
