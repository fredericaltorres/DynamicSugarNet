using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;

namespace DynamicSugarSharp_UnitTests {

    [TestClass]
    public class String_EM_UnitTests {

        [TestMethod]
        public void ToXXXX() {

            Assert.AreEqual(123, "123".ToInt());
            Assert.AreEqual(123u, "123".ToUInt());
            Assert.AreEqual(123L, "123".ToLong());
            Assert.AreEqual(123UL, "123".ToULong());
            Assert.AreEqual(123M, "123".ToDecimal());
            Assert.AreEqual(123.0, "123".ToDouble());
            
            Assert.AreEqual(new Guid("{2E36429B-2695-4CB3-BCF2-9C7C6DC56B45}"), "{2E36429B-2695-4CB3-BCF2-9C7C6DC56B45}".ToGuid());
            Assert.AreEqual(new Guid("{2E36429B-2695-4CB3-BCF2-9C7C6DC56B45}"), "2E36429B-2695-4CB3-BCF2-9C7C6DC56B45".ToGuid());
            Assert.AreEqual(new Guid("2E36429B-2695-4CB3-BCF2-9C7C6DC56B45"), "2E36429B-2695-4CB3-BCF2-9C7C6DC56B45".ToGuid());
            Assert.AreEqual(new Guid("2E36429B-2695-4CB3-BCF2-9C7C6DC56B45"), "{2E36429B-2695-4CB3-BCF2-9C7C6DC56B45}".ToGuid());

            Assert.AreEqual(new DateTime(1964, 12, 11), "12/11/1964".ToDateTime());

            Assert.AreEqual(false, "false".ToBool());
            Assert.AreEqual(true, "true".ToBool());
        }

        [TestMethod]
        public void ToXXXX_NegativeWithDefault() {

            Assert.AreEqual(123, "AA".ToInt(123));
            Assert.AreEqual(123u, "AA".ToUInt(123U));
            Assert.AreEqual(123L, "AA".ToLong(123L));
            Assert.AreEqual(123UL, "AA".ToULong(123UL));
            Assert.AreEqual(123M, "AA".ToDecimal(123M));
            Assert.AreEqual(123.0, "AA".ToDouble(123.0));
            Assert.AreEqual(new Guid("{2E36429B-2695-4CB3-BCF2-9C7C6DC56B45}"), "AA".ToGuid(new Guid("{2E36429B-2695-4CB3-BCF2-9C7C6DC56B45}")));
            Assert.AreEqual(new DateTime(1964, 12, 11), "AA".ToDateTime(new DateTime(1964, 12, 11)));
            Assert.AreEqual(false, "AA".ToBool(false));
            Assert.AreEqual(true, "AA".ToBool(true));
        }

        [TestMethod, ExpectedException(typeof(FormatException))]
        public void ToInt_NegativeNoDefault() { Assert.AreEqual(123, "AAA".ToInt()); }

        [TestMethod, ExpectedException(typeof(FormatException))]
        public void ToDouble_NegativeNoDefault() { Assert.AreEqual(123.0, "AAA".ToDouble()); }

        [TestMethod, ExpectedException(typeof(FormatException))]
        public void ToDecimal_NegativeNoDefault() { Assert.AreEqual(123M, "AAA".ToDecimal()); }

        [TestMethod, ExpectedException(typeof(FormatException))]
        public void ToDateTime_NegativeNoDefault() { Assert.AreEqual(DateTime.Now, "AAA".ToDateTime()); }

        [TestMethod, ExpectedException(typeof(FormatException))]
        public void ToGuid_NegativeNoDefault() { Assert.AreEqual(Guid.Empty, "AAA".ToGuid()); }

        [TestMethod]
        public void Capitalize() {

            Assert.AreEqual("Abcd", "abcd".Capitalize());
            Assert.AreEqual("Abcd", "ABCD".Capitalize());
            Assert.AreEqual("",     "".Capitalize());
            Assert.AreEqual("!@#$",  "!@#$".Capitalize());
            Assert.AreEqual(null,  ((string)null).Capitalize());
        }

        [TestMethod]
        public void RemoveLastChar() {

            string s = "ABCD";
            Assert.AreEqual("ABC", s.RemoveLastChar());
            Assert.AreEqual("ABC", s.RemoveLastChar('D'));
            Assert.AreEqual("ABCD", s.RemoveLastChar('d'));

            s = null;
            Assert.AreEqual(null, s.RemoveLastChar());
            Assert.AreEqual(null, s.RemoveLastChar('a'));
        }
        
