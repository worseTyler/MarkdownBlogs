---
title: "Role Access Security in .NET"
date: "2003-08-15"
categories: 
  - "net"
  - "blog"
tags: 
  - "net"
---

First of all let me say I am very disappointed with the Role Access Security in .NET.  I am probably missing something but it appears Microsoft entirely missed my requirements on this one.  With Role Access security you essentially hard wire what roles are in your application (Administrators, OfficeWorkers, FieldWorkers, etc) and then check whether the current user is in one of these roles before granting them access.  My requirements force me to allow new roles to be defined by my users and that operations be assigned to each role indicating whether a user assigned to that role is allowed to perform the operation.

Role based security enables us to ask whether the current user (Principal) is in a particular role.  The question that I believe is much more prevalent is can the current Principal perform a particular operation.  Trying to morph Microsoft's model to a finer grained model involving operations is a significant challenge without simply writing everything from scratch.

None of the following papers address my concerns but they provide an introduction as to what is available in Role Access Security:

- .NET Security
- Role Access Security by Jason Bock, Pete Stromquist, Tom Fischer, and Nathan Smith
- Unify the Role-Based Security Models for Enterprise and Application Domains with .NET by Juval Lowy
- Distributed Security Practices by Juval Lowy
- Secure Your .NET Apps
- Sample Chapter from Visual Basic .NET Code Security Handbook
- Chapter 8 of Building Secure MicrosoftÂ® ASP.NET Applications
- [Implementing .NET Role-Based security withouth COM+ by Peter Bromberg](http://nullskull.com/articles/20020418.asp)
