---
title: "Providing a Test Method Expansion Template"
date: "2005-02-16"
categories: 
  - "net"
  - "blog"
tags: 
  - "net"
---

Another expansion template I use frequently is a test method template (test driven development and all that).  It does extremely little but when you write lots of tests it is a nice to have.

> \[TestMethod\] public void CalculateSalaryBonus() {
> 
> }

The template simply declares a VS Team Test test method with the appropriate attributes.  If you are using MbUnit/NUnit/TestDriven.NET you need to update the attribute to be \[Test\].  You can download the template from [here](/wp-content/uploads/binary/9921d557-62f5-4d7a-ac58-b953e8819093/Test.zip), saving it to ".\\Microsoft Visual Studio 8\\VC#\\Expansions\\1033\\Expansions" mentioned in previous post.
