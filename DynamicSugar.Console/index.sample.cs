using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using DynamicSugar;
using System.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace N {  
public class samples {

static void DoSomething(List<int> l){

}
static void main(string [] args) {
        
    DoSomething(new List<int>() { 1, 2, 3 });   // Regular C#
        
    DoSomething(DS.List( 1, 2, 3 ));            // Dynamic Sugar Syntax
}                    
              
static void Sample2(string [] args) {

var i = 3;

var Values = new List<int>() { 1, 2, 3 };   // Regular C#
if(Values.Contains(i))
    Console.WriteLine("in the list");            

if(i.In(1,2,3))                             // Dynamic Sugar Syntax
    Console.WriteLine("in the list");                    

}

void MyFunction(IDictionary<string, object> d){

}
void Sample3(){

    MyFunction(new Dictionary<string,object>() {    // Regular C#
                { "Debug"    , true            }, 
                { "Continent", "North America" }
             });

    // Dynamic Sugar Syntax
    MyFunction( DS.Dictionary( new { Debug=true, Continent="North America" } ) ); 
}

void MyFunction( IDictionary<string, bool> d){

}
void Sample4(){

    MyFunction(new Dictionary<string, bool>() { // Regular C#    
                { "Debug"  , true  }, 
                { "Verbose", false } 
             });

    // Dynamic Sugar Syntax
    MyFunction( DS.Dictionary<bool>( new { Debug=true, Verbose=false } ) );
}

// Returning anonymous types from a method and accessing members without reflection.

void Sample5(){

    var firstName = "Fred";
    var age       = 45;
    
    var s1 = String.Format("firstName:{0}, age:{1}", firstName, age ); // Regular C#

    // Dynamic Sugar Syntax. Is that beautiful?
    var s2 = "firstName:{firstName}, age:{age}".Format( new { firstName , age } );
}
    

class Person {

    public string   Name { set; get; }
    public int      Age  { set; get; }   

    public  string Format(string format, params object[] args) {

        return DynamicSugar.ExtendedFormat.Format(this, format, args);
    }
}
void Sample6(){

    var p = new Person() { Name = "Fred", Age = 45 };

    Console.WriteLine( String.Format( "Name={0}, Age={1:000}", p.Name, p.Age ) );    // Regular C#
    
    Console.WriteLine( p.Format( "Name={Name}, Age={Age:000}" ) );                  // Dynamic Sugar Syntax
}

    
public static void Sample6_2(){
    
    var v1  = DS.List(1, 2, 3).Format();                    // 1, 2, 3
    var v2  = DS.List(1, 2, 3).Format( @"""{0}""", "; " );  // "1"; "2"; "3"

    var v3  = DS.List("a", "b", "c").Format();                  // "a", "b", "c"
    var v4  = DS.List("a", "b", "c").Format( @"({0})", "; " );  // ("a"); ("b"); ("c")

    var beatles = DS.Dictionary<int>( new { Paul=1942, John=1940, Richard=1940, George=1943 } );
    var v5 = beatles.Format(); // { Paul:1942, John:1940, Richard:1940, George:1943 }
    var v6 = beatles.Format(@"""{0}""=""{1}""", "; "); // { "Paul"="1942"; "John"="1940"; "Richard"="1940"; "George"="1943" }
}

// Regular C#
static bool ComputeValues(int value, out double amount, out string message){

    amount  = 2.0 * value;
    message = "Hello";
    return true;
}
// Dynamic Sugar Syntax
static dynamic ComputeValues(int value){

    return DS.Values( new { returnValue = true, amount = 2.0 * value, message = "Hello" } );
}
void Sample7(){

    // Regular C#
    string message;
    double amount;
    var returnStatus = ComputeValues( 2, out amount, out message );
    Console.WriteLine( "returnStatus:{0}, amount:{1}, message:{2}", returnStatus, amount, message );

    // Dynamic Sugar Syntax
    var r = ComputeValues( 2 );
    Console.WriteLine("returnStatus:{0}, amount:{1}, message:{2}", r.returnValue, r.amount, r.message );
}

void Sample8(){

    for(int i=0; i<10; i++)         // Regular C#
        Console.WriteLine(i);

    foreach(var i in DS.Range(10))  // Dynamic Sugar Syntax
        Console.WriteLine(i);
}


public static void Sample9(){
        
    var v1  = DS.List(1, 2, 3).Add( DS.List( 4, 5, 6 ) );
    var v2  = DS.List(1, 2, 3).Clone();
    var v3  = DS.List(1, 2, 3).Filter( e => e % 2 == 0 );      // Same as FindAll() for IEnumerable<>
    var v5  = DS.List(1, 2, 3).Identical( DS.List( 1, 2, 3 ) );
    var v6  = DS.List(1, 2, 3).Include( DS.List( 2, 3 ) );
    var v7  = DS.List(1, 2, 3).Include( 2, 3 );
    var v8  = DS.List(1, 2, 3).Intersect( DS.List( 3, 4, 5) ); // Same as Intersect() for IEnumerable<>
    var v9  = DS.List(1, 2, 3).IsNullOrEmpty();
    var v10 = DS.List(1, 2, 3).Map( e => e * 2 );              // Same as Select() for IEnumerable<>    
    var v11 = DS.List(1, 2, 3).Merge( DS.List(3, 4, 5) );
    var v12 = DS.List(1, 2, 3).Reject( e => e % 2 == 0 );
    var v13 = DS.List(1, 2, 3).Substract( DS.List( 3, 4, 5) );
              DS.List(1, 2, 3).ToFile(@"C:\MyFile.txt");
    var v15 = DS.ListHelper.FromFile<int>(@"C:\MyFile.txt");
    var v16 = DS.List(1, 2, 3).Without( DS.List(2, 3) );
    var v17 = DS.List(1, 2, 3).Without( 2, 3 );
}
    
    


void Sample10(){

    var v1  = DS.List(1, 2, 3);
    while(!v1.IsEmpty()) {

        Console.WriteLine("{0} - {1}", v1.First(), v1.Last());
        v1 = v1.Rest();
    }    
}


public void Sample11() {

    var s = "ABCD";
    Assert.AreEqual("ABCD"  , s.Slice(0    ));
    Assert.AreEqual("BCD"   , s.Slice(1    ));
    Assert.AreEqual("AB"    , s.Slice(0 , 2));
    Assert.AreEqual("BC"    , s.Slice(1 , 2));
    Assert.AreEqual("DCBA"  , s.Slice(-1   ));
    Assert.AreEqual("CB"    , s.Slice(-2, 2));            
}

public static void Sample12() {

    var s1 = "ABCD";
    var s2 = s1.Reverse();
    var s3 = ";".Join(DS.List(1, 2, 3));
    var s4 = s2.IfNullOrEmpty("default");
    
    if(s1.IsNullOrEmpty()){

    }
}

public static void Sample13() {

    var beatles      = DS.Dictionary<int>( new { Paul=1942, John=1940, Richard=1940, George=1943 } );
    var other        = beatles.Get("Pete", 1941); // Support a default value for non existing key
    
    var younger1     = beatles.Max();
    var younger2     = beatles.Max(DS.List("Paul", "John", "Richard"));

    var older1       = beatles.Min();
    var older2       = beatles.Min(DS.List("Paul", "Richard", "George"));

    var newMusicians = beatles.Clone();

    var stones       = DS.Dictionary<int>( new { Mick=1943, Keith=1943, Brian=1942, Bill=1936,  Charlie=1941 } );
    var allTogether  = beatles.Add(stones);
    var noStones     = allTogether.Substract(stones);

    var b1           = beatles.Include(beatles);
    var b2           = beatles.Include("Paul");
    var b3           = beatles.Include( new { Paul=1942, John=1940 } );
}


    public static void MyFunction2(int i, string s, DateTime d) {

    var parameters = ReflectionHelper.GetLocals(i, s, d);
    var v1 = ReflectionHelper.GetLocals(i, s, d).Format(); // { i:1, s:"A", d:"4/21/2011 10:23:13 PM" }

}
public static void Sample14() {

    MyFunction2( 1, "A", DateTime.Now );
}

}
}
