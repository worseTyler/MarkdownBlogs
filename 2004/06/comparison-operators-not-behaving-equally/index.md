---
title: "Comparison Operators not Behaving Equally"
date: "2004-06-01"
categories: 
  - "net"
  - "blog"
tags: 
  - "net"
---

The current C# 2.0 specification includes the following quote:

> "A comparison operator (\==, !=, <, \>, <=, \>=) has a lifted form when the operand types are both non-nullable value types and the result type is bool. The lifted form of a comparison operator is formed by adding a ? modifier to each operand type (but not to the result type). Lifted forms of the \== and != operators consider two null values equal, and a null value unequal to a non-null value. Lifted forms of the <, \>, <=, and \>= operators return false if one or both operands are null."

What does this mean?

Perhaps the most significant concept in this paragraphs is at the end where it declares that the operators <= and <= versus the operator \== behave differently for Nullable<T> types when that have the value null.  As a result, even though \== may return true, the \>= operator and the <= operator will sometimes return false.  Let's consider an example.

> int? x, y;   [//](http:///) Declares two variables of type Nullable<int> x = null; y = null;
> 
> Assert.IsTrue(x == y); Assert.IsFalse(x <= y);

When null is involved with a nullable type, therefore, the >= operator would not be equivalent to the combination of the \> and \== operators.  In other words,  the expression x>=y would not be equivalent to the combination of x>y || x==y.  Perhaps what is most unusual about this is generally they operator >= is called greater-then-or-equal but in the case of both operands being null, the result of the >= operator would be not equal even though the \== operator indicates they are equal.

Furthermore if you were to sort a list of Nullable<T> types using the \> operator for ascending order and the < operator for descending order then regardless, all items with the value null would sort to the same location regardless of which operator (< or \>) was used (null items would always sort to the top or the bottom regardless of which operator is used.)

Note that currently the May 2005 Visual Studio.NET Tech. Preview does not support the \>= and <= operators.  Also, the \== operator is marked as obsolete.

I would be curious to know what folks think about this implementation?

(This topic is also being discussed at on the GotDotNet C# Language Message board here.)
