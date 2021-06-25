---
title: "The basics of System.IO.Ports.SerialPort"
date: "2005-09-06"
categories: 
  - "net"
  - "blog"
tags: 
  - "net"
---

For the past two weeks I taught a C#/.NET course in Pullman, WA.  Last week they asked me to demonstrate how access the serial port using .NET.  This wasn't something I had done before but I borrowed a serial device and went to work.  I was pleased with how simple it was.  (The hardest part was figuring out the correct settings on the device, but I had some help with this - Thanks Jeff.)  The final code sample was a console base HyperTerminal type application.

> using System.IO; using System.IO.Ports; using System.Text; class Program { static SerialPort ComPort; static ASCIIEncoding ASCIIEncoder \= new ASCIIEncoding();
> 
> public static void OnSerialDataReceived( object sender, SerialDataReceivedEventArgs args) { string data \= ComPort.ReadExisting(); Console.Write(data.Replace("\\r", "\\n")); }
> 
> static void Main(string\[\] args) { string port \= "COM1"; int baud \= 9600; if (args.Length >= 1) { port \= args\[0\]; } if (args.Length >= 2) { baud \= int.Parse(args\[1\]); }
> 
> InitializeComPort(port, baud);
> 
> string text; do { text \= Console.ReadLine(); ComPort.Write(text + '\\r'); } while (text.ToLower() !\= "q"); }
> 
> private static void InitializeComPort(string port, int baud) { ComPort \= new SerialPort(port, baud); // ComPort.PortName = port; // ComPort.BaudRate = baud; ComPort.Parity \= Parity.None; ComPort.StopBits \= StopBits.One; ComPort.DataBits \= 8; ComPort.Handshake \= Handshake.None; ComPort.DataReceived += OnSerialDataReceived; ComPort.Open(); } }

I agree, replacing '\\r' with '\\n' is a primitive way to deal with line feed issues especially considering there is a SerialPort.NewLine property.
