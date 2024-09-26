using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Net.Mime;

namespace DynamicSugarSharp_UnitTests{

    [TestClass]
    public class StringFormat_UnitTests {

        [TestMethod]
        public void string_format() {
                        
            var s = String.Format("[{0}] Age={1:000}", "TORRES", 45);
            Assert.AreEqual("[TORRES] Age=045",s);

            s = "[{0}] Age={1:000}".FormatString("TORRES", 45);
            Assert.AreEqual("[TORRES] Age=045",s);
        }

        [TestMethod]
        public void String_Format()
        {
            var s = String.Format("[{0}] Age={1:000}", "TORRES", 45);
            Assert.AreEqual("[TORRES] Age=045", s);

            string a = "[{0}] Age={1:000}";
            s = a.FormatString("TORRES", 45);
            Assert.AreEqual("[TORRES] Age=045", s);
        }
        
        [TestMethod]
        public void String_Format__WithDictionary() {
                        
            var dic = new Dictionary<string,object>() {
                { "LastName" , "TORRES" },
                { "Age"      , 45       }
            };
            var s = "[{LastName}] Age={Age:000}".Template(dic);
            Assert.AreEqual("[TORRES] Age=045",s);
        }
        [TestMethod]
        public void String_Format__WithAnonymousType() {

            var s = "[{LastName}] Age={Age:000}".Template(new { LastName = "TORRES", Age = 45 });
            Assert.AreEqual("[TORRES] Age=045", s);
        }
        [TestMethod,ExpectedException(typeof(ExtendedFormatException))]
        public void String_Format__WithAnonymousType_WithTypoInFormat() {

            var s = "[{LastName_BAD}] Age={Age:000}".Template(new { LastName = "TORRES", Age = 45 });
            Assert.AreEqual("[TORRES] Age=045", s);
        }
        [TestMethod]
        public void String_Format__WithExpandoObject() {
                 
            dynamic eo  = new ExpandoObject();
            eo.LastName = "TORRES";
            eo.Age      = 45;
            var s = "[{LastName}] Age={Age:000}".Template(eo as ExpandoObject);
            Assert.AreEqual("[TORRES] Age=045",s);
        }

        [TestMethod]
        public void RemoveComment__C_Comment_SlashStar()
        {
            var result = $"[/* comment */]".RemoveComment(commentType: ExtensionMethods_Format.StringComment.C);
            var expected = $"[]";
            Assert.AreEqual(expected, result);


            result = @"[Hello/* comment 
*/]".RemoveComment(commentType: ExtensionMethods_Format.StringComment.C);
            expected = @"[Hello
]";
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void RemoveComments__C_Comment_SlashStar()
        {
            var result = $"[/* comment */]".RemoveComments(commentType: ExtensionMethods_Format.StringComment.C);
            var expected = $"[]";
            Assert.AreEqual(expected, result);


            result = @"[Hello/* comment 
*/]".RemoveComments(commentType: ExtensionMethods_Format.StringComment.C);
            expected = @"[Hello
]";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RemoveComment__Pyhton_Comment()
        {
            var result = @"print(""Hello World"") # a comment".RemoveComment(commentType: ExtensionMethods_Format.StringComment.Python);
            var expected = @"print(""Hello World"") ";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RemoveComment__SQL_Comment()
        {
            var result = @"print(""Hello World"") -- a comment".RemoveComment(commentType: ExtensionMethods_Format.StringComment.SQL);
            var expected = @"print(""Hello World"") ";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RemoveComment__SQL_Comment_CornerCaseWithOneExtraDash()
        {
            var result = @"print(""Hello World"") --- a comment".RemoveComment(commentType: ExtensionMethods_Format.StringComment.SQL);
            var expected = @"print(""Hello World"") ";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RemoveComment__SQL_Comment_CornerCaseWithTwoExtraDash()
        {
            var result = @"print(""Hello World"") ---- a comment".RemoveComment(commentType: ExtensionMethods_Format.StringComment.SQL);
            var expected = @"print(""Hello World"") ";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RemoveComment__CPP_Comment()
        {
            var result = @"print(""Hello World"") // a comment".RemoveComment(commentType: ExtensionMethods_Format.StringComment.CPP);
            var expected = @"print(""Hello World"") ";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RemoveComments__CPP_Comment()
        {
            var text = @"
print(""Hello World"") // a comment
var a = 1; // a comment
";

            var expected = @"print(""Hello World"") 
var a = 1; ";

            var result = text.RemoveComments(commentType: ExtensionMethods_Format.StringComment.CPP);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void String_TemplateWithDifferentMacro()
        {
            var dic = new Dictionary<string, object>() {
                { "LastName" , "TORRES" },
                { "Age"      , 45       }
            };
            var s = "[LastName]:[Age]".Template(dic, "[", "]");
            Assert.AreEqual("TORRES:45", s);
        }
    }
}
