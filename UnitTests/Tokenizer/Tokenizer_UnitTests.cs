using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Reflection;
using System.CodeDom.Compiler;

namespace DynamicSugarSharp_UnitTests
{

    [TestClass]
    public class Tokenizer_UnitTests
    {
        const string TestLogString1 = @"2025-05-24 A[BB=2] mode: execute";
        const string DateTime1 = @"2025-05-24 13:16:52";
        const string DateTime2 = @"2025/05/24 13-16-52";
        const string DateTimePlusIdentifiers = @"2025/05/24 13-16-52,Info,Export";
        const string DateTimeWithMS = @"2025-05-24 13:16:52.123";
        const string TestDateTimeTZWithMs = @"2025-05-26T22:06:11.513Z";
        const string TestDateTimeTZNoMs = @"2025-05-26T22:06:11Z";

        const string TestLongLogLine = @"2025-05-24 13:16:52.859,   Info   ,Export,[          id: 709046703, mode: Export]        [ExecuteConversion()]       Slide: 10755223, type: IMAGE, index: 0001";

        const string TestString1 = @" ""ok"" 'ko' ";

        [TestMethod]
        public void TokenizerTest_Strings()
        {
            var tokens = new Tokenizer().Tokenize(TestString1);
            var x = 0;

            tokens[x++].Assert(Tokenizer.TokenType.StringLiteralDQuote, "ok");
            tokens[x++].Assert(Tokenizer.TokenType.StringLiteralSQuote, "ko");
        }

        [TestMethod]
        public void TokenizerTest_StringsInArray()
        {
            var tokens = new Tokenizer().Tokenize($"[ {TestString1} ]");
            var x = 0;
            tokens[x++].Assert(Tokenizer.TokenType.Delimiter, "[");
            tokens[x++].Assert(Tokenizer.TokenType.StringLiteralDQuote, "ok");
            tokens[x++].Assert(Tokenizer.TokenType.StringLiteralSQuote, "ko");
            tokens[x++].Assert(Tokenizer.TokenType.Delimiter, "]");
        }

        [TestMethod]
        public void TokenizerTest_SStringsInNameValue()
        {
            var tokens = new Tokenizer().Tokenize("name:'value' ");
            var x = 0;
            var raw = tokens[x].GetRawText();
            Assert.AreEqual("name:'value'", raw);
            tokens[x++].AssertNameValue("value", "name", "name : 'value'");
        }

        [TestMethod]
        public void TokenizerTest_DStringsInNameValue()
        {
            var tokens = new Tokenizer().Tokenize(@"name:""value""");
            var x = 0;
            var raw = tokens[x].GetRawText();
            Assert.AreEqual(@"name:""value""", raw);
            tokens[x++].AssertNameValue("value", "name", @"name : ""value""");
        }

        [TestMethod]
        public void TokenizerTest_NameColonDelimiter_Braket()
        {
            var tokens = new Tokenizer().Tokenize(@"name:[toto]");
            var x = 0;
            Assert.AreEqual(@"name:", tokens[x].GetRawText());
            tokens[x++].AssertNameValue(null, "name", @"name :");
            tokens[x++].Assert(Tokenizer.TokenType.Delimiter, "[");
        }

        [TestMethod]
        public void TokenizerTest_NameColonDelimiter_Coma()
        {
            var tokens = new Tokenizer().Tokenize(@"name:,toto");
            var x = 0;
            Assert.AreEqual(@"name:", tokens[x].GetRawText());
            tokens[x++].AssertNameValue(null, "name", @"name :");
            tokens[x++].Assert(Tokenizer.TokenType.Delimiter, ",");
            tokens[x++].Assert(Tokenizer.TokenType.Identifier, "toto");
        }

        [TestMethod]
        public void TokenizerTest_NameColonDelimiter_FilePath()
        {
            var tokens = new Tokenizer().Tokenize(@"name: c:\windows\notepad.exe, toto");
            var x = 0;

            var raw = tokens[x].GetRawText();
            Assert.AreEqual(@"name: c", raw);
            tokens[x++].AssertNameValue(@"c", "name", @"name : c");

            tokens[x++].AssertDelimiter(":");
            tokens[x++].Assert(Tokenizer.TokenType.FilePath, @"\windows\notepad.exe");
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
        public void TokenizerTest_TestDateTimeTZWithMs()
        {
            var tokens = new Tokenizer().Tokenize(TestDateTimeTZWithMs);
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.DateTime, tokens[x].Type);
            Assert.AreEqual(TestDateTimeTZWithMs, tokens[x++].Value);
        }

