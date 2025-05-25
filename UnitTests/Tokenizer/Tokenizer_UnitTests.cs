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
    public class Tokenizer_UnitTests
    {
        const string TestLogString1 = @"2025-05-24 A[B=2] mode: execute";
        const string TestLogString2 = @"2025-05-24 13:16:52.859,Info,Export,[id: 709046703, mode: Export][ExecuteConversion()]Slide 10755223, type: IMAGE, index: 0001";

        [TestMethod]
        public void TokenizerTest1()
        {
            var tokenizer = new Tokenizer();
            var tokens = tokenizer.Tokenize(TestLogString1);

            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.DateToken, tokens[x].Type);
            Assert.AreEqual("2025-05-24", tokens[x++].Value);

            Assert.AreEqual(Tokenizer.TokenType.Identifier, tokens[x].Type);
            Assert.AreEqual("A", tokens[x++].Value);

            Assert.AreEqual(Tokenizer.TokenType.ArrayOfTokens, tokens[x].Type);
            Assert.AreEqual(null, tokens[x++].Value); // TO DO

            Assert.AreEqual(Tokenizer.TokenType.NameValuePair, tokens[x].Type);
            Assert.AreEqual("mode", tokens[x].Value);
            Assert.AreEqual("execute", tokens[x++].Value);
        }
    }
}
