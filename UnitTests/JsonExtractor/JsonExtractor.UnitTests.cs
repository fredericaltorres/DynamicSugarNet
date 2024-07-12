using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Reflection;
using System.Text.RegularExpressions;
using DynamicSugar;

namespace DynamicSugarSharp_UnitTests
{
    [TestClass]
    public class JsonExtractorTests
    {
        [TestMethod]
        public void ExtractOneObjectWithTextBeforeAndAfter()
        {
            var Json1 = @"TATA { ""a"": 1 } TUTU";
            var expectedJson1 = "{\r\n  \"a\": 1\r\n}";
            var text = $"{Json1}";
            var result =  JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }

        [TestMethod]
        public void ExtractOneObject()
        {
            var Json1 = @"{   ""a""   :   1 }";
            var expectedJson1 = "{\r\n  \"a\": 1\r\n}";
            var text = $"noiseStart {Json1} noiseEnd";
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }

        [TestMethod]
        public void ExtractOneArray()
        {
            var Json1 = @"[1, 2, 3]";
            var expectedJson1 = "[\r\n  1,\r\n  2,\r\n  3\r\n]";
            var text = $"noiseStart {Json1} noiseEnd";
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }
    }
}