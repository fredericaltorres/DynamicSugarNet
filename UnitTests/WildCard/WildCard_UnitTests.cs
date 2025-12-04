using DynamicSugar;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using static DynamicSugar.Tokenizer;

namespace DynamicSugarSharp_UnitTests
{

    [TestClass]
    public class WildCard_UnitTests
    {
        [TestMethod]
        public void IsMatch_OneWildCard()
        {
            var tests = new[]
            {
                new { Text = "hello.txt", Pattern = "*.txt", Expected = true },
                new { Text = "hello.txt", Pattern = "*.jpg", Expected = false },

                new { Text = "document.pdf", Pattern = "doc*.pdf", Expected = true },
                new { Text = "document.pdf", Pattern = "doc*en?.pdf", Expected = true },
                new { Text = "image.jpeg", Pattern = "img?.jpeg", Expected = false },

                new { Text = "report2021.docx", Pattern = "report????.docx", Expected = true },
                new { Text = "data.csv", Pattern = "data.*", Expected = true },
                new { Text = "archive.zip", Pattern = "*.zip,*.rar", Expected = true },
                new { Text = "notes.txt", Pattern = "*.doc,*.pdf", Expected = false },
                new { Text = "presentation.pptx", Pattern = "present*.ppt?", Expected = true },
                //new { Text = "summary.doc", Pattern = "", Expected = false }, 
                //new { Text = "summary.doc", Pattern = null as string, Expected = false }, 
            };

            foreach (var test in tests)
            {
                AssertWildCard(test);
            }
        }

        private static void AssertWildCard(dynamic test)
        {
            var result = WildCard.IsMatch(test.Text, test.Pattern);
            Assert.AreEqual(test.Expected, result, $"Failed for Text: '{test.Text}' with Pattern: '{test.Pattern}'");
        }

        [TestMethod]
        public void IsMatch_MultipleWildCard()
        {
            var tests = new[]
            {
                new { Text = "hello.txt", Pattern = "*.log,*.txt", Expected = true },
                new { Text = "hello.txt", Pattern = "*.txt,*.log", Expected = true },
                new { Text = "hello.txt", Pattern = "*.jpg,*.bmp", Expected = false },

                new { Text = "document.pdf", Pattern = "doc*.pde,doc*.pdf", Expected = true },
                new { Text = "document.pdf", Pattern = "doc*.pde,doc*.pdz", Expected = false },

                new { Text = "document.pdf", Pattern = "doc*en?.pdZ,doc*en?.PDF", Expected = true },
                new { Text = "document.pdf", Pattern = "doc*en?.pdZ,doc*en?.pdB", Expected = false },

                new { Text = "report2021.docx", Pattern = "report???.docx,report????.docx", Expected = true },
                new { Text = "report2021.docx", Pattern = "report????.docx,report???.docx", Expected = true },

                new { Text = "data.csv", Pattern = "doto.*,data.*", Expected = true },
                new { Text = "data.csv", Pattern = "data.*,doto.*", Expected = true },

                new { Text = "archive.zip", Pattern = "*.zip,*.rar", Expected = true },
                new { Text = "archive.zip", Pattern = "*.rar,*.zip", Expected = true },
                new { Text = "archive.zip", Pattern = "*.rar,*.zup,*.zip,", Expected = true },
                new { Text = "archive.zip", Pattern = "*.rar,*.zup,*.zap,", Expected = false },

                new { Text = "notes.txt", Pattern = "*.doc,*.pdf", Expected = false },
                new { Text = "notes.txt", Pattern = "*.doc,*.pdf,*.txt", Expected = true },

                new { Text = "presentation.pptx", Pattern = "present*.ppt?", Expected = true },
                //new { Text = "summary.doc", Pattern = "", Expected = false }, // Empty pattern matches all
            };

            foreach (var test in tests)
            {
                AssertWildCard(test);
            }
        }
    }
}

