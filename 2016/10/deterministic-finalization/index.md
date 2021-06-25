---
title: "Deterministic Finalization with IDisposable and the Using Statement"
date: "2016-10-14"
categories: 
  - "net"
  - "blog"
  - "c"
tags: 
  - "c"
  - "csharp"
---

The problem with finalizers on their own is that they don’t support deterministic finalization (the ability to know when a finalizer will run). Rather, finalizers serve the important role of being a backup mechanism for cleaning up resources if a developer using a class neglects to call the requisite cleanup code explicitly.

For example, consider the `TemporaryFileStream`, which includes not only a finalizer but also a `Close()` method. This class uses a file resource that could potentially consume a significant amount of disk space. The developer using `TemporaryFileStream` can explicitly call `Close()` to restore the disk space.

Providing a method for deterministic finalization is important because it eliminates a dependency on the indeterminate timing behavior of the finalizer. Even if the developer fails to call `Close()` explicitly, the finalizer will take care of the call. In such a case, the finalizer will run later than if it was called explicitly—but it will be called eventually.

Because of the importance of deterministic finalization, the base class library includes a specific interface for the pattern and C# integrates the pattern into the language. The `IDisposable` interface defines the details of the pattern with a single method called `Dispose()`, which developers call on a resource class to “dispose” of the consumed resources. Listing 9.21 demonstrates the `IDisposable` interface and some code for calling it.

using System;
using System.IO;

class Program
{
 // ...
 static void Search()
 {
  TemporaryFileStream fileStream =
   new TemporaryFileStream();

  // Use temporary file stream;
  // ...

  fileStream.Dispose();

  // ...
 }
}
class TemporaryFileStream: IDisposable
{
 public TemporaryFileStream(string fileName)
 {
  File = new FileInfo(fileName);
  Stream = new FileStream(
   File.FullName, FileMode.OpenOrCreate,
   FileAccess.ReadWrite);
 }

 public TemporaryFileStream()
 : this(Path.GetTempFileName()) {}

 ~TemporaryFileStream()
 {
  Dispose(false);
 }

 public FileStream Stream {get; }
 public FileInfo File {  get; }

 public void Close()
 {
  Dispose();
 }

 #
 region IDisposable Members

 public void Dispose()

 {

  Dispose(true);

  // Turn off calling the finalizer

  System.GC.SuppressFinalize(this);

 }

 #endregion

 public void Dispose(bool disposing)
 {
  // Do not dispose of an owned managed object (one with a
  // finalizer) if called by member finalize,
  // as the owned managed objects finalize method
  // will be (or has been) called by finalization queue
  // processing already
  if (disposing)
  {
   Stream ? .Dispose();
  }
  File ? .Delete();
 }
}

From `Program.Search()`, there is an explicit call to `Dispose()` after using the TemporaryFileStream. `Dispose()` is the method responsible for cleaning up the resources (in this case, a file) that are not related to memory and, therefore, subject to cleanup implicitly by the garbage collector. Nevertheless, the execution here contains a hole that would prevent execution of `Dispose()`—namely, the chance that an exception will occur between the time when `TemporaryFileStream` is instantiated and the time when `Dispose()` is called. If this happens, `Dispose()` will not be invoked and the resource cleanup will have to rely on the finalizer. To avoid this problem, callers need to implement a try/finally block. Instead of requiring programmers to code such a block explicitly, C# provides a using statement expressly for the purpose (Listing 9.22).

class Program
{
 // ...
 
 static void Search()
 {
  using(TemporaryFileStream fileStream1 =
   new TemporaryFileStream(),
   fileStream2 = new TemporaryFileStream())
  {
   // Use temporary file stream;
  }
 }
}

The resultant CIL code is identical to the code that would be created if the programmer specified an explicit try/finally block, where `fileStream.Dispose()` is called in the finally block. The using statement, however, provides a syntax shortcut for the try/finally block.

Within a using statement, you can instantiate more than one variable by separating each variable from the others with a comma. The key considerations are that all variables must be of the same type and that they implement `IDisposable`. To enforce the use of the same type, the data type is specified only once rather than before each variable declaration.
