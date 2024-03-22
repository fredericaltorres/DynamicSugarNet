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
            var p = new DS.Processor(SOURCE_TEXT1).ExtractMacros();
            Assert.AreEqual(2, p.Macros.Count);

            var expected = @"

message1: foo 
message2: bar
";

            var result = p.ProcessMain();
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
            var p = new DS.Processor(SOURCE_TEXT2_STRING).ExtractMacros();
            Assert.AreEqual(2, p.Macros.Count);

            var expected = @"

message1: foo 
message2: bar
";

            var result = p.ProcessMain();
            Assert.AreEqual(expected, result);
        }

        const string SOURCE_TEXT3_EXTERNAL_MACROS = @"[__PID__]";

        [TestMethod]
        public void Processor_Process_ExternalIds()
        {
            var p = new DS.Processor(SOURCE_TEXT3_EXTERNAL_MACROS).ExtractMacros(@"C:\temp\fLogViewer.IDs.json");
            Assert.IsTrue(p.Macros.Count > 0);

            var result = p.ProcessMain();
            Assert.IsTrue(result.Length > 4);
        }

        const string SOURCE_TEXT_PARAMETERS = @"
#define CONVERT_DATE(xxxx) (DateTimeToTimestamp xxxx / 9999)
[CONVERT_DATE(c._ts*1000)]
";

        [TestMethod]
        public void Processor_Process_WithParameters()
        {
            var p = new DS.Processor(SOURCE_TEXT_PARAMETERS).ExtractMacros();
            Assert.AreEqual(1, p.Macros.Count);

            var expected = @"[(DateTimeToTimestamp c._ts*1000 / 9999)]";

            var result = p.ProcessMain();
            Assert.AreEqual(expected, result.Trim());
        }

        const string SOURCE_TEXT_PARAMETERS_2 = @"
#define CURRENT_DAY_ROOT	2023-09-19
#define CONVERT_DATE(x)		(DateTimeToTimestamp( x ) / 1000)
#define CURRENT_DAY			(CONVERT_DATE(""CURRENT_DAY_ROOTT00:00:00""))
#define CURRENT_END_OF_DAY	(CONVERT_DATE(""CURRENT_DAY_ROOTT23:59:59"")) 

select c as record FROM c WHERE c._ts > CURRENT_DAY and c._ts < CURRENT_END_OF_DAY
";

        [TestMethod]
        public void Processor_Process_WithParameters_2()
        {
            var p = new DS.Processor(SOURCE_TEXT_PARAMETERS_2).ExtractMacros();
            Assert.AreEqual(4, p.Macros.Count);

            var expected = @"";

            var result = p.ProcessMain();
            ///////Assert.AreEqual(expected, result.Trim());
        }
    }

}
