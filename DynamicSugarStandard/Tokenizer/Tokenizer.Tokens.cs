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
                return c;
            }

            public Tokens Clone()
            {
                var clonedTokens = new Tokens();
                foreach (var token in this)
                    clonedTokens.Add(token.Clone());

                return clonedTokens;
            }
        }
    }
}
