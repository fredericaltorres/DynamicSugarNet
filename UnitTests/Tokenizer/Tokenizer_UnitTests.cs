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
        const string TestLogString1     = @"2025-05-24 A[B=2] mode: execute";
        const string DateTime1     = @"2025-05-24 13:16:52";
        const string DateTime2    =  @"2025/05/24 13-16-52";
        const string DateTimePlusIdentofiers   = @"2025/05/24 13-16-52,Info,Export";
        const string DateTimeWithMS     = @"2025-05-24 13:16:52.123";
        const string TestDateTimeTZ= @"2025-05-26T22:06:11.513Z";

        const string TestLongLogLine = @"2025-05-24 13:16:52.859,Info,Export,[id: 709046703, mode: Export][ExecuteConversion()]Slide: 10755223, type: IMAGE, index: 0001";

        [TestMethod]
        public void TokenizerTest_DateTime()
        {
            var tokenizer = new Tokenizer();
            var tokens = tokenizer.Tokenize(DateTime1);
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.DateTimeToken, tokens[x].Type);
            Assert.AreEqual(DateTime1, tokens[x++].Value);
        }

        [TestMethod]
        public void TokenizerTest_TestDateTimeTZ()
        {
            var tokenizer = new Tokenizer();
            var tokens = tokenizer.Tokenize(TestDateTimeTZ);
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.DateTimeToken, tokens[x].Type);
            Assert.AreEqual(TestDateTimeTZ, tokens[x++].Value);
        }

        [TestMethod]
        public void TokenizerTest_DateTime_22()
        {
            var tokenizer = new Tokenizer();
            var tokens = tokenizer.Tokenize(DateTime2);
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.DateTimeToken, tokens[x].Type);
            Assert.AreEqual(DateTime2, tokens[x++].Value);
        }

        [TestMethod]
        public void TokenizerTest_DateTime_WithSlashSeparator_AndMoreIdentifierAfter()
        {
            var tokenizer = new Tokenizer();
            var tokens = tokenizer.Tokenize(DateTimePlusIdentofiers).RemoveDelimiters();
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.DateTimeToken, tokens[x].Type);
            Assert.AreEqual(DateTime2, tokens[x++].Value);

            Assert.AreEqual(Tokenizer.TokenType.Identifier, tokens[x].Type);
            Assert.AreEqual("Info", tokens[x++].Value);

            Assert.AreEqual(Tokenizer.TokenType.Identifier, tokens[x].Type);
            Assert.AreEqual("Export", tokens[x++].Value);
        }

        [TestMethod]
        public void TokenizerTest_DateTimeWithMilliSecond()
        {
            var tokenizer = new Tokenizer();
            var tokens = tokenizer.Tokenize(DateTimeWithMS);
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.DateTimeToken, tokens[x].Type);
            Assert.AreEqual(DateTimeWithMS, tokens[x++].Value);
        }

        [TestMethod]
        public void Tokenizer_LogString1()
        {
            var tokenizer = new Tokenizer();
            var tokens = tokenizer.Tokenize(TestLogString1);

            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.DateToken, tokens[x].Type);
            Assert.AreEqual("2025-05-24", tokens[x++].Value);

            Assert.AreEqual(Tokenizer.TokenType.Identifier, tokens[x].Type);
            Assert.AreEqual("A", tokens[x++].Value);

            // [B=2]
            Assert.AreEqual(Tokenizer.TokenType.ArrayOfTokens, tokens[x].Type);
            Assert.AreEqual(1, tokens[x].ArrayValues.Count);
            Assert.AreEqual(Tokenizer.TokenType.NameValuePair, tokens[x].ArrayValues[0].Type);
            Assert.AreEqual("B", tokens[x].ArrayValues[0].Name);
            Assert.AreEqual("2", tokens[x].ArrayValues[0].Value);
            x++;

            Assert.AreEqual(Tokenizer.TokenType.NameValuePair, tokens[x].Type);
            Assert.AreEqual("mode", tokens[x].Name);
            Assert.AreEqual("execute", tokens[x++].Value);
        }

        // const string TestLogString4 = @"2025-05-24 13:16:52.859,Info,Export,[id: 709046703, mode: Export][ExecuteConversion()]Slide: 10755223, type: IMAGE, index: 0001";

        [TestMethod]
        public void Tokenizer_LogString_LongComplexLine()
        {
            var tokenizer = new Tokenizer();
            var tokens = tokenizer.Tokenize(TestLongLogLine).RemoveDelimiters();

            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.DateTimeToken, tokens[x].Type);
            Assert.AreEqual("2025-05-24 13:16:52.859", tokens[x++].Value);

            Assert.AreEqual(Tokenizer.TokenType.Identifier, tokens[x].Type);
            Assert.AreEqual("Info", tokens[x++].Value);

            Assert.AreEqual(Tokenizer.TokenType.Identifier, tokens[x].Type);
            Assert.AreEqual("Export", tokens[x++].Value);

            // [id: 709046703, mode: Export]
            Assert.AreEqual(Tokenizer.TokenType.ArrayOfTokens, tokens[x].Type);
            Assert.AreEqual(2, tokens[x].ArrayValues.Count);
            Assert.AreEqual(Tokenizer.TokenType.NameValuePair, tokens[x].ArrayValues[0].Type);
            Assert.AreEqual("id", tokens[x].ArrayValues[0].Name);
            Assert.AreEqual("709046703", tokens[x].ArrayValues[0].Value);

            Assert.AreEqual(Tokenizer.TokenType.NameValuePair, tokens[x].ArrayValues[1].Type);
            Assert.AreEqual("mode", tokens[x].ArrayValues[1].Name);
            Assert.AreEqual("Export", tokens[x].ArrayValues[1].Value);
            x++;

            //[ExecuteConversion()]
            Assert.AreEqual(Tokenizer.TokenType.ArrayOfTokens, tokens[x].Type);
            Assert.AreEqual(Tokenizer.TokenType.Identifier, tokens[x].ArrayValues[0].Type);
            Assert.AreEqual("ExecuteConversion", tokens[x].ArrayValues[0].Value);
            x++;

            // Slide: 10755223, type: IMAGE, index: 0001
            Assert.AreEqual(Tokenizer.TokenType.NameValuePair, tokens[x].Type);
            Assert.IsTrue(tokens[x].IsNameValue("Slide", "10755223"));
            x++;

            Assert.AreEqual(Tokenizer.TokenType.NameValuePair, tokens[x].Type);
            Assert.IsTrue(tokens[x].IsNameValue("type", "IMAGE"));
            x++;

            Assert.AreEqual(Tokenizer.TokenType.NameValuePair, tokens[x].Type);
            Assert.IsTrue(tokens[x].IsNameValue("index", "0001"));
            x++;
        }
    }
}
