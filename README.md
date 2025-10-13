DynamicSugar.net /Standard
===============

## What is DynamicSugar.Net?
DynamicSugar.net a C# Library which provides methods and classes inspired by the dynamic languages
- Python 
- JavaScript
to write shorter and more readable source code in C#.

Created in 2011 for C# v 4.0 and .NET 4.0.

### Videos
- [List](https://ftorres.azurewebsites.net/fmsui?videoId=dynamicsugar2024-list-video)
- [Dictionary and Reflection](https://ftorres.azurewebsites.net/fmsui?videoId=dynamicsugar2024-dic-reflection-video)
- [String](https://ftorres.azurewebsites.net/fmsui?videoId=dynamicsugar2024-strings)

### 2023.

* From 2011 to 2020, the C# language evolved and some features in DynamicSugar became obsolete, like
the DynamicSugar string interpolation feature. But most of the features are still useful and I want 
to use them in my .NET 4.6.1 and .NET Core development in 2023 and 2024.

* In 2023 I moved the library to .NET Standard 2.0, so it can be used with .NET 4.6.1 and .NET Core.

![Logo ](DynamicSugarNet.50.png "Logo")

# Examples:
## List
```csharp
// Quick and clean way to create lists
var intList   = DS.List(1,2,3);
var intString = DS.List("a","b");
var l1        = DS.Range(10); // Range of number like in Python

// New list methods
var b = l1.Include(5);
var l = DS.List(1, 2, 3).Without( DS.List(3) );
var b = l1.IsEmpty();
var b = l1.IsNullOrEmpty();
var l = DS.List(1, 2, 3).Filter( e => e % 2 == 0 );
var l = DS.List(1, 2, 3).Merge( DS.List(3, 4, 5) );
var l = DS.List(1, 2, 3).Substract( DS.List(3, 4, 5) );
var l = DS.List(1, 2, 3).Intersect( DS.List(3, 4, 5) );
var l = DS.List(1, 2, 3).Identical( DS.List(1, 2, 3) );
var l = DS.List(1, 2, 3).Reject( e => e % 2 == 0 );
var l = DS.List(1,2,3,4).Map( e => e * e );
var l = DS.List(1,2,3,4).ToFile(@"c:\temp\list.txt", create: true);
var l = DS.List<string>().FromFile(fileName);
var l = DS.List<>(1,2,3).FromFile(fileName);

// Format
var l1  = DS.List(1,2,3);
Assert.AreEqual(@"1, 2, 3", DS.List(1, 2, 3).Format());
Assert.AreEqual(@"[1]:[2]:[3]", DS.List(1, 2, 3).Format("[{0}]", ":"));

// Cleaner syntax than Contains()
int i = 1;
if(i.In(1,2,3)) {
  // ...
}
var l = DS.List(1,2,3);
if(i.In(l)) {
  // ...
}

// Other Helpers
var l = DS.Array(1, 2, 3);
var l = DS.Queue(1, 2, 3);
var l = DS.Stack(1, 2, 3);
```


## Dictionary
```csharp

// Quick and clean way to create dictionary of <string, object>
var dic = DS.Dictionary(new { i = 1,  f = 1.1f , s = "string", b = true });

// Get all the properties of one POCO into a Dictionary
var dic = DS.Dictionary(TestDataInstanceManager.TestPersonInstance);
Assert.AreEqual("TORRES", dic["LastName"]);
Assert.AreEqual(45, dic["Age"]);

// Include private property
var dic = DS.Dictionary(TestDataInstanceManager.TestPersonInstance, allProperties: true);

// Clone
var d2 = ReflectionHelper.CloneDictionary<string, object>(d);

// Dictionary includes sub Dictionary 
var dic1 = DS.Dictionary(new { a = 1, b = 2, c = 3, d = 4, e = 5 });
Assert.IsTrue(dic1.Include(dic1));

// Merge
var dic1 = DS.Dictionary(new { a = 1, b = 2, c = 3 });
var dic2 = DS.Dictionary(new { d = 4, e = 5 });
var dic3 = dic1.Merge(dic2); // dic1 is not change

// Format
var dic = DS.Dictionary(new { i = 1,  f = 1.1f , s = "string", b = true});
Assert.AreEqual(@"{ i:1, f:1.1, s:""string"", b:True }", dic.Format());

// Custom format
var a = dic.Format("{0} ~ {1}", ",", "<", ">");
Assert.AreEqual(@"<i ~ 1,f ~ 1.1,s ~ ""string"",b ~ True>", dic.Format("{0} ~ {1}", "," , "<", ">"));

var d1 = DS.Dictionary( new { a=1, b=2, c=3 } );
Assert.IsTrue(DS.DictionaryHelper.Identical<string,object>(d1,d1));

// File
var d1 = DS.Dictionary( new { a=1, b=2, c=3 } );
d1.ToFile(@"c:\temp\dic.txt", create: true);

var d2 = new Dictionary<string, int>() { ["e"] = 100 };
d2.FromFile(@"c:\temp\dic.txt");

var d3 = (new Dictionary<string, int>()).FromFile(fileName);

// Get a dictionary of the properties types for a POCO
var testInstance = new TypeTestClass();
var dicType = DynamicSugar.ReflectionHelper.GetDictionaryWithType(testInstance);
Assert.AreEqual("String", dicType["LastName"]);
Assert.AreEqual("Int32", dicType["Age"]);

var dsi = new Dictionary<string, int>();
Type keyType, valueType;
ReflectionHelper.GetDictionaryType(dsi.GetType(), out keyType, out valueType);
Assert.AreEqual(typeof(string), keyType);
Assert.AreEqual(typeof(int), valueType);

``````

## Reflection and Property
```csharp

// Just getting one property, including static property though you need to pass an instance
var lastName = ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "LastName").ToString();

var b = ReflectionHelper.MethodExist(TestDataInstanceManager.TestPersonInstance, "GetLastName");

var privateTitle = ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "PrivateTitle", isPrivate: true);

// Retrieve the type of a generic list or dictionary
var li = new List<int>();
Assert.AreEqual(typeof(int), ReflectionHelper.GetListType(li.GetType()));

// Get or set indexer property
var f = new IndexerTestClass();
ReflectionHelper.SetIndexer(f, 2, 123);
Assert.AreEqual(2 * 123, ReflectionHelper.GetIndexer(f, 1));

``````



## String Processing
```csharp

// Deprecated since we have now string interpolation, but still works
var LastName = "TORRES";
var Age      = 45;
var s1 = "LastName:{LastName}, Age:{Age:000}".Template( new { LastName, Age } );

// Template
s1 = "LastName:[LastName], Age:[Age]".Template(new { LastName, Age }, "[", "]");

``````

## String methods
```csharp


Assert.AreEqual(123, "123".ToInt(defaultValue: -1));
Assert.AreEqual(new Guid("2E36429B-2695-4CB3-BCF2-9C7C6DC56B45"), "{2E36429B-2695-4CB3-BCF2-9C7C6DC56B45}".ToGuid());
Assert.AreEqual(new DateTime(1964, 12, 11), "12/11/1964".ToDateTime());
Assert.AreEqual(false, "false".ToBool(defaultValue: false));

Assert.AreEqual("Abcd", "abcd".Capitalize());
Assert.AreEqual("Abcd", "ABCD".Capitalize());

Assert.AreEqual("BCD", "ABCD".RemoveFirstChar());
Assert.AreEqual("ABC", "ABCD".RemoveLastChar());

Assert.AreEqual("terday I was here", "yesterday I was here".RemoveIfStartsWith("yes"));
Assert.AreEqual("yesterday I was ",  "yesterday I was here".RemoveIfEndsWith("here"));

var text = "A\r\nB\r\nC";
var lines = text.SplitByCRLF();
Assert.AreEqual(3, lines.Count);

var text = "A  \r\nB  \r\nC  ";
var lines = text.SplitByCRLF().TrimEnd();

var text = "A  \r\nB  \r\nC  ";
var lines = text.SplitByCRLF().TrimEnd().Indent("  ", skipFirstOne: true);
Assert.AreEqual(3, lines.Count);
Assert.AreEqual("A", lines[0]);
Assert.AreEqual("  B", lines[1]);
Assert.AreEqual("  C", lines[2]);

string s = "";
Assert.AreEqual("default", s.IfNullOrEmpty("default"));
string s = null;
Assert.AreEqual("default", s.IfNull("default"));

string s = null;
Assert.IsTrue(s.IsNullOrEmpty());

string s = "";
Assert.IsTrue(s.IsEmpty());

var s = "ABCD";
Assert.AreEqual("BCD" , s.Slice(1));
Assert.AreEqual("BC"  , s.Slice(1,2));

Assert.AreEqual("DCBA", "ABCD".Reverse());

// Removing comment C like comment
var result = $"[/* comment */]".RemoveComment(commentType: ExtensionMethods_Format.StringComment.C);

// Removing comment Python and Powershell like comment
var result = @"print(""Hello World"") # a comment".RemoveComment(commentType: ExtensionMethods_Format.StringComment.Python);

// Removing comment SQL like comment
var result = @"select * from customers -- a comment".RemoveComment(commentType: ExtensionMethods_Format.StringComment.SQL);

``````

## Unit Test Helper
```csharp

// Comparing the properties of 2 POCO
var o = new { a = 1, b = 2, c = "ok", d = true, e = DateTime.Now, f = 1.2, g = 1.2M, h = 1.2f };
DS.Assert.AreEqualProperties(o, o);

// Comparing 2 Dictionary<string, object> 
var o = new { a = 1, b = 2, c = "ok", d = true, e = DateTime.Now, f = 1.2, g = 1.2M, h = 1.2f };
DS.Assert.AreEqualProperties(DS.Dictionary(o), DS.Dictionary(o));

// Assert a string contains some sets of words
DS.Assert.Words("aa bb", "(aa | bb) & (bb | aa)");
```

## Resource files
```csharp

// Retreive the content of a text file embedded as a resource
var alphabet = DS.Resources.GetTextResource("Alphabet.txt", Assembly.GetExecutingAssembly());

// Retreive multiple content of a text files embedded as a resource
Dictionary<string, string> alphabetDic = DS.Resources.GetTextResource(new Regex("DataClasses.Alphabet", RegexOptions.IgnoreCase), Assembly.GetExecutingAssembly());

// Retreive the content of a GZip text file embedded as a resource
var text = DS.Resources.GetTextResource("DS_Compression.txt.gzip", Assembly.GetExecutingAssembly(), true, DS.TextResourceEncoding.UTF8);

// Retreive the content of bitmap embedded as a resource
byte [] b = DS.Resources.GetBinaryResource("EmbedBitmap.bmp", Assembly.GetExecutingAssembly());

```

## .NET GZip format, Zipping Unzipping file
```csharp

var gzipFilename = DynamicSugar.Compression.GZip.GZipFile(fileName);
var newTextFileName = DynamicSugar.Compression.GZip.UnGZipFile(gzipFilename);

GZip.GZipFolder(string path, string wildCard);

``````

# License:
You may use DynamicSugar.Net under the terms of the MIT License.
  
# NuGet
Install-Package DynamicSugarStandard

# Blog Posts:
How to Write a Spelling Corrector? From Python to C# with Dynamic Sugar 
  
# Screen Casts: 
* [Dynamic Sugar Demo](https://www.youtube.com/watch?v=aUDxnU4VY2s&feature=youtu.be)
* [The Dynamic Sugar Sharp Library's Format() Method](https://www.youtube.com/watch?v=ggFEs0JyM90)

# Platforms:
* Microsoft Windows and Windows Phone, .NET v 4.x
* Xamarin iOS and Android

***Outdated***
- The extension methods string.Format() and and string.Template() are now out dated,
since C# support string interpolation.
- The class MultiValues was an attempt to create functions returning multiple values, which is now supported by C#.

### A lot of the features are still useful see  [Examples](http://frederictorres.blogspot.com/2014/03/dynamicsugarnet.html)

# License:
You may use DynamicSugar.Net under the terms of the MIT License.
  
# NuGet
* TODO: Create NuGet Package and publish
```powershell
Install-Package DynamicSugarStandard
```

# Platforms: 
* Microsoft Windows and Windows Phone, .NET v 4.x - 2011 - [Source](https://github.com/fredericaltorres/DynamicSugarNet)
* Xamarin iOS and Android - 2011
* Dot Net Standard 2.0 - 2019