        [TestMethod]
        public void TokenizerTest_TestDateTimeTZNoMs()
        {
            var tokens = new Tokenizer().Tokenize(TestDateTimeTZNoMs);
            var x = 0;
            tokens[x].Assert(Tokenizer.TokenType.DateTime, TestDateTimeTZNoMs);
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
            // const string TestLogString1 = @"2025-05-24 A[BB=2] mode: execute";
            var tokens = new Tokenizer().Tokenize(TestLogString1);
            var x = 0;
            tokens[x++].Assert(Tokenizer.TokenType.Date, "2025-05-24");
            tokens[x++].Assert(Tokenizer.TokenType.Identifier, "A");
            tokens[x++].AssertDelimiter("[");
            // [B=2]
            tokens[x++].AssertNameValue("2", "BB", "BB = 2");
            tokens[x++].AssertDelimiter("]");
            // mode: execute
            tokens[x++].AssertNameValue("execute", "mode", "mode : execute");
        }


        [TestMethod]
        public void Tokenizer_LogString_LongComplexLine()
        {
            //  @"2025-05-24 13:16:52.859,   Info   ,Export,[          id: 709046703, mode: Export]        [ExecuteConversion()]       Slide: 10755223, type: IMAGE, index: 0001";
            var tokens = new Tokenizer().Tokenize(TestLongLogLine).RemoveDelimiters();
            var x = 0;

            var aaa = tokens[4].ToString();
            tokens[x++].Assert(Tokenizer.TokenType.DateTime, "2025-05-24 13:16:52.859");
            tokens[x++].AssertIdentifier("Info");
            tokens[x++].AssertIdentifier("Export");

            // [id: 709046703, mode: Export]
            tokens[x++].AssertNameValue("709046703", "id", "id : 709046703");
            tokens[x++].AssertNameValue("Export", "mode", "mode : Export");

            //[ExecuteConversion()]
            tokens[x++].AssertIdentifier("ExecuteConversion");


            // Slide: 10755223, type: IMAGE, index: 0001
            tokens[x++].AssertNameValue("10755223", "Slide", "Slide : 10755223");
            tokens[x++].AssertNameValue("IMAGE", "type", "type : IMAGE");
            tokens[x++].AssertNameValue("0001", "index", "index : 0001");
        }

        [TestMethod]
        public void Tokenizer_NameSpaceClassFunction()
        {
            var tokens = new Tokenizer().Tokenize("Global.ExecutorManager.RunTask()").RemoveDelimiters();
            var x = 0;
            tokens[x].Assert(Tokenizer.TokenType.IdentifierPath, "Global.ExecutorManager.RunTask");
        }

        [TestMethod]
        public void Tokenizer_LongOne()
        {
            var testLine = @"2025-05-29 20:52:48.769|SAS|Platform|1.0|INFO|QA_BACK_END_SERVICES_01|F:0||||||Info|msg1=64b,msg2=""64b"",msg3='64b',Information,TSUploader,[systemId: 674, machineName: QA_BACK_END_SERVICES_01][TSUploader.Trace()][INFO][TSExecutor.Execute(), TaskId: 14895851, objectId:804410389, Provider: Nicrosoft]Start";
            var tokens = new Tokenizer().Tokenize(testLine).RemoveDelimiters();

            var variables = tokens.GetVariables();

            /// Assert.AreEqual("0", tokens.GetVariableValue("F")); name value name.len>1
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
            var testLine = @"   3 | 2025/06/12 11:11:18.329 AM | 2025/06/12 11:11:20.380 AM | backEndSvc01 | prod/backendsvc/app_logs | 2025-06-12 11:11:18.329|Barkrain|Core|1.0.1.0|INFO|BackEndsvc01|CEF:0|Barkrain|Core|0|Message|Message|Info|msg=Barkrain Service on BACKENDSVC,Informational,TTSConverter2,[monitorId: 1934, machineName: BACENDSVC][TTSMonitor.Trace()][INFO][TextToSpeechExecutor.ExecuteOnSlide(), JobId:485916067, pid:252369326, Provider: MicrosoftCognitiveServices, slideId:360959416][SUCCEEDED], Duration:2.6s, TextLength:662, Mp3Duration:46s, Mp3Size: 0.3 Mb rt=Jun 12 2025 11:11:18 start=Jun 12 2025 11:11:18 end=Jun 12 2025 11:11:18|   |";
            var tokens = new Tokenizer().Tokenize(testLine);
        }

