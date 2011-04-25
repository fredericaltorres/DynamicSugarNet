using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Reflection;

namespace DynamicSugarSharp_UnitTests {

    //TODO:Try extension method to List<T>

    [TestClass]
    public class DS_Resource_UnitTests {
        [TestMethod]
        public void GetTextResource() {

            var exepectedAlphabet = "ABCDEFGHIJKLMNOPQRSTVWXYZ";
            var alphabet = DS.Resources.GetTextResource("Alphabet.txt", Assembly.GetExecutingAssembly());
            Assert.AreEqual(exepectedAlphabet, alphabet);
        }
    }
}

