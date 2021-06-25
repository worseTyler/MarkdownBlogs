---
title: "Notes on some C# 3.0 features that make LINQ possible"
date: "2005-09-13"
categories: 
  - "net"
  - "blog"
tags: 
  - "net"
---

**Implicit Type Declarations**

- Variables can be declared as type var. var items = new List<int>;
- var variables must be initialized at declaration time. // var moreItems;   ERROR:  Must be initialized
- This is mostly serves as a convenience, especially when using generics and having really long type names.
- The data type of var is resolved at compile time.  Given the local variable declaration of items above, the corresponding CIL code uses \[0\] class \[mscorlib\]System.Collections.Generic.List\`1<int32> items
- var is only for local variables to avoid any chance of anonymous types being exposed outside of the method in which they are declared.
- var is a contextual keyword only.
- var is the only variable "type" that can be assigned an anonymous type

**Extension Methods**

- Extension methods allow the addition of methods to existing classes (even sealed classes).  For example:

> > using System.IO;
> > 
> > public static class FileInfoEx { **public static string GetFileContent(this FileInfo file)** { StreamReader reader = file.OpenRead(); return reader.ReadToEnd(); } }
> > 
> > public static class Program { static void Main() { FileInfo file = new FileInfo("data.txt"); string data = **file.GetFileContent(file);** Console.WriteLine(data); } }

- The class containing the extension method must be static.
- It is not possible to define extension methods that will show up as static methods on the extended class.  For example, it is not possible to define extension methods for System.IO.Path or any static class.

**Lambda Expressions**

- With lambda expressions, there is a replacement for a limited subset of anonymous method declarations:

> > public delegate bool Comparison<T>(T first, T second); class Sorter { public static void BubbleSort<T>( IList<T> items, Comparison<T> comparison) { // ... } }
> > 
> > class Program { public static void Main() { // ...
> > 
> > BubbleSort<int\>(items, **(int first, int second) => first > second**); } }

- The delegate for the lambda expression must have a return type (it cannot be void).
- If the data type is implied and there is only a single parameter, then the parenthesis are optional items = Filter<int>(items, item => item > 42);
- Multiple parameters need to be placed into parenthesis and separated by a comma.
- The data type on the parameter name of the lambda expression may be inferred.  In the above example, int is not required on first and second.

**PDC Tech. Preview Bugs**

- It is not possible to call extension methods that overload methods in the original class.  You can define such methods but it is not possible to call them as extension methods.  Using the above code, for example, it is possible to define public static void CopyTo(this FileInfo file, params string\[\] destFileNames){} However, it is not possible to call this method as an extension method.
- var is treated as a keyword rather than a contextual keyword.

Thanks Mike for pairing with me as we discovered this.  Thanks also to the C# team (Mads, [Anson](https://blogs.msdn.com/ansonh/), and Alex) for answering my questions.