        [TestMethod]
        public void Tokenizer_ForColorCoding_Url()
        {
            var testLine = @"Start (https://big.atlassian.net/wiki/spaces/8980398205/Export+ReImport+PowerPoint+feature+-+Design+documentation) End";
            var tokens = new Tokenizer().Tokenize(testLine);

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
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            Assert.AreEqual(Tokenizer.TokenType.StringLiteralDQuote_FilePath, tokens[x].Type);
            Assert.AreEqual(@"""c:\windows\notepad.exe""", tokens[x].Value);

            testLine = @"'c:\windows\notepad.exe' ";
            tokens = new Tokenizer().Tokenize(testLine);
            x = 0;
            Assert.AreEqual(Tokenizer.TokenType.StringLiteralSQuote_FilePath, tokens[x].Type);
            Assert.AreEqual(@"'c:\windows\notepad.exe'", tokens[x].Value);

            testLine = @"'\\windows\notepad.exe' ";
            tokens = new Tokenizer().Tokenize(testLine);
            x = 0;
            Assert.AreEqual(Tokenizer.TokenType.StringLiteralSQuote_FilePath, tokens[x].Type);
            Assert.AreEqual(@"'\\windows\notepad.exe'", tokens[x].Value);

            testLine = @"""\\windows\notepad.exe"" ";
            tokens = new Tokenizer().Tokenize(testLine);
            x = 0;
            Assert.AreEqual(Tokenizer.TokenType.StringLiteralDQuote_FilePath, tokens[x].Type);
            Assert.AreEqual(@"""\\windows\notepad.exe""", tokens[x].Value);
        }


        [TestMethod]
        public void Tokenizer_JSON()
        {
            var testLine = @"{ ""JobId"":485939676,""PresentationId"":397596452,""ErrorMessage"":"""",""UserId"":11123574,""TimeStamp"":""2025-06-13T12:22:15.7092738Z"" }";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;

            tokens[x++].Assert(Tokenizer.TokenType.Delimiter, "{");

            tokens[x++].AssertNameValue("485939676", "JobId", @"""JobId"" : 485939676");

            tokens[x++].Assert(Tokenizer.TokenType.Delimiter, ",");

            tokens[x++].AssertNameValue("397596452", "PresentationId", @"""PresentationId"" : 397596452");
        }


        [TestMethod]
        public void Tokenizer_IdentifierPath()
        {
            var testLine = @" prod.backendsvc.app_logs ┊ prod/backendsvc/app_logs | prod\backendsvc\app_logs| prod-backendsvc-app_logs |";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].Assert(Tokenizer.TokenType.IdentifierPath, @"prod.backendsvc.app_logs");
            tokens[x++].Assert(Tokenizer.TokenType.Delimiter, @"┊");

            tokens[x++].Assert(Tokenizer.TokenType.IdentifierPath, @"prod/backendsvc/app_logs");
            tokens[x++].Assert(Tokenizer.TokenType.Delimiter, @"|");

            tokens[x++].Assert(Tokenizer.TokenType.IdentifierPath, @"prod\backendsvc\app_logs");
            tokens[x++].Assert(Tokenizer.TokenType.Delimiter, @"|");

            tokens[x++].Assert(Tokenizer.TokenType.IdentifierPath, @"prod-backendsvc-app_logs");
            tokens[x++].Assert(Tokenizer.TokenType.Delimiter, @"|");
        }


        [TestMethod]
        public void Tokenizer_Filename_NoString()
        {
            var testLine = @"ok c:\windows\notepad.exe , c:\dvt\development 123";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].AssertIdentifier("ok");
            tokens[x++].Assert(Tokenizer.TokenType.FilePath, @"c:\windows\notepad.exe");
            tokens[x++].AssertDelimiter(",");
            tokens[x++].Assert(Tokenizer.TokenType.FilePath, @"c:\dvt\development");
            tokens[x++].AssertNumber("123");
        }

