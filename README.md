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
```cssharp

var intList   = DS.List(1,2,3);
var intString = DS.List("a","b");
var l1        = DS.Range(10);

var b = l1.Include(5);
var l2 = l1.Without(0, 2, 4, 6, 8);
var l3 = l1.Without(DS.List(0, 2, 4, 6, 8));
var b = l1.IsEmpty();
Console.WriteLine(l2.Format()); // => 1, 3, 5, 7, 9
Console.WriteLine(l3.Format()); // => 1, 3, 5, 7, 9

var l = DS.List(1,2,3,4).Map( e => e*e );

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
```cssharp

var LastName = "TORRES";
var Age      = 45;
var s1 = "LastName:{LastName}, Age:{Age:000}".Template( new { LastName, Age } );

s1 = "LastName:[LastName], Age:[Age]".Template(new { LastName, Age }, "[", "]");

``````

## Reflection
```cssharp

var dic = DS.Dictionary(TestDataInstanceManager.TestPersonInstance);
Assert.AreEqual("TORRES", dic["LastName"]);
Assert.AreEqual("Frederic", dic["FirstName"]);
Assert.AreEqual(45, dic["Age"]);
Assert.AreEqual(new DateTime(1964, 12, 11), dic["BirthDay"]);

var dic = DS.Dictionary(TestDataInstanceManager.TestPersonInstance, isPrivate: false);

var lastName = ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "LastName").ToString();

var b = ReflectionHelper.MethodExist(TestDataInstanceManager.TestPersonInstance, "GetLastName");

var privateTitle = ReflectionHelper.GetProperty(TestDataInstanceManager.TestPersonInstance, "PrivateTitle", isPrivate: true);


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