        [TestMethod]
        public void RemoveIfStartsWith() {

            string sref = "yesterday I was here";
            string s = sref;
            Assert.AreEqual("terday I was here", s.RemoveIfStartsWith("yes"));
            Assert.AreEqual(sref, s.RemoveIfStartsWith("null"));
            Assert.AreEqual(sref, s.RemoveIfStartsWith(""));

            Assert.AreEqual("", "A".RemoveIfStartsWith("A"));
            Assert.AreEqual("A", "AA".RemoveIfStartsWith("A"));

            Assert.AreEqual("", "".RemoveIfStartsWith("yes"));
            s = null;
            Assert.AreEqual(null, s.RemoveIfStartsWith("yes"));
        }

        [TestMethod]
        public void RemoveIfEndsWith() {

            string sref = "yesterday I was here";
            string s = sref;
            Assert.AreEqual("yesterday I was ", s.RemoveIfEndsWith("here"));
            Assert.AreEqual("yesterday I was", s.RemoveIfEndsWith(" here"));

            Assert.AreEqual(sref, s.RemoveIfEndsWith("null"));
            Assert.AreEqual(sref, s.RemoveIfEndsWith(""));

            Assert.AreEqual("", "A".RemoveIfEndsWith("A"));
            Assert.AreEqual("A", "AA".RemoveIfEndsWith("A"));

        }

        [TestMethod]
        public void RemoveFirstChar() {

            string s = "ABCD";
            Assert.AreEqual("BCD", s.RemoveFirstChar());
            Assert.AreEqual("BCD", s.RemoveFirstChar('A'));
            Assert.AreEqual("ABCD", s.RemoveFirstChar('a'));

            s = null;
            Assert.AreEqual(null, s.RemoveFirstChar());
            Assert.AreEqual(null, s.RemoveFirstChar('a'));
        }

        [TestMethod]
        public void ___IfNullOrEmpty() {

            string s = null;
            Assert.AreEqual("default", s.IfNullOrEmpty("default"));
            s = "";
            Assert.AreEqual("default", s.IfNullOrEmpty("default"));
            s = "A";
            Assert.AreEqual("A", s.IfNullOrEmpty("default"));            
        }
        
        [TestMethod]
        public void ___IfNull()
        {
            string s = null;
            Assert.AreEqual("default", s.IfNull("default"));
            s = "";
            Assert.AreEqual("", s.IfNull("default"));
            s = "A";
            Assert.AreEqual("A", s.IfNull("default"));
        }

        [TestMethod]
        public void IsNullOrEmpty() {

            string s = null;
            Assert.IsTrue(s.IsNullOrEmpty());
            s = "";
            Assert.IsTrue(s.IsNullOrEmpty());
            s = "a";
            Assert.IsFalse(s.IsNullOrEmpty());
        }
        [TestMethod]
        public void IsEmpty() {

            string s = null;
            Assert.IsFalse(s.IsEmpty());
            s = "";
            Assert.IsTrue(s.IsEmpty());
            s = "a";
            Assert.IsFalse(s.IsEmpty());
        }
        
        [TestMethod]
        public void Slice() {

            var s = "ABCD";
            Assert.AreEqual("ABCD", s.Slice(0));
            Assert.AreEqual("BCD" , s.Slice(1));
            Assert.AreEqual("CD"  , s.Slice(2));
            Assert.AreEqual("D"   , s.Slice(3));
        }
        [TestMethod]
        public void Slice_Range() {

            var s = "ABCD";
            Assert.AreEqual("A"     ,s.Slice(0,1));
            Assert.AreEqual("AB"    ,s.Slice(0,2));
            Assert.AreEqual("ABC"   ,s.Slice(0,3));
            Assert.AreEqual("ABCD"  ,s.Slice(0,4));

            Assert.AreEqual("B"     ,s.Slice(1,1));
            Assert.AreEqual("BC"    ,s.Slice(1,2));
            Assert.AreEqual("BCD"   ,s.Slice(1,3));

            Assert.AreEqual("C"     ,s.Slice(2,1));
            Assert.AreEqual("CD"    ,s.Slice(2,2));            
        }

        [TestMethod]        
        public void Slice_Range_All() {

            var s = "ABCD";
            Assert.AreEqual("ABCD"     ,s.Slice(0));
            Assert.AreEqual("BCD"      ,s.Slice(1));
            Assert.AreEqual("CD"       ,s.Slice(2));
            Assert.AreEqual("D"        ,s.Slice(3));
        }

