---
title: "Fun Overloading with Generics"
date: "2005-02-18"
categories: 
  - "net"
  - "blog"
tags: 
  - "net"
---

Questions have to be answered before moving to the next question or else you may encounter an answer.

Question 1:

> Will the following code compile? Why or Why not?
> 
> class FunOverloadingWithGenerics { static void Swap(ref object left, ref object right) { // ... }
> 
> static void Main() { string first \= "Inigo Montoya"; string second \= "George Lucas";
> 
> Swap(ref first, ref second);
> 
> // ... } }

Question 2:

> How does the code in Question 1 need to be changed so that it compiles.  Assume no data types or method signatures can be changed and you are using a C# 1.0 compiler.

Question 3:

> Will the following code compile?  Why or Why not
> 
> class FunOverloadingWithGenerics { static void Swap(ref object left, ref object right) { System.Console.WriteLine("Object Swap() called."); // ... } static void Swap<T>(ref T param0, ref T param1) { System.Console.WriteLine("Generic Swap() called."); // ... }
> 
> static void Main() { string first \= "Inigo Montoya"; string second \= "George Lucas";
> 
> Swap(ref first, ref second);
> 
> // ... } }

Question 4:

> What will the output from the code in Question 3 be and why?
