using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Dynamic;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace DynamicSugarSharp_UnitTests
{

    [TestClass]
    public class Roslyn_UnitTests
    {
        static Assembly CompileCode(string sourceCode)
        {
            // Parse the code into a syntax tree
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

            // Define the assembly name
            string assemblyName = Path.GetRandomFileName();

            // Get references to required assemblies
            var references = new[]
            {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Collections").Location)
        };

            // Create compilation options
            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // Compile to memory stream
            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    // Compilation failed, display errors
                    Console.WriteLine("Compilation failed:");
                    var failures = result.Diagnostics.Where(d =>
                        d.IsWarningAsError ||
                        d.Severity == DiagnosticSeverity.Error);

                    foreach (var diagnostic in failures)
                    {
                        Console.Error.WriteLine($"{diagnostic.Id}: {diagnostic.GetMessage()}");
                    }

                    return null;
                }
                else
                {
                    // Compilation successful, load the assembly
                    ms.Seek(0, SeekOrigin.Begin);
                    return Assembly.Load(ms.ToArray());
                }
            }
        }
        [TestMethod]
        public void ASM_2()
        {
            string staticCode = @"
            using System;
            public static class Helper
            {
                public static string Greet(string name)
                {
                    return $""Hello, {name}! Current time: {DateTime.Now:HH:mm:ss}"";
                }
            }";

            var assembly2 = CompileCode(staticCode);
            if (assembly2 != null)
            {
                var type = assembly2.GetType("Helper");
                var method = type.GetMethod("Greet");
                var greeting = method.Invoke(null, new object[] { "World" });
                Console.WriteLine(greeting);
            }
        }

    }
}
