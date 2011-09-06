using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicSugar;
using System.Web.Razor;
using System.Dynamic;

namespace DynamicSugar.ConsoleApplication {

    class Program {        

        static void Main(string[] args) {

            N.samples.Sample6_2();

            Console.WriteLine("Dynamic Sugar # Library\r\n");
            Why();
            Why2();            
            In();
            FormatMethodSample();
            FormatMethodWithDictionarySample();
            FormatMethodWithExpandoObjectSample();
            DictMethodSample();
            RangeSample();
            List_Map();
            List_Inject();
            List_Filter();
            MultiValuesSample();
            ToFile_FromFile();
            Include();
            Without();
            First_Last_Rest();
            Pluck();
            Reject();
            Pause();
        }          
        static void Why2(){

            // Regular Syntax
            var LastName = "TORRES";
            var Age      = 45;
            var s1       = String.Format("LastName:{0}, Age:{1:000}", LastName, Age);

            // First syntax with DynamicSugar
            LastName = "TORRES";
            Age      = 45;
            s1       = "LastName:{0}, Age:{1:000}".format(LastName, Age);

            // Second syntax with DynamicSugar            
            s1       = "LastName:{LastName}, Age:{Age:000}".Format( new { LastName, Age } );
        }
        static void Why(){

            // Syntax 1, with regular C#
            List<int> someIntegers = new List<int>() { 1, 2 ,3 };
            int i                  = 2;
            if(someIntegers.Contains(i)){

                Console.WriteLine("2 is in the list");
            }

            // Syntax 2, with regular C#
            i = 2;            
            if((new List<int>() { 1, 2 ,3 }).Contains(i)){

                Console.WriteLine("2 is in the list");
            }

            // Syntax DynamicSugar
            i = 2;            
            if(i.In(DS.List(1, 2 ,3))){

                Console.WriteLine("2 is in the list");
            }
        }

        enum CustomerType {
            Good,
            Bad,
            SoSo
        }
        static void In(){

            int i = 1;

            if(i.In(1,2,3)){

            }

            List<int> l = DS.List(1,2,3);

            if(i.In(l)){

            }

            var state = "good";

            if(state.In("good","bad","soso")){

            }

            var customerType = CustomerType.Good;
            if(customerType.In(CustomerType.Good, CustomerType.SoSo)){

            }

            var people = DS.List(
                new Person() { LastName = "Descartes",   FirstName = "Rene", BirthDay = new DateTime(1596,3,31)},
                new Person() { LastName = "Montesquieu", FirstName = "Charles-Louis", BirthDay = new DateTime(1689,1,18)},
                new Person() { LastName = "Rousseau",    FirstName = "JJ", BirthDay = new DateTime(1712,3,31)}
            );
            var Descartes = new Person() { LastName = "Descartes",   FirstName = "Rene", BirthDay = new DateTime(1596,3,31)};

            if(Descartes.In(people)){
                
                Console.WriteLine("In the list people");
            }
            else{
                Console.WriteLine("Not in the list people");
            }
        }
        static void RangeSample(){

            
            foreach(var i in DS.Range(5)){

                Console.WriteLine(i);
            }
        }
        static void List_Map(){

            var l = DS.List(1,2,3,4).Map( e => e*e );
            Console.WriteLine(l.Format()); // => 1, 4, 9, 16

            var l2 = DS.ListHelper.Map( DS.Range(10), e => e*e );
            Console.WriteLine(l2.Format("'{0}'",",")); // => '0','1','4','9','16','25','36','49','64','81'
            
        }
        static void List_Inject(){

             // Sum the values from 0 to 9
            var l1 = DS.Range(10).Inject( (v,e) => v += e ) ;
            var l2 = DS.Range(10).Aggregate((v,e) => v += e ) ;
            
            Console.WriteLine( l1 );
            Console.WriteLine( l2 );
        }
        static void List_Filter(){
            
            var l = DynamicSugar.DS.Range(10).Filter(e => e % 2 == 0);
            Console.WriteLine( l.Format() );
        }   

        // Here is a function returning 3 values
        static dynamic ComputeValues() {
                        
            return DS.Values( new { a=1, b=2.0, c="Hello" } );
        }
        // Here is how to use the results
        static void MultiValuesSample() {
                        
            var values = ComputeValues();
            Console.Write(values.a);
            Console.Write(values.b);
            Console.Write(values.c);
        }
        
