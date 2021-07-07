---
title: "How to detect virtual machine execution"
date: "2005-07-14"
categories: 
  - "net"
  - "blog"
tags: 
  - "net"
---

Several months ago I cam across some C code that cleverly detected whether a process was running on a Virtual Machine or not.  It uses terms like "redpill" and "matrix" in order to symbolize context within a virtual machine or not.  The code places the SIDT assembler instruction into a string and then executes the instruction to determine whether it successfully modifies the expected register or not.  The problem is that the code no longer works with Windows 2003 SP1 and Windows XP SP2.

The issue is caused by the addition of the Data Execution Protection (DEP) feature that current CPUs support and the service packs now recognize.  DEP is a security counter measure against buffer overflow holes.  It prevents the execution of instructions within memory assigned to data and instead only processes instructions specifically allocated within an execution block.  It is a pretty cool feature that required both processor and OS support.

To circumvent DEP it is necessary to place the instructions into memory allocated using VirtualAllocEx() and VirtualProtectEx() with the PAGE\_EXECUTE\_READWRITE for the protection.  Here is the updated C/C++ code:

> #define WIN32\_LEAN\_AND\_MEAN #include #include #include
> 
> #if \_UNICODE #define cout wcout #endif
> 
> using namespace std;
> 
> void WriteLastError() { DWORD dw \= GetLastError(); TCHAR szBuf\[80\]; LPVOID lpMsgBuf; FormatMessage( FORMAT\_MESSAGE\_ALLOCATE\_BUFFER | FORMAT\_MESSAGE\_FROM\_SYSTEM, NULL, dw, MAKELANGID(LANG\_NEUTRAL, SUBLANG\_DEFAULT), (LPTSTR) &lpMsgBuf, 0, NULL );
> 
> wsprintf(szBuf, \_T("ERROR(%d): %s"), dw, lpMsgBuf);
> 
> wcout << szBuf;
> 
> LocalFree(lpMsgBuf); }
> 
> int \_tmain() { unsigned char matrix\[6\];
> 
> unsigned char redpill\[\] \= "\\x0f\\x01\\x0d\\x00\\x00\\x00\\x00\\xc3";
> 
> HANDLE hProcess \= GetCurrentProcess();
> 
> LPVOID lpAddress \= NULL; PDWORD lpflOldProtect \= NULL;
> 
> try { DWORD dw;
> 
> **\*((unsigned\*)&redpill\[3\]) \= (unsigned)matrix;**
> 
> **lpAddress \= VirtualAllocEx(hProcess, NULL, 6, MEM\_RESERVE|MEM\_COMMIT , PAGE\_EXECUTE\_READWRITE);** if(lpAddress == NULL) { WriteLastError(); }
> 
> **BOOL success \= VirtualProtectEx( hProcess, lpAddress, 6, PAGE\_EXECUTE\_READWRITE , lpflOldProtect);** dw \= GetLastError(); if(success !\= 0) { WriteLastError(); }
> 
> **memcpy(lpAddress, redpill, 8);**
> 
> **((void(\*)())lpAddress)();** if (matrix\[5\]>0xd0) { wcout << \_T("Inside Matrix!\\n"); return 1; } else { wcout << \_T("Not in Matrix.\\n"); return 0; } } finally { VirtualFreeEx(hProcess, lpAddress, 0, MEM\_RELEASE); }
> 
> }

Next I hope to demonstrate the same code in C# using the new [Marshal.GetDelegateForFunctionPointer()](https://msdn2.microsoft.com/library/zdx6dyyh(en-us,vs.80).aspx) function that Devin Jenson mentions.