        [TestMethod]
        public void Tokenizer_ComplexLogLine()
        {
            var testLine = @" 106 ┊ 2025/06/13 03:47:05.598 AM ┊ 2025/06/13 03:47:07.192 AM ┊ BackEndService01 ┊ prod/backendsvc/app_logs ┊ 2025-06-13 03:47:05.598|Barkrain|Core|1.0.1.0|INFO|BackEndService01|msg=CEF:0|Barkrain|Core|0|Message|Message|Info|msg=06/13/2025 03:47:05 AM Severity=3(Info)&&&LocalNum=0&&&NonLocalNum=0&&&Source=Barkrain.Barkrain.Platform.EventGridNotificator:Void SendToEventGrid(Barkrain.Barkrain.Platform.EventGridNotificationPayLoad)&&&Description=No error.Server=BACKENDSVC&&&ExtendedInfo=Parameter Info: Param0:[SendToEventGrid.SignalR]{""JobId"":485937390,""PresentationId"":773853575,""ErrorMessage"":"""",""UserId"":11523947,""TimeStamp"":""2025-06-13T07:47:05.5827769Z"",""FailedSlides"":null,""RequestedJobState"":125,""RequestedJobStateString"":""ADDING_AUDIO_JOBS"",""RequestedJobStateExtraInfoString"":""TextToSpeechExecutor"",""PresentationBatchJobStateString"":""ADDING_AUDIO_JOBS"",""PresentationBatchJobState"":125,""JobStateType"":2,""JobStateTypeString"":""Processing"",""JobErrorCode"":0,""JobErrorCodeString"":""NO_ERROR"",""PercentComplete"":100,""CompletedSlideJobs"":0,""TotalSlideJobs"":0},Param1:Barkrain.Barkrain.Platform.EventGridNotificator rt=Jun 13 2025 03:47:05 start=Jun 13 2025 03:47:05 end=Jun 13 2025 03:47:05|sid=|cid=|uid=|pid=|errorCode=-1|errorMessage=NO_ERROR_CODE_PROVIDED|url=|  ┊";
            var tokens = new Tokenizer().Tokenize(testLine);
            //var x = 0;
            //tokens[x++].Assert(Tokenizer.TokenType.Delimiter, "{");
        }

        [TestMethod]
        public void Tokenizer_RawText_1()
        {
            var testLine = @"  | _messagetime               | _receipttime               | _collector                     ";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;

            Assert.AreEqual("  |", tokens[x].GetRawText());
            tokens[x++].Assert(Tokenizer.TokenType.Delimiter, "|");

            Assert.AreEqual(" _messagetime", tokens[x].GetRawText());
            tokens[x++].Assert(Tokenizer.TokenType.Identifier, "_messagetime");

            Assert.AreEqual("               |", tokens[x].GetRawText());
            tokens[x++].Assert(Tokenizer.TokenType.Delimiter, "|");

            Assert.AreEqual(" _receipttime", tokens[x].GetRawText());
            tokens[x++].Assert(Tokenizer.TokenType.Identifier, "_receipttime");

            Assert.AreEqual("               |", tokens[x].GetRawText());
            tokens[x++].Assert(Tokenizer.TokenType.Delimiter, "|");

            Assert.AreEqual(" _collector", tokens[x].GetRawText());
            tokens[x++].Assert(Tokenizer.TokenType.Identifier, "_collector");

            var rawText = tokens.GetRawText();
            Assert.AreEqual(testLine.TrimEnd(), rawText.TrimEnd()); // W We ignore the space at the end
        }

        [TestMethod]
        public void Tokenizer_RawText_2()
        {
            var tokens = new Tokenizer().Tokenize(TestLongLogLine);
            var rawText = tokens.GetRawText();
            Assert.AreEqual(TestLongLogLine.TrimEnd(), rawText.TrimEnd()); // W We ignore the space at the end
        }

        [TestMethod]
        public void Tokenizer_URL()
        {
            var url = "https://jir.bitbuclet.com/browse/ZZZZ-20055";
            var testLine = $@" ({url}) ";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;

            tokens[x++].Assert(Tokenizer.TokenType.Delimiter, "(");
            tokens[x++].Assert(Tokenizer.TokenType.Url, url);
            var raw = tokens[x - 1].GetRawText();
            Assert.AreEqual(url, raw);

            tokens[x++].Assert(Tokenizer.TokenType.Delimiter, ")");
        }


        [TestMethod]
        public void Tokenizer_BracketWithNames()
        {
            var testLine = $@"[BACKENDSVC,Barkrain.Converters.Listener.VideoStatus,UpdateVideoJobStatus,]";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;

            tokens[x++].AssertDelimiter("[");
            tokens[x++].AssertIdentifier("BACKENDSVC");
            tokens[x++].AssertDelimiter(",");
            tokens[x++].AssertIdentifierPath("Barkrain.Converters.Listener.VideoStatus");
            tokens[x++].AssertDelimiter(",");
            tokens[x++].AssertIdentifier("UpdateVideoJobStatus");
            tokens[x++].AssertDelimiter(",");
            tokens[x++].AssertDelimiter("]");
        }


        [TestMethod]
        public void Tokenizer_Time()
        {
            var testLine = "03:02:01 foo";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].Assert(Tokenizer.TokenType.Time, "03:02:01");
            tokens[x++].Assert(Tokenizer.TokenType.Identifier, "foo");
        }

        [TestMethod]
        public void Tokenizer_NegativeTimeZone()
        {
            var testLine = $@"-04:00 foo";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].Assert(Tokenizer.TokenType.TimeZoneOffset, "-04:00");
            tokens[x++].Assert(Tokenizer.TokenType.Identifier, "foo");
        }

        [TestMethod]
        public void Tokenizer_NegativeNumber()
        {
            var testLine = $@" -04 foo";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].AssertNumber("-04");
            tokens[x++].Assert(Tokenizer.TokenType.Identifier, "foo");
        }


        [TestMethod]
        public void Tokenizer_Header()
        {
            var testLine = $@"

Decoded-Values:

Number 6
DateTime 2025/06/13 12:39:00.401 PM
DateTime 2025/06/13 12:39:01.874 PM";

            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].AssertNameValue("Number", "Decoded-Values", "Decoded-Values : Number");
            tokens[x++].AssertNumber("6");
            tokens[x++].AssertIdentifier("DateTime");
            tokens[x++].Assert(Tokenizer.TokenType.DateTime, "2025/06/13 12:39:00.401 PM");
        }


        [TestMethod]
        public void Tokenizer_CommandLine_Linux_LongFormat_NoSpaceBefore_ButIsFirstToken()
        {
            var testLine = $@"--win01, ";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].AssertDelimiter("-");
            tokens[x++].AssertDelimiter("-");
            tokens[x++].AssertIdentifier("win01");
            tokens[x++].AssertDelimiter(",");
        }

        [TestMethod]
        public void Tokenizer_CommandLine_Linux_LongFormat_NoSpaceBefore()
        {
            var testLine = $@"MachineName:uat3node--win01, ";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].AssertNameValue("uat3node--win01", "MachineName", @"MachineName : uat3node--win01");
            tokens[x++].AssertDelimiter(",");
        }


        [TestMethod]
        public void Tokenizer_CommandLine_Linux_LongFormat()
        {
            var testLine = $@"
""C:\development\Console.exe"" webServicesMobile --authId ""TestingX__Company_06"" --username TestingX__Company_06_CoursePerf --password ""xie"" --host ""uat.Barkrain.com"" --setStagingDB  --verbose --logFile ""C:\Barkrain\logs\Barkrain.IntegratedTest.Converters.log"" --toto 1
";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].Assert(Tokenizer.TokenType.StringLiteralDQuote_FilePath, @"""C:\development\Console.exe""");
            tokens[x++].IsIdentifier("webServicesMobile");

            Assert.AreEqual(@"--authId ""TestingX__Company_06""", tokens[x].GetRawText());
            Assert.AreEqual(@"--authId", tokens[x].ValueAsString);
            Assert.AreEqual(@"--authId", tokens[x].Value);

