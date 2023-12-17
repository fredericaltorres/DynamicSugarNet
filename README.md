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
// Creating quick lists
var intList   = DS.List(1,2,3);
var intString = DS.List("a","b");
var l1        = DS.Range(10);

// New list methods
var b = l1.Include(5);
var l2 = l1.Without(0, 2, 4, 6, 8);
var l3 = l1.Without(DS.List(0, 2, 4, 6, 8));
var b = l1.IsEmpty();
Console.WriteLine(l2.Format()); // => 1, 3, 5, 7, 9
Console.WriteLine(l3.Format()); // => 1, 3, 5, 7, 9

var l = DS.List(1,2,3,4).Map( e => e*e );

// Clear syntax
int i = 1;
if(i.In(1,2,3)) {
  // ...
}

var l = DS.List(1,2,3);
if(i.In(l)) {
  // ...
}

```

## String Processing
```csharp

// Deprecated since we have now string interpolation
var LastName = "TORRES";
var Age      = 45;
var s1 = "LastName:{LastName}, Age:{Age:000}".Template( new { LastName, Age } );

// Template
s1 = "LastName:[LastName], Age:[Age]".Template(new { LastName, Age }, "[", "]");

``````

## Reflection Dictionary
```csharp

// Quick way to create a dictionary of <string, object>
var dic = DS.Dictionary(new { i = 1,  f = 1.1f , s = "string", b = true });

// Get all the properties of one POCO into a Dictionary
var dic = DS.Dictionary(TestDataInstanceManager.TestPersonInstance);
Assert.AreEqual("TORRES", dic["LastName"]);
Assert.AreEqual("Frederic", dic["FirstName"]);
Assert.AreEqual(45, dic["Age"]);
Assert.AreEqual(new DateTime(1964, 12, 11), dic["BirthDay"]);

// Include private property
var dic = DS.Dictionary(TestDataInstanceManager.TestPersonInstance, isPrivate: false);

// Clone
var d2 = ReflectionHelper.CloneDictionary<string, object>(d);

// Format
var dic = DS.Dictionary(new { i = 1,  f = 1.1f , s = "string", b = true});
Assert.AreEqual(@"{ i:1, f:1.1, s:""string"", b:True }", dic.Format());

var dsi = new Dictionary<string, int>();
Type keyType, valueType;
ReflectionHelper.GetDictionaryType(dsi.GetType(), out keyType, out valueType);
Assert.AreEqual(typeof(string), keyType);
Assert.AreEqual(typeof(int), valueType);

``````

## Reflection List and Property
```csharp

// Just getting one property, including static property though you need to pass an instance
var lastName = ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "LastName").ToString();

var b = ReflectionHelper.MethodExist(TestDataInstanceManager.TestPersonInstance, "GetLastName");

var privateTitle = ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "PrivateTitle", isPrivate: true);

// Retrieve the type of a generic list or dictionary
var li = new List<int>();
Assert.AreEqual(typeof(int), ReflectionHelper.GetListType(li.GetType()));

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
