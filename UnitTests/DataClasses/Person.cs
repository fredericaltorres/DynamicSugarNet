using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicSugarSharp_UnitTests {

    public class TypeTestClass
    {
        public string LastName;
        public string FirstName { get; set; }
        public int Age { get; set; }
        public DateTime BirthDay { get; set; }

        private string PrivateTitle { get; set; } = "privateSomething";

        public List<string> ListOfString1 = new List<string>();
        public List<string> ListOfString2 { get; set; } = new List<string>();

        public Dictionary<string, string> DictionaryOfString1 = new Dictionary<string, string>();
        public Dictionary<string, string> DictionaryOfString2 { get; set; } = new Dictionary<string, string>();

        public Stack<string> StackOfString2 { get; set; } = new Stack<string>();

    }

    public class TestClassAllType
    {
        public int IntValue { get; set; } = 123;
        public long LongValue { get; set; } = 123;
        public double DoubleValue { get; set; } = 123.456;  
        public decimal DecimalValue { get; set; } = 123.456m;
        public DateTime DateTimeValue { get; set; } = new DateTime(1964, 12, 11, 0, 0 , 0);
        public string StringValue { get; set; } = "Hello";
        public bool BoolValue { get; set; } = true;
        public char CharValue { get; set; } = 'A';
        public byte ByteValue { get; set; } = 123;
        public short ShortValue { get; set; } = 123;
        public float FloatValue { get; set; } = 123.456f;
        public Single SingleValue { get; set; } = 123.456f;
        public uint UIntValue { get; set; } = 123;
        public ulong ULongValue { get; set; } = 123;    
        public ushort UShortValue { get; set; } = 123;  
        public sbyte SByteValue { get; set; } = 123;
        public Guid GuidValue { get; set; } = Guid.Parse("A4E7E546-D75C-4B4C-B717-EC0D66085CA0");
        public TimeSpan TimeSpanValue { get; set; } = TimeSpan.FromDays(1);
        public DateTimeOffset DateTimeOffsetValue { get; set; } = new DateTimeOffset(new DateTime(1964, 12, 11, 0, 0, 0), new TimeSpan(0, 1, 0));
        public Uri UriValue { get; set; } = new Uri("http://www.flogviewer.com");
    }

    public class Address {
        public string Street;
        public string ZipCode;
        public string State;
    }
    /// <summary>
    /// A test class
    /// </summary>
    /// 
    public class Person {

        public string LastName;
        public string FirstName  { get; set; }
        public int Age           { get; set; }
        public DateTime BirthDay { get; set; }


        public static string PublicStaticTitle = "PublicStaticTitle";
        private static string PrivateStaticTitle = "PrivateStaticTitle";
        private string PrivateTitle { get; set; } = "privateSomething";

        public Address Address = new Address();

        public List<string> DrivingLicenses = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {

            return this.Format("LastName:{LastName}, FirstName:{FirstName}, Age:{Age}, BirthDay:{BirthDay}, DrivingLicenses:{DrivingLicenses}");
        }
        /// <summary>
        /// Implemented to test calling method with no parameter
        /// </summary>
        /// <returns></returns>
        public string GetLastName() {

            return this.LastName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetUniqueID(){

            return this.Format("#{LastName}.{FirstName}.{BirthDay:yyyy}");
        }
        /// <summary>
        /// Define how to determine if the person are the same.
        /// Implementing this method is required to use the extension 
        /// method In().
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {

            Person p = obj as Person;
            return (this.LastName==p.LastName) && (this.FirstName==p.FirstName) && (this.BirthDay==p.BirthDay);
        }
    }
}
