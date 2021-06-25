---
title: "Unsuccessfully Defining a Wrapper for Debug.Assert()"
date: "2005-10-23"
categories: 
  - "net"
  - "blog"
tags: 
  - "net"
---

In my previous post, I mentioned how Debug.Assert() is nice in debug builds but that in release builds it should be replaced by throwing an exception if the conditional is false.  It seems fairly obvious to define a wrapper around Debug.Assert() as follows:

> void Assert(bool condition) { if(condition) { System.Diagnostics.Debug.Assert(condition, message); // Throw exception... } }

The problem comes in the commented out code.  Presumably, it should be possible to define the exception type that is thrown in the call to Assert().  However, we don't want to instantiate an exception if we are not going to get throw it.  Instead, it seems a better solution would be to supply the exception type using generics as follows:

> void Assert<T>(bool condition, string message) where T : System.Exception, new() { if(condition) { System.Diagnostics.Debug.Assert(condition, message); // 1.  ERROR: T cannot provide objects in instantiation //     of type parameters // throw new T(message);
> 
> // 2. ERROR: Exception.Message is readonly // T exception = new T(); // exception.Message = message } }

The problem with this code is that:

1. The constructor constraint (new()) only works on default (parameterless) constructors.
    
2. exception.Message is readonly.
    

In conclusion, unless the conditional of the assert is expensive, don't use Debug.Assert(), simply throw an exception if the conditional is false.
