using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DynamicSugarSharp_UnitTests
{

    [TestClass]
    public class DS_Assert_GetTextResource
    {
        [TestMethod]
        public void AreEqualProperties_Poco()
        {
            var o = new { a = 1, b = 2, c = "ok", d = true, e = DateTime.Now, f = 1.2, g = 1.2M, h = 1.2f };
            DS.Assert.AreEqualProperties(o, o);
        }

        [TestMethod]
        public void AreEqualProperties_Poco_RegEx()
        {
            var o1 = new { a = 1, Letters = "ABC123foo" };
            var o2 = new { a = 1, Letters = new Regex("[A-Z]+[0-9]+foo") };
            DS.Assert.AreEqualProperties(o1, o2);
        }

        [TestMethod, ExpectedException(typeof(DynamicSugar.AssertFailedException))]
        public void AreEqualProperties_Poco_RegEx_Fail()
        {
            var o1 = new { a = 1, Letters = "ABC123foo" };
            var o2 = new { a = 1, Letters = new Regex("[A-Z]+foo") };
            DS.Assert.AreEqualProperties(o1, o2);
        }

        [TestMethod, ExpectedException(typeof(DynamicSugar.AssertFailedException))]
        public void AreEqualProperties_Poco_Fail()
        {
            var o1 = new { a = 1, b = 2, c = "ok", d = true, e = DateTime.Now, f = 1.2, g = 1.2M, h = 1.2f};
            var o2 = new { a = 1, b = 2, c = "ok", d = true, e = DateTime.Now, f = 1.2, g = 1.2M, h = 1.1f };
            DS.Assert.AreEqualProperties(o1, o2);
        }

        [TestMethod]
        public void AreEqualProperties_Dictionary()
        {
            var o = new { a = 1, b = 2, c = "ok", d = true, e = DateTime.Now, f = 1.2, g = 1.2M, h = 1.2f };
            DS.Assert.AreEqualProperties(DS.Dictionary(o), DS.Dictionary(o));
        }

        [TestMethod, ExpectedException(typeof(DynamicSugar.AssertFailedException))]
        public void AreEqualProperties_Dictionary_Fail()
        {
            var o1 = new { a = 1, b = 2, c = "ok", d = true, e = DateTime.Now, f = 1.2, g = 1.2M, h = 1.2f };
            var o2 = new { a = 2, b = 2, c = "ok", d = true, e = DateTime.Now, f = 1.2, g = 1.2M, h = 1.1f };
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
        public void Words_Negative_TripleNestedSubAndExpression()
        {
            DS.Assert.Words("aa bb", "aa & (bb & (aa & (a & z)))");
        }

        [TestMethod, ExpectedException(typeof(DynamicSugar.AssertFailedException))]
        public void Words_Negative_TripleSubAndExpression()
        {
            DS.Assert.Words("aa bb", "aa & bb & z");
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

        [TestMethod]
        public void Word_OrWhereSecondTermMatch()
        {
            DS.Assert.Words("bb", "aa | bb");
        }

        [TestMethod]
        public void Word_AndNestedWithOrExpressionWhereSecondTermMatch()
        {
            DS.Assert.Words("zz bb", "zz & (aa | bb)");
            DS.Assert.Words("zz bbxx ", "zz & (aa | bb) & xx");
            DS.Assert.Words("zz bbxx ", "zz & (aa | bb) & xx & (a | aa | aaa | bb) ");
            DS.Assert.Words("zz bbxx ", "zz & (aa | bb) & xx & (a | aa | aaa | bb) ");
        }

        [TestMethod, ExpectedException(typeof(DynamicSugar.AssertFailedException))]
        public void Word_AndNestedWithOrExpressionWhereNoTermMatch()
        {
            DS.Assert.Words("zz bb", "zz & (aa | tt)");
        }

        [TestMethod, ExpectedException(typeof(DynamicSugar.AssertFailedException))]
        public void Word_OrWhereNoTermMatch()
        {
            DS.Assert.Words("dd", "aa | bb | cc");
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

        [TestMethod]
        public void Words_Positive_ExpectCount()
        {
            DS.Assert.Words("aa bb cc", "aa & aaa & bb & bbb", expectedMinimumCountMatch: 2);
            DS.Assert.Words("aa bb", "aa & bb", expectedMinimumCountMatch: 1);
        }

        [TestMethod, ExpectedException(typeof(DynamicSugar.AssertFailedException))]
        public void Words_Negative_ExpectCount_2_OutOf_3()
        {
            // Only aa and bb will match returning 2 and not >= 3
            DS.Assert.Words("aa bb cc", "aa & aaa & bb & bbb", expectedMinimumCountMatch: 3);
        }

        [TestMethod]
        public void Words_Positive_ExpectCount_ZeroMatch_Or()
        {
            // Having a or and a minium match does not make sense. But I am supporting it anyway
            DS.Assert.Words("aa bb cc", "aa | bb | cc", expectedMinimumCountMatch: 3);
        }

        [TestMethod, ExpectedException(typeof(DynamicSugar.AssertFailedException))]
        public void Words_Negative_ExpectCount_ZeroMatch_Or()
        {
            // Having a or and a minium match does not make sense. But I am supporting it anyway
            DS.Assert.Words("aa bb cc", "zz | yy | xx", expectedMinimumCountMatch: 3);
        }

        [TestMethod, ExpectedException(typeof(DynamicSugar.AssertFailedException))]
        public void Words_Negative_ExpectCount_ZeroMatch_And()
        {
            DS.Assert.Words("aa bb cc", "zz & yy & xx", expectedMinimumCountMatch: 3);
        }

        public void Words_Positive_ExpectCount_ZeroMatch_And()
        {
            DS.Assert.Words("aa bb cc", "aa & bb & cc", expectedMinimumCountMatch: 3);
        }
    }
}
