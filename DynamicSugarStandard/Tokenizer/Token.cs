using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DynamicSugar
{
    public partial class Tokenizer
    {
        public class Token {
            
            public string Value { get; set; }
            public TokenType Type { get; set; }
            public string PreSpaces { get; set; }
            public string Name { get; set; } // Only used when the token is a NameValuePair
            public Tokens ArrayValues { get; set; } // nested []
            public Tokens __internalTokens { get; set; } // Original token

            public string GetRawText()
            {
                var sb = new    System.Text.StringBuilder();
                if(__internalTokens != null && __internalTokens.Count > 0)
                {
                    foreach(var token in __internalTokens)
                        sb.Append(token.GetRawText());
                }
                else
                {
                    if(!string.IsNullOrEmpty(PreSpaces))
                        sb.Append(PreSpaces);

                    sb.Append(ValueAsString);
                }
                return sb.ToString();
            }

            public void Assert(TokenType type, string value, string name = null, string preSpaces = null)
            {
                if(Type != type ||  
                   (value != null && !IsEqualValue(value, true)) || 
                   (name != null && Name != name) ||
                   (preSpaces != null && PreSpaces != preSpaces))
                {
                    throw new InvalidEnumArgumentException($"Token assertion failed: Expected Type={type}, Value={value}, Name={name}, PreSpaces='{PreSpaces}', but got Type={Type}, Value={Value}, Name={Name}, PreSpaces='{preSpaces}'.");
                }
            }

            public void AssertNameValue(string value, string name, string valueAsString)
            {
                if (Type != TokenType.NameValuePair ||
                  (/*value != null &&*/ !IsEqualValue(value, true)) ||
                  (name != null && Name != name) ||
                  (this.__internalTokens.Count != 3) || (this.ValueAsString != valueAsString)
                )
                {
                    throw new InvalidEnumArgumentException($"Token NameValue assertion failed: Expected Type={TokenType.NameValuePair}, Value={value}, Name={name}, but got Type={Type}, Value={Value}, Name={Name}.");
                }
            }

            public string ValueAsString
            {
                get
                {
                    if (Type == TokenType.StringLiteralDQuote)
                        return $@"""{this.Value}""";
                    if (Type == TokenType.StringLiteralSQuote)
                        return $@"'{this.Value}'";

                    if (Type == TokenType.NameValuePair)
                    {
                        if (this.__internalTokens[2].IsUndefined)
                            return $@"{this.__internalTokens[0].ValueAsString} {this.__internalTokens[1].ValueAsString}";
                        else
                            return $@"{this.__internalTokens[0].ValueAsString} {this.__internalTokens[1].ValueAsString} {this.__internalTokens[2].ValueAsString}";
                    }
                        

                    return this.Value;
                }
            }

            public Token Clone()
            {
                return new Token(this.Value, this.Type, this.PreSpaces)
                {
                    Name = this.Name,
                    ArrayValues = this.ArrayValues?.Clone()
                };
            }

            public Token(Tokens tokens)
            {
                ArrayValues = tokens;
                Type = TokenType.ArrayOfTokens;
            }

            public Token(Token tokenName, Token tokenDelimiter, Token tokenValue)
            {
                this.Name = tokenName.Value;
                this.Value = tokenValue.Value;
                this.Type = TokenType.NameValuePair;
                this.__internalTokens = new Tokens { tokenName, tokenDelimiter, tokenValue };
                PreSpaces = "";
            }

            public Token(string value, TokenType type) : this(value , type, "")
            {
            }

            public Token(string value, TokenType type, string preSpaces)
            {
                Value = value;
                Type = type;
                PreSpaces = preSpaces;
            }

            public Token(string value, TokenType type, string preSpaces, Tokens internalTokens) : this(value , type, preSpaces)
            {
                __internalTokens = internalTokens;
            }

            public static Token GetUndefinedToken()
            {
                return new Token(null, TokenType.UndefinedToken, null);
            }

            public bool IsEqualValue(string value, bool ignoreCase) 
            {
                if(ignoreCase && value != null)
                    return string.Equals(this.Value, value, StringComparison.OrdinalIgnoreCase);
                return this.Value == value; 
            }

            public char GetValueCharIndex(int index)
            {
                return index < Value.Length ? this.Value[index] : '\0';
            }

            public bool IsIdentifierOrIdentifierPath => Type == TokenType.Identifier || Type == TokenType.IdentifierPath;
            public bool IsIdentifier(string value = null, bool ignoreCase = true) => value == null ? this.IsIdentifierOrIdentifierPath : this.IsIdentifierOrIdentifierPath && IsEqualValue(value, ignoreCase);
            public bool IsString => Type == TokenType.StringLiteralDQuote || Type == TokenType.StringLiteralSQuote;
            public bool IsDString => Type == TokenType.StringLiteralDQuote;
            public bool IsSString => Type == TokenType.StringLiteralSQuote;
            public bool IsNumber => Type == TokenType.Number;
            public bool IsUndefined => Type == TokenType.UndefinedToken;
            public bool IsInteger => Type == TokenType.Number && !Value.Contains(".");
            public bool IsFloat => Type == TokenType.Number && Value.Contains(".");
            public bool IsAnyValue => !(Type == TokenType.UndefinedToken || Type == TokenType.ArrayOfTokens || Type == TokenType.NameValuePair);
            public bool IsDelimiter(string value = null) => value == null ? (Type == TokenType.Delimiter) : (Type == TokenType.Delimiter && this.Value == value);
            public bool IsDelimiter(List<string> values ) =>  (Type == TokenType.Delimiter && values.Contains(this.Value));

            public override string ToString()
            {
                return ToStringWithType();
            }

            public string ToString(bool addType)
            {
                if (addType)
                    return ToStringWithType();
                return ToStringWithNoType();
            }

            public string ToStringWithType()
            {
                if (this.Type == TokenType.ArrayOfTokens)
                {
                    var arrayValuesString = string.Join(", ", ArrayValues);
                    return $"{this.Type} [{arrayValuesString}]";
                }
                else if (this.Type == TokenType.NameValuePair)
                {
                    return $"{this.Type} {this.Name}: {this.Value}";
                }

                return $"{this.Type} {this.Value}";
            }

            public string ToStringWithNoType()
            {
                if (this.Type == TokenType.ArrayOfTokens)
                {
                    var arrayValuesString = string.Join(", ", ArrayValues);
                    return $"[{arrayValuesString}]";
                }
                else if (this.Type == TokenType.NameValuePair)
                {
                    return $"{this.Name}: {this.Value}";
                }

                return $"{this.Value}";
            }

            public bool IsNameValue(string name, string value)  => 
                Type == TokenType.NameValuePair && 
                Name == name && 
                Value == value;

            internal bool Is(Token token)
            {
                return this.Type == token.Type && 
                       this.Value == token.Value && 
                       this.Name == token.Name &&
                       (this.ArrayValues == null && token.ArrayValues == null || 
                                              this.ArrayValues?.Count == token.ArrayValues?.Count);
            }
        }
    }
}
