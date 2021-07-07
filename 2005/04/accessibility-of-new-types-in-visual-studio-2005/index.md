---
title: "Accessibility of new types in Visual Studio 2005"
date: "2005-04-26"
categories: 
  - "net"
  - "blog"
tags: 
  - "net"
---

One subtle but noteworthy change in Visual Studio 2005 from Visual Studio 2003 is the accessibility modifier on new types.  In Visual Studio 2005, the default accessibility is internal rather than public.  (This is true from the Add Item menu but not from the Add File menu because they are generated from different templates.)  For example, when defining a new class or interface (yes, Visual Studio provides an Add Item option for interfaces) there is no accessibility modifier.  Rather, it looks like this:

> interface ISettingsProvider { }

The modification is subtle to the casual observer, but not so to the compiler.  As soon as one attempts to program against the type from outside the assembly (a unit test assembly for example), they will discover the new accessibility (or lack there of).

There are two observations to be made from this.  Firstly, Microsoft has consciously chosen "better programming practices" over "usability."  In choosing to automatically mark new types as public, as they did in Visual Studio 2003, they avoid the inevitable developers initial confusion when they cannot find the type they defined even though the assembly is correctly referenced and the appropriate using declarations provided.  Rather, Microsoft has chosen to define types with the minimum accessibility by default, and leave it to the developer to make an intentional choice that enables greater accessibility.  Although developers may often find this inconvenient, the default is in accordance with best practices and I support it.

The second observation is that the default templates can be changed.  I describe the procedure here.  The key is to note that there is a template cache that needs to be updated as well.
