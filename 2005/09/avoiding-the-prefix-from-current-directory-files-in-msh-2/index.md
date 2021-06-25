---
title: "Avoiding the \".\" Prefix from Current Directory Files in MSH"
date: "2005-09-19"
categories: 
  - "net"
  - "blog"
tags: 
  - "net"
---

For those not familiar with the Unix flavor of command shells, you may be surprised to learn that typing the name of an executable that is in the current path does not work.  Instead, the executable name must be prefixed with ".".  I asked Jim Truher about this and I was informed the change from the cmd.exe behavior was for security reasons yet there was a work around.  (I could go into the details of his security explanation but I don't really buy it.)

To get back the cmd.exe behavior that doesn't require the prefix, update your path environment variable in MSH to include a period by adding the following to your Profile.msh file.

> #Allow current directory files to execute without "." prefix $ENV:path=".;" + $ENV:path

The system PATH environment variable can also be updated to achieve the same result without adversely affecting cmd.exe.
