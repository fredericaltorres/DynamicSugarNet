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
    public class DS_Processor
    {
        const string SOURCE_TEXT1 = @"
#define __FOO__ foo
#define __BAR__ bar

message1: __FOO__ 
message2: __BAR__
";

        [TestMethod]
        public void Processor_Process_Basic()
        {
            var p = new DS.Processor(SOURCE_TEXT1);
            var macros = p.ExtractMacros();
            Assert.AreEqual(2, macros.Count);

            var expected = @"

message1: foo 
message2: bar
";

            var result = p.Process();
            Assert.AreEqual(expected, result);
        }


        const string SOURCE_TEXT2_STRING = @"
#define __FOO__ ""foo""
#define __BAR__ ""bar""

message1: __FOO__ 
message2: __BAR__
";

        [TestMethod]
        public void Processor_Process_Strings()
        {
            var p = new DS.Processor(SOURCE_TEXT2_STRING);
            var macros = p.ExtractMacros();
            Assert.AreEqual(2, macros.Count);

            var expected = @"

message1: foo 
message2: bar
";

            var result = p.Process();
            Assert.AreEqual(expected, result);
        }

    }



}
