---
title: "PGP Pipeline Component"
date: "2007-08-07"
categories: 
  - "biztalk"
  - "blog"
  - "pipeline-components"
tags: 
  - "biztalk"
  - "pipeline-components"
---

**UPDATE - Check out [my blog](/pgp-pipeline-component-v1-1/) covering V1.1 updates.**

Recently I was required to perform some PGP encryption and decryption of files.  Realizing this was going to require a custom Pipeline Component, off I went to Google to find one.

Hey, why reinvent the wheel?

I'm not certain why Microsoft didn't put one in place with the release of BTS 2006, but who am I to judge? :)

The ones that kept popping up:

- Gnu Privacy Guard (GPG/PGP) for .NET \[v1.0\]
- [Pro BizTalk 2006 (Pro) (Paperback)](https://www.amazon.com/Pro-BizTalk-2006-George-Dunphy/dp/1590596994/ref=sr_1_1/105-3971130-9369259?ie=UTF8&s=books&qid=1183137095&sr=8-1) by George Dunphy (Author), Ahmed Metwally (Author)
- And a few others that wanted you to buy it.  Sorry, but I'm not going to buy something I can write myself.

**GnuPG** This one works very well.  I actually used GnuPG when creating a pipeline component for another project.  However, it is a command line program and requires installation and key management before it works.  While I knew this would get the job done, I wanted to use something that could be automatically deployed from machine to machine.

**Pro BizTalk 2006** Since I have not purchased this book, I don't have access to the code.  Again, I don't want to buy something I know I can do. Don't get me wrong, I'm not opposed to buying code, but it needs to make sense.  And in this case, it didn't.  Especially since I have done this before and knew the job could be performed in the allotted timeframe.  Also, I have nothing against this book. It comes highly reviewed.  I'm just a little put-off about buying another BizTalk book.  I waited and waited for the official BTS 2004 book to come out and was very disappointed.  Ok, so I need to get over it.  I will probably buy a BTS 2006 book eventually.  I just don't know which one. \[end rant\]

**Bouncy Castle Crypto** A co-worker pointed me in the direction of the [Bouncy Castle C# API](https://www.bouncycastle.org/csharp/index.html).  They give you the DLL as well as the source code.  The only problem I ran in to was that the DLL was not strongly named.  Once I resolved that issue I was off and running.  This gave me the ability to easily deploy from server to server.  The only thing I had to do was GAC the DLL and copy the keys.

Here is the code for the PGP Pipeline component.  I did not distribute the necessary crypto.dll, so you will need to get it from Bouncy Castle.  Remember, you will need to get the source in order to strongly name the assembly.

\[**UPDATED - 7/27/2007**\] - I have updated the code and source with version 1.1.

Link to readme.txt: readme.txt Link to dll: BAJ.BizTalk.PipelineComponent.PGP.dll Link to source code:  PGP.zip

**Notes:**

- I can't get the stupid icon to appear for whatever reason.  If you spot my error, please let me know so I can fix it.
- The project has a **Post Build Event** that will copy the DLL to C:\\Program Files\\Microsoft BizTalk Server 2006\\Pipeline Components.
    - You may need to change the path based on your environment.
    - You will receive a build failure if you have a project open that references that file.  Also, you may need to stop your BizTalk host before compilation once you have deployed and used the component.
- The code assumes you already have the necessary Public and/or Private keys.  I did not include a way to generate the pair.
- Be careful when testing.  I chased around this error message for almost an hour before I realized what was happening: Could not find file 'C:\\Temp\\testfile.txt'.
    - The PGP Pipeline Component expects the encrypted version of the file and the decrypted version of the file to have the same name with the exception of the .PGP extension.  If you encrypt somefile.txt, it becomes somefile.txt.pgp.  If you then rename somefile.txt.pgp to differentfile.txt.pgp, the crypto.dll writes the decrypted file to somefile.txt.
    - Because I wasn't sure what the end-user's desired outcome should be (error if the filenames don't match, always use the filename of the message, etc.) I left it alone and created a DecryptFileAsStream() method that does not create a file, but returns the decrypted content as a Stream.
- I had thought about adding a property to specify the temporary location where the file is written during encryption/decryption but have not had time to add it.  Currently, it will place the temporary files in C:\\Windows\\System32.
    - Ok, so now I have added it. :)

Feedback welcomed.
