using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Net.Mime;

namespace DynamicSugarSharp_UnitTests{

    [TestClass]
    public class StringFormat_UnitTests {

        [TestMethod]
        public void string_format() {
                        
            var s = String.Format("[{0}] Age={1:000}", "TORRES", 45);
            Assert.AreEqual("[TORRES] Age=045",s);

            s = "[{0}] Age={1:000}".FormatString("TORRES", 45);
            Assert.AreEqual("[TORRES] Age=045",s);
        }

        [TestMethod]
        public void String_Format()
        {
            var s = String.Format("[{0}] Age={1:000}", "TORRES", 45);
            Assert.AreEqual("[TORRES] Age=045", s);

            string a = "[{0}] Age={1:000}";
            s = a.FormatString("TORRES", 45);
            Assert.AreEqual("[TORRES] Age=045", s);
        }
        
        [TestMethod]
        public void String_Format__WithDictionary() {
                        
            var dic = new Dictionary<string,object>() {
                { "LastName" , "TORRES" },
                { "Age"      , 45       }
            };
            var s = "[{LastName}] Age={Age:000}".Template(dic);
            Assert.AreEqual("[TORRES] Age=045",s);
        }
        [TestMethod]
        public void String_Format__WithAnonymousType() {

            var s = "[{LastName}] Age={Age:000}".Template(new { LastName = "TORRES", Age = 45 });
            Assert.AreEqual("[TORRES] Age=045", s);
        }
        [TestMethod,ExpectedException(typeof(ExtendedFormatException))]
        public void String_Format__WithAnonymousType_WithTypoInFormat() {

            var s = "[{LastName_BAD}] Age={Age:000}".Template(new { LastName = "TORRES", Age = 45 });
            Assert.AreEqual("[TORRES] Age=045", s);
        }
        [TestMethod]
        public void String_Format__WithExpandoObject() {
                 
            dynamic eo  = new ExpandoObject();
            eo.LastName = "TORRES";
            eo.Age      = 45;
            var s = "[{LastName}] Age={Age:000}".Template(eo as ExpandoObject);
            Assert.AreEqual("[TORRES] Age=045",s);
        }

