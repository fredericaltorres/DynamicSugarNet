using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Reflection;

namespace DynamicSugarSharp_UnitTests 
{

    [TestClass]
    public class Tokenizer_UnitTests
    {
        const string TestLogString1     = @"2025-05-24 A[B=2] mode: execute";
        const string DateTime1     = @"2025-05-24 13:16:52";
        const string DateTime2    =  @"2025/05/24 13-16-52";
        const string DateTimePlusIdentifiers   = @"2025/05/24 13-16-52,Info,Export";
        const string DateTimeWithMS     = @"2025-05-24 13:16:52.123";
        const string TestDateTimeTZ= @"2025-05-26T22:06:11.513Z";

        const string TestLongLogLine = @"2025-05-24 13:16:52.859,Info,Export,[id: 709046703, mode: Export][ExecuteConversion()]Slide: 10755223, type: IMAGE, index: 0001";

        const string TestString1 = @" ""ok"" 'ko' ";

        [TestMethod]
        public void TokenizerTest_Strings()
        {
            var tokens = new Tokenizer().Tokenize(TestString1);
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.StringLiteralDQuote, tokens[x].Type);
            Assert.AreEqual(@"ok", tokens[x++].Value);

            Assert.AreEqual(Tokenizer.TokenType.StringLiteralSQuote, tokens[x].Type);
            Assert.AreEqual(@"ko", tokens[x++].Value);
        }

        [TestMethod]
        public void TokenizerTest_StringsInArray()
        {
            var tokens = new Tokenizer().Tokenize($"[ {TestString1} ]");
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.ArrayOfTokens, tokens[x].Type);

            var arrayTokens = tokens[x++].ArrayValues;
            var xx = 0;
            Assert.AreEqual(Tokenizer.TokenType.StringLiteralDQuote, arrayTokens[xx].Type);
            Assert.AreEqual(@"ok", arrayTokens[xx++].Value);

