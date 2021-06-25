---
title: "Associating the C# language service with other extensions"
date: "2003-08-06"
categories: 
  - "net"
  - "blog"
tags: 
  - "net"
---

Now that I have got "Scripting with C#" working I wanted to associate the .NCS files so that I could use VS.NET to edit them.  Anson Horton at Microsoft gave me the solution:

> "Go to HKLM\\Software\\Microsoft\\VisualStudio\\<version>\\Languages\\File Extensions, add a key with your extension name, and then copy the GUID from the Â“.csÂ” key to the new one"

Thats it... and it works too.
