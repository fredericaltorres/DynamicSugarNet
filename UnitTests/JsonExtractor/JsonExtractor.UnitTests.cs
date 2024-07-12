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
        public void ExtractOneInvalidObject_BadCurlyBracketPosition ()
        {
            var Json1 = @"}   ""a""   :   1 {";
            var text = $"noiseStart {Json1} noiseEnd";
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void ExtractOneInvalidObject_JsonSyntaxError()
        {
            var Json1 = @"{   ""a""      1 }";
            var text = $"noiseStart {Json1} noiseEnd";
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(null, result);
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


        [TestMethod]
        public void ExtractOneArrayOfObject()
        {
            var Json1 = @"[{ ""a"":1 }, { ""b"":1 }]";
            var expectedJson1 = @"[
  {
    ""a"": 1
  },
  {
    ""b"": 1
  }
]";
            var text = $"noiseStart {Json1} noiseEnd";
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }


        [TestMethod]
        public void ExtractInvalidJson()
        {
            var Json1 = @"[1,2,3";
            var text = $"noiseStart {Json1} noiseEnd";
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void ExtractOneInvalidArrayOfInt()
        {
            var Json1 = @"]1,2,3[";
            var text = $"noiseStart {Json1} noiseEnd";
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void ExtractOneObjectWithTimeStampInSquareBraket()
        {
            var Json1 = @"[2021-12-10T00:00:20.257Z]  {""IsSuccessStatusCode"":true } ";
            var expectedJson1 = @"{
  ""IsSuccessStatusCode"": true
}";
            var text = Json1;
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }



        [TestMethod]
        public void ExtractOneObjectWithTimeStampInCurlyBraket()
        {
            var Json1 = @"{2021-12-10T00:00:20.257Z}  {""IsSuccessStatusCode"":true } ";
            var expectedJson1 = @"{
  ""IsSuccessStatusCode"": true
}";
            var text = Json1;
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }


        [TestMethod]
        public void Extract_DateInBracket_JsonBracketContainingOneObject()
        {
            var Json1 = @"[2024-07-12T10:23:50.403Z]  JSON Message [{""JobId"":14669442}]";
            var expectedJson1 = @"[
  {
    ""JobId"": 14669442
  }
]";
            var text = Json1;
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }


        [TestMethod]
        public void Extract_DateInBracket_JsonBracketContainingTwoObject()
        {
            var Json1 = @"[2024-07-12T10:23:50.403Z]  JSON Message [{""JobId"":1111}, {""JobId"":2222}]";
            var expectedJson1 = @"[
  {
    ""JobId"": 1111
  },
  {
    ""JobId"": 2222
  }
]";
            var text = Json1;
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }

        [TestMethod]
        public void Extract_DateInBracket_JsonObjectContainingArray()
        {
            var Json1 = @"[2024-07-12T10:23:40.325Z] | [HTTPCallStatus] Content:{""VideoId"":0, ""Timings"":[], ""mimeType"":null }";
            var expectedJson1 = @"{
  ""VideoId"": 0,
  ""Timings"": [],
  ""mimeType"": null
}";
            var text = Json1;
            var result = JsonExtractor.Extract(text);
            Assert.AreEqual(expectedJson1, result);
        }
    }
}