            Assert.AreEqual(Tokenizer.TokenType.StringLiteralSQuote, arrayTokens[xx].Type);
            Assert.AreEqual(@"ko", arrayTokens[xx++].Value);
        }

        [TestMethod]
        public void TokenizerTest_StringsInNameValue()
        {
            var tokens = new Tokenizer().Tokenize("name:'value' ");
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.NameValuePair, tokens[x].Type);
            Assert.AreEqual("name", tokens[x].Name);
            Assert.AreEqual("value", tokens[x].Value);
        }

        [TestMethod]
        public void TokenizerTest_DateTime()
        {
            var tokens = new Tokenizer().Tokenize(DateTime1);
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.DateTime, tokens[x].Type);
            Assert.AreEqual(DateTime1, tokens[x++].Value);
        }

        [TestMethod]
        public void TokenizerTest_TestDateTimeTZ()
        {
            var tokens = new Tokenizer().Tokenize(TestDateTimeTZ);
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.DateTime, tokens[x].Type);
            Assert.AreEqual(TestDateTimeTZ, tokens[x++].Value);
        }

        [TestMethod]
        public void TokenizerTest_DateTime_22()
        {
            var tokens = new Tokenizer().Tokenize(DateTime2);
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.DateTime, tokens[x].Type);
            Assert.AreEqual(DateTime2, tokens[x++].Value);
        }

        [TestMethod]
        public void TokenizerTest_DateTime_WithSlashSeparator_AndMoreIdentifierAfter()
        {
            var tokens = new Tokenizer().Tokenize(DateTimePlusIdentifiers).RemoveDelimiters();
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.DateTime, tokens[x].Type);
            Assert.AreEqual(DateTime2, tokens[x++].Value);

            Assert.AreEqual(Tokenizer.TokenType.Identifier, tokens[x].Type);
            Assert.AreEqual("Info", tokens[x++].Value);

            Assert.AreEqual(Tokenizer.TokenType.Identifier, tokens[x].Type);
            Assert.AreEqual("Export", tokens[x++].Value);
        }

        [TestMethod]
        public void TokenizerTest_DateTimeWithMilliSecond()
        {
            var tokens = new Tokenizer().Tokenize(DateTimeWithMS);
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.DateTime, tokens[x].Type);
            Assert.AreEqual(DateTimeWithMS, tokens[x++].Value);
        }

        [TestMethod]
        public void Tokenizer_LogString1()
        {
            var tokens = new Tokenizer().Tokenize(TestLogString1);
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.Date, tokens[x].Type);
            Assert.AreEqual("2025-05-24", tokens[x++].Value);

            Assert.AreEqual(Tokenizer.TokenType.Identifier, tokens[x].Type);
            Assert.AreEqual("A", tokens[x++].Value);

            // [B=2]
            Assert.AreEqual(Tokenizer.TokenType.ArrayOfTokens, tokens[x].Type);
            Assert.AreEqual(1, tokens[x].ArrayValues.Count);
            Assert.AreEqual(Tokenizer.TokenType.NameValuePair, tokens[x].ArrayValues[0].Type);
            Assert.AreEqual("B", tokens[x].ArrayValues[0].Name);
            Assert.AreEqual("2", tokens[x].ArrayValues[0].Value);
            x++;

            Assert.AreEqual(Tokenizer.TokenType.NameValuePair, tokens[x].Type);
            Assert.AreEqual("mode", tokens[x].Name);
            Assert.AreEqual("execute", tokens[x++].Value);
        }

        // const string TestLogString4 = @"2025-05-24 13:16:52.859,Info,Export,[id: 709046703, mode: Export][ExecuteConversion()]Slide: 10755223, type: IMAGE, index: 0001";

        [TestMethod]
        public void Tokenizer_LogString_LongComplexLine()
        {
            var tokens = new Tokenizer().Tokenize(TestLongLogLine).RemoveDelimiters();
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.DateTime, tokens[x].Type);
            Assert.AreEqual("2025-05-24 13:16:52.859", tokens[x++].Value);

            Assert.AreEqual(Tokenizer.TokenType.Identifier, tokens[x].Type);
            Assert.AreEqual("Info", tokens[x++].Value);

            Assert.AreEqual(Tokenizer.TokenType.Identifier, tokens[x].Type);
            Assert.AreEqual("Export", tokens[x++].Value);

            // [id: 709046703, mode: Export]
            Assert.AreEqual(Tokenizer.TokenType.ArrayOfTokens, tokens[x].Type);
            Assert.AreEqual(2, tokens[x].ArrayValues.Count);
            Assert.AreEqual(Tokenizer.TokenType.NameValuePair, tokens[x].ArrayValues[0].Type);
            Assert.AreEqual("id", tokens[x].ArrayValues[0].Name);
            Assert.AreEqual("709046703", tokens[x].ArrayValues[0].Value);

            Assert.AreEqual(Tokenizer.TokenType.NameValuePair, tokens[x].ArrayValues[1].Type);
            Assert.AreEqual("mode", tokens[x].ArrayValues[1].Name);
            Assert.AreEqual("Export", tokens[x].ArrayValues[1].Value);
            x++;

            //[ExecuteConversion()]
            Assert.AreEqual(Tokenizer.TokenType.ArrayOfTokens, tokens[x].Type);
            Assert.AreEqual(Tokenizer.TokenType.Identifier, tokens[x].ArrayValues[0].Type);
            Assert.AreEqual("ExecuteConversion", tokens[x].ArrayValues[0].Value);
            x++;

            // Slide: 10755223, type: IMAGE, index: 0001
            Assert.AreEqual(Tokenizer.TokenType.NameValuePair, tokens[x].Type);
            Assert.IsTrue(tokens[x].IsNameValue("Slide", "10755223"));
            x++;

            Assert.AreEqual(Tokenizer.TokenType.NameValuePair, tokens[x].Type);
            Assert.IsTrue(tokens[x].IsNameValue("type", "IMAGE"));
            x++;

            Assert.AreEqual(Tokenizer.TokenType.NameValuePair, tokens[x].Type);
            Assert.IsTrue(tokens[x].IsNameValue("index", "0001"));
            x++;
        }

        [TestMethod]
        public void Tokenizer_NameSpaceClassFunction()
        {
            var tokens = new Tokenizer().Tokenize("Global.ExecutorManager.RunTask()").RemoveDelimiters();
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.Identifier, tokens[x].Type);
            Assert.AreEqual("Global.ExecutorManager.RunTask", tokens[x++].Value);
        }

        [TestMethod]
        public void Tokenizer_LongOne()
        {
            var testLine = @"2025-05-29 20:52:48.769|SAS|Platform|1.0|INFO|QA_BACK_END_SERVICES_01|F:0||||||Info|msg1=64b,msg2=""64b"",msg3='64b',Information,TSUploader,[systemId: 674, machineName: QA_BACK_END_SERVICES_01][TSUploader.Trace()][INFO][TSExecutor.Execute(), TaskId: 14895851, objectId:804410389, Provider: Nicrosoft]Start";
            var tokens = new Tokenizer().Tokenize(testLine).RemoveDelimiters();

            var variables = tokens.GetVariables();

            Assert.AreEqual("0", tokens.GetVariableValue("F"));
            Assert.AreEqual("64", tokens.GetVariableValue("msg1"));
            Assert.AreEqual("64b", tokens.GetVariableValue("msg2"));
            Assert.AreEqual("64b", tokens.GetVariableValue("msg3"));
            Assert.AreEqual("674", tokens.GetVariableValue("systemId"));
            Assert.IsTrue(tokens.IdentifierExists("Information"));

            Assert.IsTrue(tokens.IdentifierExists(DS.List("SAS", "Platform", "QA_BACK_END_SERVICES_01", "Information", "TSUploader", "INFO", "TSExecutor.Execute", "Start")));

            var scriptWithType = tokens.GetTokenScript(true);
            var scriptWithNoType = tokens.GetTokenScript(false);

            var x = 0;
            //Assert.AreEqual(Tokenizer.TokenType.Identifier, tokens[x].Type);
            //Assert.AreEqual("Global.ExecutorManager.RunTask", tokens[x++].Value);
        }

        [TestMethod]
        public void Tokenizer_ForColorCoding()
        {
            var testLine = @"   3 | 2025/06/12 11:11:18.329 AM | 2025/06/12 11:11:20.380 AM | bos3bkndsvc01 | prod/backendsvc/app_logs | 2025-06-12 11:11:18.329|Brainshark|Core|1.0.1.0|INFO|bos3bkndsvc01|CEF:0|Brainshark|Core|0|Message|Message|Info|msg=BrainsharkMonitorService64 on BOS3BKNDSVC01,Informational,TTSConverter2,[monitorId: 1934, machineName: BOS3BKNDSVC01][TTSMonitor.Trace()][INFO][TextToSpeechExecutor.ExecuteOnSlide(), JobId:485916067, pid:252369326, Provider: MicrosoftCognitiveServices, slideId:360959416][SUCCEEDED], Duration:2.6s, TextLength:662, Mp3Duration:46s, Mp3Size: 0.3 Mb rt=Jun 12 2025 11:11:18 start=Jun 12 2025 11:11:18 end=Jun 12 2025 11:11:18 dvchost=bos3bkndsvc01|   |";
            var tokens = new Tokenizer().Tokenize(testLine, combineArray: false);
        }

        [TestMethod]
        public void Tokenizer_ForColorCoding_Url()
        {
            var testLine = @"Start (https://big.atlassian.net/wiki/spaces/8980398205/Export+ReImport+PowerPoint+feature+-+Design+documentation) End";
            var tokens = new Tokenizer().Tokenize(testLine, combineArray: false);

            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.Identifier, tokens[x].Type);
            Assert.AreEqual("Start", tokens[x].Value);
            x += 1;

            Assert.AreEqual(Tokenizer.TokenType.Delimiter, tokens[x].Type);
            Assert.AreEqual("(", tokens[x].Value);
            x += 1;

            Assert.AreEqual(Tokenizer.TokenType.Url, tokens[x].Type);
            Assert.AreEqual("https://big.atlassian.net/wiki/spaces/8980398205/Export+ReImport+PowerPoint+feature+-+Design+documentation", tokens[x].Value);
            x += 1;

            Assert.AreEqual(Tokenizer.TokenType.Delimiter, tokens[x].Type);
            Assert.AreEqual(")", tokens[x].Value);
            x += 1;

            Assert.AreEqual(Tokenizer.TokenType.Identifier, tokens[x].Type);
            Assert.AreEqual("End", tokens[x].Value);
        }


        [TestMethod]
        public void Tokenizer_FileNameInString()
        {
            var testLine = @"""c:\windows\notepad.exe"" ";
            var tokens = new Tokenizer().Tokenize(testLine, combineArray: false);
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.StringLiteralDQuote_FileName, tokens[x].Type);
            Assert.AreEqual(@"""c:\windows\notepad.exe""", tokens[x].Value);

            testLine = @"'c:\windows\notepad.exe' ";
            tokens = new Tokenizer().Tokenize(testLine, combineArray: false);
            x = 0;
            Assert.AreEqual(Tokenizer.TokenType.StringLiteralSQuote_FileName, tokens[x].Type);
            Assert.AreEqual(@"'c:\windows\notepad.exe'", tokens[x].Value);

            testLine = @"'\\windows\notepad.exe' ";
            tokens = new Tokenizer().Tokenize(testLine, combineArray: false);
            x = 0;
            Assert.AreEqual(Tokenizer.TokenType.StringLiteralSQuote_FileName, tokens[x].Type);
            Assert.AreEqual(@"'\\windows\notepad.exe'", tokens[x].Value);

            testLine = @"""\\windows\notepad.exe"" ";
            tokens = new Tokenizer().Tokenize(testLine, combineArray: false);
            x = 0;
            Assert.AreEqual(Tokenizer.TokenType.StringLiteralDQuote_FileName, tokens[x].Type);
            Assert.AreEqual(@"""\\windows\notepad.exe""", tokens[x].Value);
        }

        [TestMethod]
        public void Tokenizer_Filename_NoString()
        {
            var testLine = @"c:\windows\notepad.exe";
            var tokens = new Tokenizer().Tokenize(testLine, combineArray: false);
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.Identifier, tokens[x].Type);
            Assert.AreEqual(@"c", tokens[x].Value);
            x += 1;

            Assert.AreEqual(Tokenizer.TokenType.Delimiter, tokens[x].Type);
            Assert.AreEqual(@":", tokens[x].Value);
            x += 1;

            Assert.AreEqual(Tokenizer.TokenType.Delimiter, tokens[x].Type);
            Assert.AreEqual(@"\", tokens[x].Value);
            x += 1;

            Assert.AreEqual(Tokenizer.TokenType.Identifier, tokens[x].Type);
            Assert.AreEqual(@"windows", tokens[x].Value);
            x += 1;
        }
    }
}
