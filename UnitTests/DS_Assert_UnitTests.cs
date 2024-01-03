using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DynamicSugarSharp_UnitTests {

    //TODO:Try extension method to List<T>

    [TestClass]
    public class DS_Assert_GetTextResource {

        [TestMethod]
        public void AreEqualProperties_Poco() {

            var o = new { a=1, b=2, c="ok", d=true, e = DateTime.Now, f=1.2, g=1.2M, h=1.2f };
            DS.Assert.AreEqualProperties(o, o);
        }

        [TestMethod, ExpectedException(typeof(DynamicSugar.AssertFailedException))]
        public void AreEqualProperties_Poco_Fail() {

            var o1 = new { a=1, b=2, c="ok", d=true, e = DateTime.Now, f=1.2, g=1.2M, h=1.2f };
            var o2 = new { a=1, b=2, c="ok", d=true, e = DateTime.Now, f=1.2, g=1.2M, h=1.1f };
            DS.Assert.AreEqualProperties(o1, o2);
        }

        [TestMethod]
        public void AreEqualProperties_Dictionary() {

            var o = new { a=1, b=2, c="ok", d=true, e = DateTime.Now, f=1.2, g=1.2M, h=1.2f };
            DS.Assert.AreEqualProperties(DS.Dictionary(o), DS.Dictionary(o));
        }

        [TestMethod, ExpectedException(typeof(DynamicSugar.AssertFailedException))]
        public void AreEqualProperties_Dictionary_Fail() {

            var o1 = new { a=1, b=2, c="ok", d=true, e = DateTime.Now, f=1.2, g=1.2M, h=1.2f };
            var o2 = new { a=2, b=2, c="ok", d=true, e = DateTime.Now, f=1.2, g=1.2M, h=1.1f };
            DS.Assert.AreEqualProperties(DS.Dictionary(o1), DS.Dictionary(o2));
        }

        [TestMethod]
        public void Words_Positive()
        {
            DS.Assert.Words("aa bb", "aa & bb");
            DS.Assert.Words("aa bb", "(aa & bb)");
            DS.Assert.Words("aa bb", "aa & (bb)");

            // Operator & is optional
            DS.Assert.Words("aa bb", "aa  bb");
            DS.Assert.Words("aa bb", "(aa  bb)");
            DS.Assert.Words("aa bb", "aa (bb)");

        }
        [TestMethod, ExpectedException(typeof(DynamicSugar.AssertFailedException))]
        public void Words_Negative()
        {
            DS.Assert.Words("aa bb", "cc");
        }
        [TestMethod, ExpectedException(typeof(DynamicSugar.AssertFailedException))]
        public void Words_Negative_SubAndExpression()
        {
            DS.Assert.Words("aa bb", "(aa & bb && cc)");
        }

        [TestMethod, ExpectedException(typeof(DynamicSugar.AssertFailedException))]
        public void Words_Negative_NestedSubAndExpression()
        {
            DS.Assert.Words("aa bb", "aa & (bb & (cc)) ");
        }


        [TestMethod, ExpectedException(typeof(DynamicSugar.AssertFailedException))]
        public void Words_Negative_TripleSubAndExpression()
        {
            DS.Assert.Words("aa bb", "aa & (bb & (aa & (a & z)))");
        }

        [TestMethod]
        public void Words_DoubleNestedSubAndExpression()
        {
            DS.Assert.Words("aa bb", "aa & (bb & (aa))");
            DS.Assert.Words("aa bb", "aa & (bb & (aa & bb))");
            DS.Assert.Words("aa bb", "aa & (bb & (aa & (a & b)))");
        }

        [TestMethod]
        public void Words_OIrExpression()
        {
            DS.Assert.Words("aa bb", "aa | bb");
            DS.Assert.Words("aa bb", "(aa | bb)");
            DS.Assert.Words("aa bb", "((aa | bb))");
            DS.Assert.Words("aa bb", "(aa | bb) & (bb | aa)");
            DS.Assert.Words("aa bb", "((aa | cc))");
        }

        [TestMethod, ExpectedException(typeof(DynamicSugar.AssertFailedException))]
        public void Words_DoubleParenthesisAndAndSubAndExpression()
        {
            DS.Assert.Words("aa bb", "((aa & cc))");
        }

        [TestMethod, ExpectedException(typeof(DynamicSugar.AssertFailedException))]
        public void Words_Negative_RegExExpression()
        {
            DS.Assert.Words("aa bb", "regex z.");
        }

        [TestMethod]
        public void Words_RegExExpression()
        {
            // RegEx limitation due to the tokenizer charatect ()&| and ' ' cannot be part of the regex
            DS.Assert.Words("aa bb", "regex a. & regex b.");
            DS.Assert.Words("aa bb", "(regex a. & regex b.) & (regex b. & regex a.)");
        }
    }
}
