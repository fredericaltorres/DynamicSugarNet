using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExtendedFormat_UnitTests {

    [TestClass]
    public class TokenizeFormat_UnitTests {

        /// <summary>
        /// Convert a List<string> to a string to help the testing
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        private string str(List<string> l) {

            System.Text.StringBuilder b = new StringBuilder(1024);
            foreach (string s in l)
                b.Append(s);
            return b.ToString();
        }


        [TestMethod]
        public void TokenizeFormat_SepcialExpressionWithABug() {

            string format = "[ID:{ID}, Name:{Name} *****]";
            var tokens    = DynamicSugar.ExtendedFormat.TokenizeFormat(format);

            Assert.AreEqual("[ID:"   ,     tokens[0]     );
            Assert.AreEqual("{ID}"   ,     tokens [1]    );
            Assert.AreEqual(", Name:",     tokens [2]    );
            Assert.AreEqual("{Name}" ,   tokens [3]      );
            Assert.AreEqual(" *****]",  tokens [4]       );
        }

        [TestMethod, ExpectedException(typeof(DynamicSugar.ExtendedFormatException))]
        public void InvalidCurlyBraketSynatx() {

            string format = "{{LastName:{LastName}}";
            var tokens    = DynamicSugar.ExtendedFormat.TokenizeFormat(format);
        }

        [TestMethod]
        public void TextPlusOneProperty() {

            string format = "LastName:{LastName}";
            var tokens    = DynamicSugar.ExtendedFormat.TokenizeFormat(format);

            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual("LastName:", tokens[0]);
            Assert.AreEqual("{LastName}", tokens[1]);
            Assert.AreEqual(format, str(tokens));
        }



        [TestMethod]
        public void TextPlusOneFunction() {

            string format = "LastName:{GetLastName()}";
            var tokens = DynamicSugar.ExtendedFormat.TokenizeFormat(format);

            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual("LastName:", tokens[0]);
            Assert.AreEqual("{GetLastName()}", tokens[1]);
            Assert.AreEqual(format, str(tokens));
        }


        [TestMethod]
        public void EmptyFormat() {

            string format = "";
            var tokens = DynamicSugar.ExtendedFormat.TokenizeFormat(format);
            Assert.AreEqual(0, tokens.Count);
            Assert.AreEqual("", str(tokens));
        }
        [TestMethod]
        public void OneProperty() {

            string format = "{toto}";
            var tokens = DynamicSugar.ExtendedFormat.TokenizeFormat(format);

            Assert.AreEqual(1, tokens.Count);
            Assert.AreEqual(format, tokens[0]);
            Assert.AreEqual(format, str(tokens));
        }
        [TestMethod]
        public void TextPlus2Property() {

            string format = "LastName:{LastName}, FirstName:{FirstName}";
            var tokens    = DynamicSugar.ExtendedFormat.TokenizeFormat(format);

            Assert.AreEqual(4, tokens.Count);
            Assert.AreEqual("LastName:", tokens[0]);
            Assert.AreEqual("{LastName}", tokens[1]);
            Assert.AreEqual(", FirstName:", tokens[2]);
            Assert.AreEqual("{FirstName}", tokens[3]);
            Assert.AreEqual(format, str(tokens));
        }
        [TestMethod, ExpectedException(typeof(DynamicSugar.ExtendedFormatException))]
        public void MissingClosingBraket() {

            string format = "LastName:{";
            var tokens = DynamicSugar.ExtendedFormat.TokenizeFormat(format);
        }
        [TestMethod]
        public void CurlyBraketAsLiteral() {

            string format = "{{LastName}}:{LastName}, {{FirstName}}:{FirstName}";
            var tokens = DynamicSugar.ExtendedFormat.TokenizeFormat(format);

            Assert.AreEqual(11, tokens.Count);

            int i = 0;
            Assert.AreEqual("{{", tokens[i++]);
            Assert.AreEqual("LastName", tokens[i++]);
            Assert.AreEqual("}}", tokens[i++]);
            Assert.AreEqual(":", tokens[i++]);
            Assert.AreEqual("{LastName}", tokens[i++]);
            Assert.AreEqual(", ", tokens[i++]);
            Assert.AreEqual("{{", tokens[i++]);
            Assert.AreEqual("FirstName", tokens[i++]);
            Assert.AreEqual("}}", tokens[i++]);
            Assert.AreEqual(":", tokens[i++]);
            Assert.AreEqual("{FirstName}", tokens[i++]);
        }        
    }
}


