using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Reflection;
using System.Web.UI.WebControls;
using DynamicSugar.TextFileHelper;

namespace DynamicSugarSharp_UnitTests {

    [TestClass]
    public class TextFileHelper_UnitTests
    {
        const string LogToTextSourceSample = @"
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
Video Processing Performance: 5 in process at  2024/09/27 04:33:48.087 PM #logto:c:\temp\ProcessLogToMacro.log
Tutu Errors:
Video Processing Performance: 9 in process at  2024/09/27 05:55:48.087 PM #logto:c:\temp\ProcessLogToMacro.log
";

        [TestMethod]
        public void ProcessLogToMacro()
        {
            var tfh = new TestFileHelper();
            var logToFileName = @"c:\temp\ProcessLogToMacro.log";
            tfh.DeleteFile(logToFileName);
            
            var textResult = TextFileReWriter.ProcessLogToMacro(LogToTextSourceSample, Environment.NewLine);

            Assert.IsTrue(File.Exists(logToFileName));
            Assert.IsTrue(File.ReadAllText(logToFileName).Contains("5 in process"));
            Assert.IsTrue(File.ReadAllText(logToFileName).Contains("9 in process"));
            Assert.AreEqual(LogToTextSourceSample, textResult);

            tfh.DeleteFile(logToFileName);
        }
    }
}
