DynamicSugar.net /Standard
===============

## What is DynamicSugar.Net?
* In 2011 I created the DynamicSugar.net Library which 
provides methods and classes inspired by the dynamic 
languages Python and JavaScript to write shorter and more readable source code in C# 4.0.

* From 2011 to 2020, the C# language evolved and some features in DynamicSugar became obsolete, like
the DynamicSugar string interpolation feature. But most of the features are still useful and I want 
to use them in my .NET 4.6.1 and .NET Core development in 2023.

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
var l = l1.Without(0, 2, 4, 6, 8);
var l = l1.Without(DS.List(0, 2, 4, 6, 8));
var b = l1.IsEmpty();
var b = l1.IsNullOrEmpty();
var l = DS.List(1, 2, 3).Filter( e => e % 2 == 0 );
var l = DS.List(1, 2, 3).Merge( DS.List(3, 4, 5) );
var l = DS.List(1, 2, 3).Substract( DS.List( 3, 4, 5) );
var l = DS.List(1, 2, 3).Intersect( DS.List( 3, 4, 5) );
var l = DS.List(1, 2, 3).Identical( DS.List( 1, 2, 3 ) );
var l = DS.List(1, 2, 3).Reject( e => e % 2 == 0 );
var l = DS.List(1,2,3,4).Map( e => e * e );

// Format
var l1  = DS.List(1,2,3);
Assert.AreEqual(@"1, 2, 3", DS.List(1, 2, 3).Format());
Assert.AreEqual(@"[1]:[2]:[3]", DS.List(1, 2, 3).Format("[{0}]", ":"));

// Clear syntax
int i = 1;
if(i.In(1,2,3)) {
  // ...
}

var l = DS.List(1,2,3);
if(i.In(l)) {
  // ...
}

// Other Types
var l = DS.Array(1, 2, 3);
var l = DS.Queue(1, 2, 3);
var l = DS.Stack(1, 2, 3);
```

## String Processing
```csharp

// Deprecated since we have now string interpolation, but still works
var LastName = "TORRES";
var Age      = 45;
var s1 = "LastName:{LastName}, Age:{Age:000}".Template( new { LastName, Age } );

// Template
s1 = "LastName:[LastName], Age:[Age]".Template(new { LastName, Age }, "[", "]");

``````

## Dictionary
```csharp

// Quick and clean way to create dictionary of <string, object>
var dic = DS.Dictionary(new { i = 1,  f = 1.1f , s = "string", b = true });

// Get all the properties of one POCO into a Dictionary
var dic = DS.Dictionary(TestDataInstanceManager.TestPersonInstance);
Assert.AreEqual("TORRES", dic["LastName"]);
Assert.AreEqual(45, dic["Age"]);

// Include private property
var dic = DS.Dictionary(TestDataInstanceManager.TestPersonInstance, isPrivate: false);

// Clone
var d2 = ReflectionHelper.CloneDictionary<string, object>(d);

// Dictionary includes sub Dictionary 
var dic1 = DS.Dictionary(new { a = 1, b = 2, c = 3, d = 4, e = 5 });
Assert.IsTrue(dic1.Include(dic1));

// Format
var dic = DS.Dictionary(new { i = 1,  f = 1.1f , s = "string", b = true});
Assert.AreEqual(@"{ i:1, f:1.1, s:""string"", b:True }", dic.Format());

// Custom format
var a = dic.Format("{0} ~ {1}", ",", "<", ">");
Assert.AreEqual(@"<i ~ 1,f ~ 1.1,s ~ ""string"",b ~ True>", dic.Format("{0} ~ {1}", "," , "<", ">"));

var d1 = DS.Dictionary( new { a=1, b=2, c=3 } );
Assert.IsTrue(DS.DictionaryHelper.Identical<string,object>(d1,d1));

// Get a dictionary of the properties types
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

``````

``````

## String methods
```csharp

// Removing comment C like comment
var result = $"[/* comment */]".RemoveComment(commentType: ExtensionMethods_Format.StringComment.C);

// Removing comment Python and Powershell like comment
var result = @"print(""Hello World"") # a comment".RemoveComment(commentType: ExtensionMethods_Format.StringComment.Python);

// Removing comment SQL like comment
var result = @"print(""Hello World"") -- a comment".RemoveComment(commentType: ExtensionMethods_Format.StringComment.SQL);

``````

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
Install-Package DynamicSugarCore
```

# Platforms: 
* Microsoft Windows and Windows Phone, .NET v 4.x - 2011 - [Source](https://github.com/fredericaltorres/DynamicSugarNet)
* Xamarin iOS and Android - 2011
* Dot Net Standard 2.0 - 2019
