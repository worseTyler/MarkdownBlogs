---
title: "Registering a string of code as an apparent \"delegate\" listener"
date: "2005-05-11"
categories: 
  - "net"
  - "blog"
tags: 
  - "net"
---

In my previous post I described how to provide custom validation code at runtime.  I register the code through a call to RegisterNameValidationCode().  One variation on this is to provide a += operator to register the code so that it looks as though I am registering a delegate.

> bool exceptionFired \= false; Person person \= new Person();
> 
> **person.NameValidationCode +=** **@"public static void OnNameChanged(object sender, PropertyValidation.OnBeforeNameChangeEventArgs args) { if (args.After == null) { throw new System.ArgumentNullException(); } }";** try { person.Name \= null; } catch(ArgumentNullException) { exceptionFired \= true; } Trace.Assert(exceptionFired);

As shown, registering the validation code just as one would a delegate, you use the += operator:

To support this, I defined a new type, ValidationCode:

> public class ValidationCode { public string Code; public ValidationCode(string code) { Code \= code; }
> 
> **public static string operator +(ValidationCode code) { return code.Code; } public static implicit operator ValidationCode(string code) { return new ValidationCode(code); }** }

They type includes operator overloading for an implicit cast and the + operator.  You can't override any assignment operators including the += operator.  However, if the assignment includes a cast then you can effectively overload the assignment operator by providing an implicit cast.

Next, I add ValidationCode as a property on Person:

> public class Person { // ... public ValidationCode NameValidationCode { set { \_NameValidationCode \= value; **RegisterNameValidationCode("OnNameChanged", value.Code);** } get { return \_NameValidationCode; } } private ValidationCode \_NameValidationCode; }

Although this allowed for calling code (see the top listing in this post) that looks just like delegate registration would look with a operator +=.  Note that there is no intellisense indicating that a string can be assigned, not just a ValidationCode instance.  In other words, this would not be an intuitive way to code the registration.  The other drawback, is that I hard code the method name inside the NameValidationCode property.  Alternatives would be to parse the code and retrieve the method name or simply not provide the method declaration in the string at all, and rather inject it before compiling.

(The full source code is provided in the previous post.)