            tokens[x++].AssertCommandLineParameter( @"--authId", "TestingX__Company_06");
            tokens[x++].AssertCommandLineParameter( @"--username", "TestingX__Company_06_CoursePerf");
            tokens[x++].AssertCommandLineParameter( @"--password", "xie");
            tokens[x++].AssertCommandLineParameter( @"--host", "uat.Barkrain.com");
            tokens[x++].AssertCommandLineParameter( @"--setStagingDB");
            tokens[x++].AssertCommandLineParameter( @"--verbose");
            tokens[x++].AssertCommandLineParameter( @"--logFile", @"C:\Barkrain\logs\Barkrain.IntegratedTest.Converters.log");

            Assert.AreEqual(@"--toto 1", tokens[x].GetRawText());
            Assert.AreEqual(@"--toto", tokens[x].ValueAsString);
            Assert.AreEqual(@"--toto", tokens[x].Value);
            tokens[x++].AssertCommandLineParameter( @"--toto", "1");
        }


        [TestMethod]
        public void Tokenizer_CommandLine_Linux_ShortFormat()
        {
            var testLine = $@"
""C:\development\Console.exe"" webServicesMobile -authId ""TestingX__Company_06"" -username ""TestingX__Company_06_CoursePerf"" -password ""xiexie"" -host ""staging.Barkrain.com"" -setStagingDB  -verbose -logFile""C:\Barkrain\logs\Barkrain.IntegratedTest.Converters.log"" -toto 1
";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].Assert(Tokenizer.TokenType.StringLiteralDQuote_FilePath, @"""C:\development\Console.exe""");
            tokens[x++].IsIdentifier("webServicesMobile");
            tokens[x++].Assert(Tokenizer.TokenType.CommandLineParameter, @"-authId");
            tokens[x++].Assert(Tokenizer.TokenType.StringLiteralDQuote, @"TestingX__Company_06");
        }

        [TestMethod]
        public void Tokenizer_CommandLine_Linux_ShortFormat_NoSpaceBefore_ButIsFirstToken()
        {
            var testLine = $@"-win01, ";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].AssertDelimiter("-");
            tokens[x++].AssertIdentifier("win01");
            tokens[x++].AssertDelimiter(",");
        }

        [TestMethod]
        public void Tokenizer_CommandLine_Linux_ShortFormat_NoSpaceBefore()
        {
            var testLine = $@"MachineName:uat3node-win01, ";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].AssertNameValue("uat3node-win01", "MachineName", @"MachineName : uat3node-win01");
            tokens[x++].AssertDelimiter(",");
        }

        [TestMethod]
        public void Tokenizer_CommandLine_Linux_backSlashSyntax()
        {
            var testLine = $@"
""C:\development\Console.exe"" webServicesMobile /authId ""TestingX__Company_06"" /username ""TestingX__Company_06_CoursePerf"" -password ""xiexie"" /host ""staging.Barkrain.com"" /setStagingDB /verbose /logFile ""C:\Barkrain\logs\Barkrain.IntegratedTest.Converters.log"" /toto 1
";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].Assert(Tokenizer.TokenType.StringLiteralDQuote_FilePath, @"""C:\development\Console.exe""");
            tokens[x++].IsIdentifier("webServicesMobile");
            tokens[x++].Assert(Tokenizer.TokenType.CommandLineParameter, @"/authId");
            tokens[x++].Assert(Tokenizer.TokenType.StringLiteralDQuote, @"TestingX__Company_06");
        }


        [TestMethod]
        public void Tokenizer_Url()
        {
            var url = "https://www.figma.com/design/tPeY9vCvjM2WgO9dbiOktB/%F0%9F%92%A1-Digital-Avatars-Brainstorming?node-id=436-356169&t=i96UB6pDHs4oqHDT-4";
            var testLine = $@"Url ({url})";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].AssertIdentifier("Url");
            tokens[x++].AssertDelimiter("(");
            tokens[x++].AssertUrl(url);
            tokens[x++].AssertDelimiter(")");
        }

        [TestMethod]
        public void Tokenizer_NameValueWithValueBeingAnIdentifierPathDot()
        {
            var testLine = $@"(poco: Barkrain.IntegratedTest.ConverterCheckConvertStatusData)";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].AssertDelimiter("(");
            tokens[x++].AssertNameValue("Barkrain.IntegratedTest.ConverterCheckConvertStatusData" ,"poco", "poco : Barkrain.IntegratedTest.ConverterCheckConvertStatusData");
            tokens[x++].AssertDelimiter(")");
        }

        [TestMethod]
        public void Tokenizer_NameValueWithValueBeingAnIdentifierPathMinus()
        {
            var testLine = $@"(poco: Barkrain-IntegratedTest-ConverterCheckConvertStatusData)";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].AssertDelimiter("(");
            tokens[x++].AssertNameValue("Barkrain-IntegratedTest-ConverterCheckConvertStatusData", "poco", "poco : Barkrain-IntegratedTest-ConverterCheckConvertStatusData");
            tokens[x++].AssertDelimiter(")");
        }

        [TestMethod]
        public void Tokenizer_NameValueWithValueBeingAnIdentifierPathAsString()
        {
            var testLine = $@"(poco: ""Barkrain.IntegratedTest.ConverterCheckConvertStatusData"")";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].AssertDelimiter("(");
            tokens[x++].AssertNameValue("Barkrain.IntegratedTest.ConverterCheckConvertStatusData", "poco", @"poco : ""Barkrain.IntegratedTest.ConverterCheckConvertStatusData""");
            tokens[x++].AssertDelimiter(")");
        }

        [TestMethod]
        public void Tokenizer_FileNameWithMinusInFolderPath()
        {
            var testLine = $@"C:\Barkrain\Fred\QA-A\qazaLion01.rdp";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].AssertFilePath(@"C:\Barkrain\Fred\QA-A\qazaLion01.rdp");
        }


        [TestMethod]
        public void Tokenizer_FileNameWithMinusInFolderPathAndNumber()
        {
            var testLine = $@"C:\Barkrain\Fred\222\qazaLion01.rdp";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].AssertFilePath(@"C:\Barkrain\Fred\222\qazaLion01.rdp");
        }

        [TestMethod]
        public void Tokenizer_FileNameWithMinusInFolderPaths()
        {
            var testLine = $@"(c:\windows\system32\mstsc.exe|C:\Barkrain\Fred\QA-A\qazaLion01.rdp)";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].AssertDelimiter("(");
            tokens[x++].AssertFilePath(@"c:\windows\system32\mstsc.exe");
            tokens[x++].AssertDelimiter("|");
            tokens[x++].AssertFilePath(@"C:\Barkrain\Fred\QA-A\qazaLion01.rdp");
            tokens[x++].AssertDelimiter(")");
        }

        [TestMethod]
        public void Tokenizer_UncFilePath()
        {
            var testLine = $@"UncFilePath (\\aaa\bbb\ccc) IdentifierPath (aaa\bbb\ccc) IdentifierPath (aaa/bbb/ccc) IdentifierPath (aaa.bbb.ccc) IdentifierPath (aaa-bbb-ccc)";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].AssertIdentifier("UncFilePath");
            tokens[x++].AssertDelimiter("(");
            tokens[x++].AssertFilePath(@"\\aaa\bbb\ccc");
            tokens[x++].AssertDelimiter(")");

            tokens[x++].AssertIdentifier("IdentifierPath");
            tokens[x++].AssertDelimiter("(");
            tokens[x++].AssertIdentifierPath(@"aaa\bbb\ccc");
            tokens[x++].AssertDelimiter(")");

            tokens[x++].AssertIdentifier("IdentifierPath");
            tokens[x++].AssertDelimiter("(");
            tokens[x++].AssertIdentifierPath(@"aaa/bbb/ccc");
            tokens[x++].AssertDelimiter(")");

            tokens[x++].AssertIdentifier("IdentifierPath");
            tokens[x++].AssertDelimiter("(");
            tokens[x++].AssertIdentifierPath(@"aaa.bbb.ccc");
            tokens[x++].AssertDelimiter(")");

            tokens[x++].AssertIdentifier("IdentifierPath");
            tokens[x++].AssertDelimiter("(");
            tokens[x++].AssertIdentifierPath(@"aaa-bbb-ccc");
            tokens[x++].AssertDelimiter(")");
        }

        [TestMethod]
        public void Tokenizer_IdentifierPath2()
        {
            var testLine = $@"[HTTPHelper.Execute()]";
            var tokens = new Tokenizer().Tokenize(testLine);
            var x = 0;
            tokens[x++].AssertDelimiter("[");
            Assert.AreEqual(@"HTTPHelper.Execute", tokens[x].GetRawText(), "Raw text should match the identifier path");
            tokens[x++].Assert(Tokenizer.TokenType.IdentifierPath, @"HTTPHelper.Execute");
            tokens[x++].AssertDelimiter("(");
            tokens[x++].AssertDelimiter(")");
            tokens[x++].AssertDelimiter("]");
        }


        const string formattedJson = @"
{
    ""Results"": [
        {
            ""Number"": 213523,
            ""Description"": ""string"",
            ""boolTrue"": true,
            ""boolFalse"": false,
            ""null"": null,
            ""date"" : ""2025-05-26T22:06:11.513Z"",
            ""Presentation"": {
                ""Number"": 213523
            }
        }
    ]
}
";

        [TestMethod]
        public void Tokenizer_AnalyzeFormattedJSON_ParseOnly()
        {
            var x = 0;
            var lineX = 0;
            var jsonLines = formattedJson.SplitByCRLF();

            var tokens = new Tokenizer().Tokenize(jsonLines[lineX++]); x = 0;
            tokens[x++].AssertDelimiter("{");

            tokens = new Tokenizer().Tokenize(jsonLines[lineX++]); x = 0;
            tokens[x++].AssertNameValue(null, "Results", @"""Results"" :");
            tokens[x++].AssertDelimiter("[");

            tokens = new Tokenizer().Tokenize(jsonLines[lineX++]); x = 0;
            tokens[x++].AssertDelimiter("{");

            tokens = new Tokenizer().Tokenize(jsonLines[lineX++]); x = 0;
            tokens[x++].AssertNameValue("213523", "Number", @"""Number"" : 213523");

            tokens = new Tokenizer().Tokenize(jsonLines[lineX++]); x = 0;
            tokens[x++].AssertNameValue("string", "Description", @"""Description"" : ""string""");

            tokens = new Tokenizer().Tokenize(jsonLines[lineX++]); x = 0;
            tokens[x++].AssertNameValue("true", "boolTrue", @"""boolTrue"" : true");

            tokens = new Tokenizer().Tokenize(jsonLines[lineX++]); x = 0;
            tokens[x++].AssertNameValue("false", "boolFalse", @"""boolFalse"" : false");

            tokens = new Tokenizer().Tokenize(jsonLines[lineX++]); x = 0;
            tokens[x++].AssertNameValue("null", "null", @"""null"" : null");

            tokens = new Tokenizer().Tokenize(jsonLines[lineX++]); x = 0;
            tokens[x++].AssertNameValue("2025-05-26T22:06:11.513Z", "date", @"""date"" : ""2025-05-26T22:06:11.513Z""");

            tokens = new Tokenizer().Tokenize(jsonLines[lineX++]); x = 0;
            tokens[x++].AssertNameValue(null, "Presentation", @"""Presentation"" :");
            tokens[x++].AssertDelimiter("{");
        }

        [TestMethod]
        public void Tokenizer_AnalyzeFormattedJSON_ReturnAnalyze()
        {
            var analyse = new Tokenizer().AnalyzeFormattedJson(formattedJson);
            var actualTypes = analyse.Select(a => a.Type).ToList();
            var actualTypesStr = string.Join(",Tokenizer.AnalyzedJsonLineType.", actualTypes);
            var expectedTypes = DS.List(Tokenizer.AnalyzedJsonLineType.StartObject, Tokenizer.AnalyzedJsonLineType.StartPropertyArray, Tokenizer.AnalyzedJsonLineType.StartObject, Tokenizer.AnalyzedJsonLineType.PropertyNumber, Tokenizer.AnalyzedJsonLineType.PropertyString, Tokenizer.AnalyzedJsonLineType.PropertyBool, Tokenizer.AnalyzedJsonLineType.PropertyBool, Tokenizer.AnalyzedJsonLineType.PropertyNull, Tokenizer.AnalyzedJsonLineType.PropertyDate, Tokenizer.AnalyzedJsonLineType.StartPropertyObject, Tokenizer.AnalyzedJsonLineType.PropertyNumber, Tokenizer.AnalyzedJsonLineType.EndObject, Tokenizer.AnalyzedJsonLineType.EndObject, Tokenizer.AnalyzedJsonLineType.EndArray, Tokenizer.AnalyzedJsonLineType.EndObject);

            for(var i = 0; i < expectedTypes.Count; i++)
                Assert.AreEqual(expectedTypes[i], actualTypes[i], $"Mismatch at index {i}: expected {expectedTypes[i]}, got {actualTypes[i]}");
        }
    }
}
