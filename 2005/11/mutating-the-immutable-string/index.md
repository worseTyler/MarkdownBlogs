---
title: "Mutating the Immutable String"
date: "2005-11-20"
categories: 
  - "net"
  - "blog"
tags: 
  - "net"
---

During my .NET Internals presentation at the Seattle code camp and again the [Spokane .NET User Group](https://www.meetup.com/Spokane-NET-User-Group/), I presented attendees with the following question:

> What code needs to be added between the ellipses below, and only between the ellipses below, to cause the output shown?
> 
> class Program
> {
>   static void Main()
>   {
>       string text;
> 
>       // ...
>       // Place code here
>       // ...
> 
>       text = "S5280ft";
>       System.Console.WriteLine(text);
>   }
> }
> 
> \>Program.exe
> 
> **Smile**

UPDATE - 11/20/2005

Perhaps one of the first lessons about .NET is that string is immutable - it cannot (generally) be modified.  The clue to my intended solution, however, is in the title.   As Austin correctly answered, I mutate the string.

> class Program
> {
>   static void Main()
>   {
>       string text;
>       text = "S5280ft";
> 
>       unsafe
>       {
>           fixed (char\* pText = text)
>           {
>               pText\[1\] = 'm';
>               pText\[2\] = 'i';
>               pText\[3\] = 'l';
>               pText\[4\] = 'e';
>               pText\[5\] = ' ';
>               pText\[6\] = ' ';
>           }
>       }
> 
>       text = "S5280ft";
>       System.Console.WriteLine(text);
>   }
> }

Unsafe code and pointers alone do not suffice, however.  The additional factor is that the C# compiler is clever enough not to duplicate the same string literal within the code.  In other words, all references to "S5280ft" are to the same address and modifying the characters at that location, modifies the data for all the references.

(My upcoming Essential C# book generally limits obtuse code like this to _Expert Note blocks_. Targeted at the more experienced developer, these blocks are designed to provide in-depth details about a particular topic so that even veteran developers will find the book interesting - not only to solidify best practices, but also to drill down to the next level of expertise.)
