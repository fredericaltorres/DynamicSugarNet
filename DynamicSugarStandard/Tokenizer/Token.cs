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
            public Tokens ArrayValues { get; set; }
            public Tokens NameValues { get; set; }


            public void Assert(TokenType type, string value, string name = null)
            {
                if(Type != type ||  
                   (value != null && !IsEqualValue(value, true)) || 
                   (name != null && Name != name))
                {
                    throw new InvalidEnumArgumentException($"Token assertion failed: Expected Type={type}, Value={value}, Name={name}, but got Type={Type}, Value={Value}, Name={Name}.");
                }
            }

            public void AssertNameValue(string value, string name, string valueAsString)
            {
                if (Type != TokenType.NameValuePair ||
                  (value != null && !IsEqualValue(value, true)) ||
                  (name != null && Name != name) ||
                  (this.NameValues.Count != 3) || (this.ValueAsString != valueAsString)
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
                        return $@"{this.NameValues[0].ValueAsString} {this.NameValues[1].ValueAsString} {this.NameValues[2].ValueAsString}";

                    return this.Value;
                }
            }

            public Token Clone ()
            {
                return new Token(this.Value, this.Type)
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
                this.NameValues = new Tokens { tokenName, tokenDelimiter, tokenValue };
            }

            public Token(string value, TokenType type)
            {
                Value = value;
                Type = type;
            }

            public static Token GetUndefinedToken()
            {
                return new Token(null, TokenType.UndefinedToken);
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

            public bool IsIdentifier(string value = null, bool ignoreCase = true) => value == null ? Type == TokenType.Identifier : 
                                                                           Type == TokenType.Identifier && IsEqualValue(value, ignoreCase);

            public bool IsString => Type == TokenType.StringLiteralDQuote || Type == TokenType.StringLiteralSQuote;
            public bool IsDString => Type == TokenType.StringLiteralDQuote;
            public bool IsSString => Type == TokenType.StringLiteralSQuote;

            public bool IsNumber => Type == TokenType.Number;
            public bool IsInteger => Type == TokenType.Number && !Value.Contains(".");
            public bool IsFloat => Type == TokenType.Number && Value.Contains(".");

            public bool IsDelimiter(string value = null) => value == null ? (Type == TokenType.Delimiter) : (Type == TokenType.Delimiter && this.Value == value);

            public bool IsDelimiter(List<string> values ) =>  (Type == TokenType.Delimiter && values.Contains(this.Value));
            public bool IsAnyValue => !(Type == TokenType.UndefinedToken || Type == TokenType.ArrayOfTokens || Type == TokenType.NameValuePair);

            public string Name { get; set; } // Only used when the token is a NameValuePair

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
