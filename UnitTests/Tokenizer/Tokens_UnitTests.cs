using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Reflection;

namespace DynamicSugarSharp_UnitTests {

    [TestClass]
    public class Tokens_UnitTests
    {
        [TestMethod]
        public void Tokens_RemoveDelimiters()
        {
            var tokens = new Tokenizer.Tokens();
            tokens.Add(new Tokenizer.Token("1", Tokenizer.TokenType.Number));
            tokens.Add(new Tokenizer.Token(",", Tokenizer.TokenType.Delimiter));
            tokens.Add(new Tokenizer.Token("1", Tokenizer.TokenType.Number));
            tokens.Add(new Tokenizer.Token(",", Tokenizer.TokenType.Delimiter));

            var clonedTokens = tokens.RemoveDelimiters();
            Assert.AreEqual(tokens.Count-2, clonedTokens.Count);
        }

        [TestMethod]
        public void Tokens_Clone()
        {
            var tokens = new Tokenizer.Tokens();
            tokens.Add(new Tokenizer.Token("1", Tokenizer.TokenType.Number));
            tokens.Add(new Tokenizer.Token(",", Tokenizer.TokenType.Identifier));

            var clonedTokens = tokens.Clone();
            Assert.AreEqual(tokens.Count, clonedTokens.Count);

            Assert.AreEqual(tokens[0].Value, clonedTokens[0].Value);
            Assert.AreEqual(tokens[0].Type, clonedTokens[0].Type);
            Assert.AreEqual(tokens[1].Value, clonedTokens[1].Value);
            Assert.AreEqual(tokens[1].Type, clonedTokens[1].Type);
        }
    }
}
