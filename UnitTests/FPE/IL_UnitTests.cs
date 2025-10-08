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

namespace DynamicSugarSharp_UnitTests
{

    [TestClass]
    public class IL_UnitTests
    {

        // Example 2: Create a dynamic property getter
        public static Func<T, TProperty> CreatePropertyGetter<T, TProperty>(string propertyName)
        {
            PropertyInfo prop = typeof(T).GetProperty(propertyName);
            if (prop == null)
                throw new ArgumentException($"Property {propertyName} not found");

            DynamicMethod method = new global::System.Reflection.Emit.DynamicMethod(
                "Get" + propertyName,
                typeof(TProperty),
                new[] { typeof(T) },
                typeof(T).Module);

            ILGenerator il = method.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, prop.GetGetMethod());
            il.Emit(OpCodes.Ret);

            return (Func<T, TProperty>)method.CreateDelegate(typeof(Func<T, TProperty>));
        }


        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        [TestMethod]
        public void ASM_2()
        {   
            var person = new Person { Name = "John", Age = 30 };
            var nameGetter = CreatePropertyGetter<Person, string>("Name");
            var v = nameGetter(person);
            Console.WriteLine($"Name: {v}");
        }

        [TestMethod]
        public void ASM_1()
        {
            var method = new DynamicMethod("Add", typeof(int), new[] { typeof(int), typeof(int) });
            ILGenerator il = method.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // Push arguments (arg0 and arg1) to the stack
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Add); // Add them
            il.Emit(OpCodes.Ret); // Return the result

            // Create a delegate and invoke it
            var add = (Func<int, int, int>)method.CreateDelegate(typeof(Func<int, int, int>));
            Assert.AreEqual(30, add(10, 20));
        }
    }
}
