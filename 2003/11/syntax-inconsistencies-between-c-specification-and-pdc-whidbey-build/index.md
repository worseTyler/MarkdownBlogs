---
title: "Syntax Inconsistencies between C# Specification and PDC-Whidbey Build"
date: "2003-11-05"
categories: 
  - "net"
  - "blog"
tags: 
  - "net"
---

The C# 2.0 draft specification was published by Microsoft shortly before the PDC.  Then, at the PDC developers received a copy of Whidbey, which included the C# 2.0 compiler.  However, there are a three noticeable inconsistencies between the C# 2.0 specification and the compiler itself that deserve mentioning:  Some of the mismatches are not actually in the draft specification but they can be gleaned from Anders Hejlsberg's presentation on C# Language Enhancements at the PDC.  Obviously updates will be made but the specification is a moving target and until it is approved, there are no guarantee of compliance.

1. When writing iterators the C# specification uses the syntax yield return.  However, the PDC version of the C# compiler requires the yield keyword on its own, without the return.  The reason for the change is that by including the return keyword after yield, it was not necessary to add a new keyword to the language since return is already a keyword.  By placing yield prior to return, yield is considered a contextual keyword which is significantly different from a regular keyword.  (partial, another C# 2.0ism, is also a contextual keyword.)
    
    public struct Pair<T>: IEnumerable<T>
    
    {
    
    ...
    
    #region IEnumerable<T>
    
    public IEnumerator<T> GetEnumerator()
    
    {
    
    yield return First;
    
    yield return Second;
    
    }
    
    #endregion IEnumerable<T>
    
    }
    
2. In the C# specification it indicates you can have mismatched access modifiers on properties (something not allowed prior to C# 2.0).  For example: public string Name { get { return \_name; } internal set { \_name = value; } } private string \_name;
3. Namespace alias qualifiers: using IO = System.IO; class Program { static void Main() { IO::Stream s = new IO::File.OpenRead("foo.txt"); global::System.Console.WriteLine("Hello"); } }