        [TestMethod,ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void Slice_Range_IndexOutOfBand() {
            
            "ABCD".Slice(1, 4);
        }
        
        [TestMethod]
        public void Slice_IndexOutOfBand() {

            Assert.AreEqual("", "ABCD".Slice(10));
        }

        [TestMethod]
        public void Slice_NegativeIndex() {

            var s = "ABCD";
            Assert.AreEqual("DCBA"  , s.Slice(-1));
            Assert.AreEqual("CBA"   , s.Slice(-2));
            Assert.AreEqual("BA"    , s.Slice(-3));
            Assert.AreEqual("A"     , s.Slice(-4));
        }
         
        [TestMethod]
        public void Slice_Range_NegativeIndex() {

            var s = "ABCD";
            Assert.AreEqual("D"      , s.Slice(-1,1));
            Assert.AreEqual("DC"     , s.Slice(-1,2));
            Assert.AreEqual("DCB"    , s.Slice(-1,3));
            Assert.AreEqual("DCBA"   , s.Slice(-1,4));
        }

        [TestMethod]
        public void Slice_Range_NegativeIndex_All() {

            var s = "ABCD";
            Assert.AreEqual("DCBA"  , s.Slice(-1,-1));
            Assert.AreEqual("CBA"   , s.Slice(-2,-1));
            Assert.AreEqual("BA"    , s.Slice(-3,-1));
            Assert.AreEqual("A"     , s.Slice(-4,-1));
        }
        
        public void Slice_NegativeIndex_IndexOutOfBand() {
                        
            Assert.AreEqual("","ABCD".Slice(-110));
        }

        /// <summary>
        /// Based on How to Write a Spelling Corrector by Peter Norvig
        /// http://norvig.com/spell-correct.html
        /// 
        /// </summary>
        [TestMethod]
        public void CutWordHello() {

            var splits = new List<List<string>>();
            var word   = "Hello";
            
            foreach(var i in DS.Range(word.Length+1))
                splits.Add(DS.List(word.Slice(0,i), word.Slice(i,-1)));
            
            Assert.AreEqual(""     , splits[0][0]);
            Assert.AreEqual("Hello", splits[0][1]);
            Assert.AreEqual("H"    , splits[1][0]);
            Assert.AreEqual("ello" , splits[1][1]);
            Assert.AreEqual("He"   , splits[2][0]);
            Assert.AreEqual("llo"  , splits[2][1]);
            Assert.AreEqual("Hel"  , splits[3][0]);
            Assert.AreEqual("lo"   , splits[3][1]);
            Assert.AreEqual("Hell" , splits[4][0]);
            Assert.AreEqual("o"    , splits[4][1]);
            Assert.AreEqual("Hello", splits[5][0]);
            Assert.AreEqual(""     , splits[5][1]);

            var deletes = new List<string>();
            foreach(var s in splits){
                var a = s[0];
                var b = s[1];
                deletes.Add(a + b.Slice(1, -1));
            }
            var transposes = new List<string>();
            foreach(var s in splits){
                var a = s[0];
                var b = s[1];
                if(b.Length>1)
                    transposes.Add(a + b[1] + b[0] + b.Slice(2,-1));
            }

            // Write an extension method, split on space
            var alphabet = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z".Split(',').ToList();

            var replaces = new List<string>();
            foreach (var s in splits)
            {
                var a = s[0];
                var b = s[1];
                foreach (var c in alphabet)                
                    if(b.Length>0)
                        replaces.Add(a + c + b.Slice(1, -1));
            }

            var inserts = new List<string>();
            foreach (var s in splits)
            {
                var a = s[0];
                var b = s[1];
                foreach (var c in alphabet)
                    inserts.Add(a + c + b);
            }
            var all = deletes.Add(transposes).Add(replaces).Add(inserts);
        }

        [TestMethod]
        public void CutWordHelloWithMap() {
            
            var s      = "Hello";
            var splits = DS.Range((int)s.Length+1).Map<int, string>(i => s.Slice(0,i)+","+s.Slice(i,-1));
            
            Assert.AreEqual(",Hello", splits[0]);
            Assert.AreEqual("H,ello", splits[1]);
            Assert.AreEqual("He,llo", splits[2]);
            Assert.AreEqual("Hel,lo", splits[3]);
            Assert.AreEqual("Hell,o", splits[4]);            
            Assert.AreEqual("Hello,", splits[5]);
        }

        [TestMethod]
        public void Slice_Demo() {

            var s = "ABCD";
            Assert.AreEqual("ABCD"  , s.Slice(0));
            Assert.AreEqual("BCD"   , s.Slice(1));
            Assert.AreEqual("AB"    , s.Slice(0,2));
            Assert.AreEqual("BC"    , s.Slice(1,2));
            Assert.AreEqual("DCBA"  , s.Slice(-1));
            Assert.AreEqual("CB"    , s.Slice(-2,2));            
        }
    }
}