        static void FormatMethodSample(){

            var firstName = "Fred";
            var age       = 45;

            var s1 = String.Format("firstName:{0}, age:{1}", firstName, age);                                 

            // Dynamic Sugar Syntax
            var s2 = "firstName:{firstName}, age:{age}".Format( new { firstName , age } );

            Console.WriteLine(s1);
            Console.WriteLine(s2);

            Person p = new Person() {

                LastName        = "TORRES"  ,
                FirstName       = "Frederic",
                BirthDay        = new DateTime(1964,12, 11),
                DrivingLicenses = DS.List("Car", "Moto Bike")
            };
            //DrivingLicenses = new List<string>() { "Car", "Moto Bike" }
            Console.WriteLine( // Call 3 properties in the format
                p.Format("FullName:'{LastName},{FirstName}', BirthDay:{BirthDay:MM/dd/yyyy}, DrivingLicenses:{DrivingLicenses}")
            );
            Console.WriteLine( // Call a method in the format
                p.Format("LoadInformation:{LoadInformation()} ")
            );
        }
        static void FormatMethodWithDictionarySample(){

            var format = "LastName:{LastName}, FirstName:{FirstName}, Age:{Age:000}";
            var Values = new Dictionary<string, object>() {
                { "LastName" , "TORRES"  },
                { "FirstName","Frederic" },
                { "Age"      , 45        }
            };
            Console.WriteLine(ExtendedFormat.Format(format, Values));
        }
        
        static void FormatMethodWithExpandoObjectSample(){

                var format    = "LastName:{LastName}, FirstName:{FirstName}, Age:{Age:000}";
            
                dynamic bag   = new ExpandoObject() ;
                bag.LastName  = "TORRES";
                bag.FirstName = "Frederic";
                bag.Age       = 45;
            
                Console.WriteLine(ExtendedFormat.Format(format, bag));
        }

        static void DictMethodSample(){

            Person p = new Person() { 

                LastName  = "TORRES",
                FirstName = "Frederic",
                BirthDay  = new DateTime(1964,12, 11)
            };
            
            foreach(var k in p.Dict()){
                Console.WriteLine("{0}='{1}'", k.Key, k.Value);
            }
        }
        private static void Pause() {

            Console.WriteLine("Press Enter");
            Console.ReadLine();
        }
        private static void Print(string f, params object [] defines){

            Console.WriteLine(String.Format(f, defines));
        }
        private static void ToFile_FromFile() {

            var l1        = DS.Range(10);
            //var fileName  = String.Format(@"{0}\DSSharpLibrary_UnitTests.txt", Environment.GetEnvironmentVariable("TEMP"));
            var fileName  = String.Format(@"{0}\DSSharpLibrary_UnitTests.txt", System.IO.Path.GetTempPath());
            l1.ToFile(fileName, true);

            var l2        = DS.ListHelper.FromFile<int>(fileName);

            DS.ListHelper.AssertListEqual(l1, l2);            
        }
         private static void Include() {

            var l1 = DS.Range(10);

            if(l1.Include(5)){

            }
            if(l1.Include(1, 2, 3)){

            }
            if(l1.Include(DS.List(1, 2 , 3))){

            }            
        }
        private static void Without() {

            var l1 = DS.Range(10);            
            var l2 = l1.Without(0, 2, 4, 6, 8);
            var l3 = l1.Without(DS.List(0, 2, 4, 6, 8));
            Console.WriteLine(l2.Format()); // => 1, 3, 5, 7, 9
            Console.WriteLine(l3.Format()); // => 1, 3, 5, 7, 9
        }
        private static void First_Last_Rest() {

            var l1 = DS.Range(10);            

            while(!l1.IsEmpty()){

                var first = l1.First();
                var last  = l1.Last();
                l1        = l1.Rest();
            }
        }
        private static void Identical() {

            var l1 = DS.Range(10);            
            var l2 = DS.Range(10);            

            if(l1.Identical(l2)){

            }
        }
        public static void Pluck() {

            var people = DS.List(
                new Person() { LastName = "Descartes",   FirstName = "Rene",          BirthDay = new DateTime(1596,3,31) },
                new Person() { LastName = "Montesquieu", FirstName = "Charles-Louis", BirthDay = new DateTime(1689,1,18) },
                new Person() { LastName = "Rousseau",    FirstName = "JJ",            BirthDay = new DateTime(1712,3,31) }
            );
            Console.WriteLine(people.Pluck<int, Person>("Age").Format()); // => 415, 322, 299 in 2010          
        }
        public static void Reject() {
            // Remove some elements based on an lambda expression
            Console.WriteLine(
                      DS.Range(10).Reject(e => e % 2 == 0).Format()
                    ); // => 1, 3, 5, 7, 9
        }
    }
}
 