        [TestMethod]
        public void RemoveComment__C_Comment_SlashStar()
        {
            var result = $"[/* comment */]".RemoveComment(commentType: ExtensionMethods_Format.StringComment.C);
            var expected = $"[]";
            Assert.AreEqual(expected, result);


            result = @"[Hello/* comment 
*/]".RemoveComment(commentType: ExtensionMethods_Format.StringComment.C);
            expected = @"[Hello
]";
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void RemoveComments__C_Comment_SlashStar()
        {
            var result = $"[/* comment */]".RemoveComments(commentType: ExtensionMethods_Format.StringComment.C);
            var expected = $"[]";
            Assert.AreEqual(expected, result);


            result = @"[Hello/* comment 
*/]".RemoveComments(commentType: ExtensionMethods_Format.StringComment.C);
            expected = @"[Hello
]";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RemoveComment__Pyhton_Comment()
        {
            var result = @"print(""Hello World"") # a comment".RemoveComment(commentType: ExtensionMethods_Format.StringComment.Python);
            var expected = @"print(""Hello World"") ";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RemoveComment__SQL_Comment()
        {
            var result = @"print(""Hello World"") -- a comment".RemoveComment(commentType: ExtensionMethods_Format.StringComment.SQL);
            var expected = @"print(""Hello World"") ";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RemoveComment__SQL_Comment_CornerCaseWithOneExtraDash()
        {
            var result = @"print(""Hello World"") --- a comment".RemoveComment(commentType: ExtensionMethods_Format.StringComment.SQL);
            var expected = @"print(""Hello World"") ";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RemoveComment__SQL_Comment_CornerCaseWithTwoExtraDash()
        {
            var result = @"print(""Hello World"") ---- a comment".RemoveComment(commentType: ExtensionMethods_Format.StringComment.SQL);
            var expected = @"print(""Hello World"") ";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RemoveComment__CPP_Comment()
        {
            var result = @"print(""Hello World"") // a comment".RemoveComment(commentType: ExtensionMethods_Format.StringComment.CPP);
            var expected = @"print(""Hello World"") ";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RemoveComments__CPP_Comment()
        {
            var text = @"
print(""Hello World"") // a comment
var a = 1; // a comment
";

            var expected = @"print(""Hello World"") 
var a = 1; ";

            var result = text.RemoveComments(commentType: ExtensionMethods_Format.StringComment.CPP);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void String_TemplateWithDifferentMacro()
        {
            var dic = new Dictionary<string, object>() {
                { "LastName" , "TORRES" },
                { "Age"      , 45       }
            };
            var s = "[LastName]:[Age]".Template(dic, "[", "]");
            Assert.AreEqual("TORRES:45", s);
        }

        const string MarkdownWithConditionalIf = @"
# Scheduler and Personal Assistant

Current date: [dayOfWeek], [date].
Current time: [time].

You are a time-aware personal assistant running on a recurring loop.
You are Professional and reliable.

#if weekEnd
---

## Time Windows For Week End (Saturday and Sunday only)

---

### Time Window: 10:00 AM – 11:00 AM

- Ask the user one of the following questions?
     * Did you party last night?

### Time Window: 11:00 AM – 12:00 PM

- Ask the user: what are you planning to do this week-end?
     * Gardening
     * AI Research?
     * Any pending task for the day job at Brainshark?

#endif     


#if dayOfWeek

## Time Windows For Week Day

### Time Window: 9:00 AM – 10:00 AM
- Ask the user one of the following questions?
     * How are you feeling this morning?
     * Did I sleep well?
     * How is my energy level?

Keep it conversational and caring — 2 to 3 short questions max.

---

### Time Window: 10:15 AM - 10:30 AM

- Remind me that the daily stand-up meeting is coming up or starting now. 
Keep it brief and action-oriented. 
Nudge me to wrap up what I'm doing and join.

---

### Time Window: 11:00 AM - 12:00 PM
Tell me it's time to begin my first deep work block of the day. 
Encourage me to eliminate distractions, focus for two hours, and give me a short motivational push to get started strong.

#endif

---

If the current time does not match any of the windows above, simply say:
""No scheduled prompt for this time. Keep going! 💪""

";

        [TestMethod]
        public void String_TemplateWithConditionalIf_PositiveCase()
        {
            var conditionalIfDictionary = new Dictionary<string, bool>() {
                { "dayOfWeek", true },
                { "weekEnd", false}
            };

            var now = new DateTime(2026, 3, 29, 9, 35, 0); // March 29, 2026 at 2:35 PM
            var dayOfWeek = now.DayOfWeek.ToString();
            var date = now.ToString("MMMM dd, yyyy"); // March 29, 2026
            var time = now.ToString("hh:mm tt");// 02:35 PM

            var s = MarkdownWithConditionalIf.Template(new { dayOfWeek, date, time }, "[", "]", conditionalIfDictionary);
            Assert.IsFalse(s.Contains("## Time Windows For Week End (Saturday and Sunday only)"));
            Assert.IsFalse(s.Contains("* Did you party last night?"));

            Assert.IsTrue(s.Contains("## Time Windows For Week Day"));
            Assert.IsTrue(s.Contains("- Remind me that the daily stand-up meeting is coming up or starting now."));
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void String_TemplateWithConditionalIf_MissingVariableInDictionary()
        {
            var conditionalIfDictionary = new Dictionary<string, bool>() {
                { "dayOfWeek", true }
            };

            var now = new DateTime(2026, 3, 29, 9, 35, 0); // March 29, 2026 at 2:35 PM
            var dayOfWeek = now.DayOfWeek.ToString();
            var date = now.ToString("MMMM dd, yyyy"); // March 29, 2026
            var time = now.ToString("hh:mm tt");// 02:35 PM

            var s = MarkdownWithConditionalIf.Template(new { dayOfWeek, date, time }, "[", "]", conditionalIfDictionary);

        }
    }
}
