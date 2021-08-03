## Using an older .NET framework? Check out these system.threading tips to directly manipulate threads without the task abstraction.

While writing the 7th edition of _[Essential C# 8.0](/essentialcsharp/)_, I realized that it was time to pull out the content on system.threading- focusing instead on Parallel Extensions (`System.Threading.Tasks`).

Parallel Extensions is generally preferred because it allows you to manipulate a higher-level abstraction - namely the task (`System.Threading.Tasks.Task`) and the resulting libraries Task Parallel Library (TPL) and Parallel LINQ (PLINQ), rather than working directly with managed threads. Unfortunately, if you are working with older versions of the framework prior to the .NET Framework 4.0), or you have a programming problem not directly addressed by the task threading abstractions then you will need to instead work with `System.Threading.Thread` and it related APIs. In this article, I cover some of the basic APIs for directly manipulating threads without the task abstraction.

### Asynchronous Operations with `System.Threading.Thread`

The operating system implements threads and provides various unmanaged APIs to create and manage those threads. The Common Language Runtime (CLR) wraps these unmanaged threads and exposes them in managed code via the `System.Threading.Thread` class, an instance of which represents a point of control in the program. As mentioned earlier, you can think of a thread as a worker that independently follows the instructions that make up your program. 

**Listing 1** provides an example. The independent point of control is represented by an instance of Thread that runs concurrently. A thread needs to know which code to run when it starts up, so its constructor takes a delegate that refers to the code that is to be executed. In this case, we convert a method group, `DoWork`, to the appropriate delegate type, `ThreadStart`. We then start the thread running by calling `Start()`. While the new thread is running, the main thread attempts to print 10,000 hyphens to the console. We instruct the main thread to then wait for the worker thread to complete its work by calling `Join()`. The result is shown in **Output 1**. 

#### Listing 1: Starting a Method Using System.Threading.Thread

```csharp
using System; 
using System.Threading; 

public class RunningASeparateThread 
{ 
  public const int Repetitions = 1000; 

  public static void Main() 
  { 
      ThreadStart threadStart = DoWork; 
      Thread thread = new Thread(threadStart); 
      thread.Start(); 
      for(int count = 0; count < Repetitions; count++) 
      { 
          Console.Write('-'); 
      } 
      thread.Join(); 
  } 

  public static void DoWork() 
  { 
      for(int count = 0; count < Repetitions; count++) 
      { 
          Console.Write('+'); 
      } 
  } 
} 
```

#### Output 1 

```
++++++++++++++++++++++++++++++++---------------------------------------- 

------------------------------------------------------------------------ 

------------------------------------------------------------------------ 

------------------------------------------------------------------------ 

------------------------------------------------------------------------ 

------------------------------------------------------------------++++++ 

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ 

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ 

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ 

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ 

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ 

+++++++++++++++++++++++++++++------------------------------------------- 

------------------------------------------------------------------------ 

------------------------------------------------------------------------ 

------------------------------------------------------------------------ 

------------------------------------------------------------------------ 

-------------------------------------------------------+++++++++++++++++ 

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ 

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ 

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ 

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ 

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ 

++++++++++++++++++------------------------------------------------------ 

------------------------------------------------------------------------ 

-----------------------------------------------+++++++++++++++++++++++++ 

++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ 

+++++++++++++++++++++++++++++++++++++++++++++ 
```

As you can see, the threads appear to be taking turns executing, each printing out a few hundred characters before the context switches. The two loops are running in parallel rather than the first one running to completion before the second one begins, as it would if the delegate had been executed synchronously. 

For code to run under the context of a different thread, you need a delegate of type `ThreadStart` or `ParameterizedThreadStart` to identify the code to execute. (The latter allows for a single parameter of type object; both are found in the system.threading namespace.) Given a Thread instance created using the thread-start delegate constructor, you can start the thread executing with a call to `thread.Start()`. (**Listing 1** creates a variable of type `ThreadStart` explicitly to show the delegate type in the source code. The method group DoWork could have been passed directly to the thread constructor.) The call to `Thread.Start()`tells the operating system to begin concurrent execution of the new thread; control on the main thread immediately returns from the call and executes the for loop in the `Main()`method. The threads are now independent, and neither waits for the other until the call to `Join()`.

### Thread Management 

Threads include several methods and properties for managing their execution. Here are some of the basic ones: 

- As we saw in **Listing 1**, you can cause one thread to wait for another with `Join()`. This tells the operating system to suspend execution of the current thread until the other thread is terminated. The `Join()` method is overloaded to take either an `int` or a `TimeSpan` to support a maximum time to wait for thread completion before continuing execution.
- By default, a new thread is a _foreground_ thread; the operating system will terminate a process when all its foreground threads are complete. You can mark a thread as a _background_ thread by setting the `IsBackground` property to true. The operating system will then allow the process to be terminated even if the background thread is still running. However, it is still a good idea to ensure that all threads are not terminated and instead to exit cleanly before the process exits.
- Every thread has an associated priority, which you can change by setting the `Priority` property to a new `ThreadPriority` enum value. The possible values are `Lowest`, `BelowNormal`, `Normal`, `AboveNormal`, and `Highest`. The operating system prefers to schedule time slices to higher-priority threads. Be careful; if you set the priorities incorrectly, you can end up with “starvation” situations where one high-priority thread prevents many low-priority threads from ever running.
- If you simply want to know whether a thread is still alive or has finished all of its work, you can use the Boolean `IsAlive` property. A more informative picture of a thread’s state is accessible through the `ThreadState` property. The `ThreadState` enum values are `Aborted`, `AbortRequested`, `Background`, `Running`, `Stopped`, `StopRequested`, `Suspended`, `SuspendRequested`, `Unstarted`, and `WaitSleepJoin`. These are flags; some of these values can be combined.

There are two commonly used—and commonly abused—methods for controlling threads that deserve to be discussed in their own sections: `Sleep()` and `Abort()` (the latter is not available on .NET Core).

### Do Not Put Threads to Sleep in Production Code

The static `Thread.Sleep(…)` method puts the current thread to sleep, essentially telling the operating system not to schedule any time slices to this thread until the given amount of time has passed. A single parameter—either a number of milliseconds or a `TimeSpan`—specifies how long the operating system will wait before continuing execution. While it is waiting, the operating system will, of course, schedule time slices for any other threads that might be waiting their turn to execute. This might sound like a sensible thing to do, but it is a “bad code smell” that indicates the design of the program could probably be better. 

Threads are often put to sleep to try to synchronize a thread with some event in time. However, the operating system does not guarantee any level of precision in its timing. That is, if you say, “Put this thread to sleep for 123 milliseconds,” the operating system will put it to sleep for _at least_ 123 milliseconds, and possibly much longer. The actual amount of time between the thread going to sleep and then waking up again is not deterministic and can be arbitrarily long. Do not attempt to use `Thread.Sleep()` as a high-precision timer, because it is not. 

Worse, `Thread.Sleep()` is often used as a “poor man’s synchronization system.” That is, if you have some unit of asynchronous work, and the current thread cannot proceed until that work is done, you might be tempted to put the thread to sleep for much longer than you think the asynchronous work will take, in the hopes that it will be finished when the current thread wakes up. This is a bad idea: Asynchronous work, by its very nature, can take longer than you think. Use proper thread synchronization mechanisms, described in the next chapter 20 of my [Essential C# 7](/essentialcsharp7/) book or chapter 22 of my [Essential C# 8](/essentialcsharp/) book, to synchronize threads. (We’ll give an example of this sort of abuse in **Listing 2**.) 

Putting a thread to sleep is also a bad programming practice because it means that the sleeping thread is, obviously, unresponsive to attempts to run code on it. If you put the main thread of a Windows application to sleep, that thread will no longer be processing messages from the user interface and will therefore appear to be hung. 

More generally, putting a thread to sleep is a bad programming practice because the whole point of allocating an expensive resource like a thread is to get work out of that resource. You wouldn’t pay an employee to sleep, so do not pay the price of allocating an expensive thread only to put it to sleep for millions or billions of processor cycles. 

That said, there are some valid uses of `Thread.Sleep()`. First, putting a thread to sleep with a time delay of zero tells the operating system “the current thread is politely giving up the rest of its quantum to another thread if there is one that can use it.” The polite thread will then be scheduled normally, without any further delay. Second, `Thread.Sleep()` is commonly used in test code to simulate a thread that is working on some high-latency operation without actually having to burn a processor doing some pointless arithmetic. Other uses in production code should be reviewed carefully to ensure that there is not a better way to obtain the desired effect. 

In task-based asynchronous programming in C# 5, you can use the await operator on the result of the [`Task.Delay()` method](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.delay) to introduce an asynchronous delay without blocking the current thread. 

**Guidelines: AVOID** calling `Thread.Sleep()` in production code. 

Do Not Abort Threads in Production Code   
The Thread object has an `Abort()` method (although not available on .NET Core) that, when executed, attempts to destroy the thread. It does so by causing the runtime to throw a `ThreadAbortException` in the thread; this exception can be caught, but even if it is caught and ignored, it is automatically rethrown to try to ensure that the thread is, in fact, destroyed. There are many reasons why it is a very bad idea to attempt to abort a thread. Here are some of them: 

- The method promises only to _try_ to abort the thread; there is no guarantee that it will succeed. For example, the runtime will not attempt to cause a `ThreadAbortException` if the point of control of the thread is currently inside a finally block (because critical cleanup code could be running right now and should not be interrupted) or is in unmanaged code (because doing so could corrupt the CLR itself). Rather, the CLR defers throwing the exception until control leaves the finally block or returns to managed code. But there is no guarantee that this ever happens. The thread being aborted might contain an infinite loop inside a finally block. (Ironically, the fact that the thread has an infinite loop might be the reason you are attempting to abort it in the first place.) 
- The aborted thread might be in critical code protected by a lock statement (see chapter 20 of my [Essential C# 7](/essentialcsharp7/) book or chapter 22 of my [Essential C# 8](/essentialcsharp/) book). Unlike a finally block, a lock will not prevent the exception. The critical code will be interrupted halfway through by the exception, and the lock object will be automatically released, allowing other code that is waiting on the lock object to enter the critical section and observe the state of the halfway-executed code. The whole point of locking is to prevent that scenario, so aborting a thread can transform what looks like thread-safe code into dangerously incorrect code. 
- The CLR guarantees that its internal data structures will never be corrupted if a thread is aborted, but the Base Class Library (BCL) does not make this guarantee. Aborting a thread can leave any of your data structures or the BCL’s data structures in an arbitrarily bad state if the exception is thrown at the wrong time. Code running on other threads, or in the finally blocks of the aborted thread, can see this corrupted state and crash or behave badly. 

In short, you should never abort a thread unless you are doing so as a last resort; ideally you should abort a thread only as part of a larger emergency shutdown whereby the entire `AppDomain` or the entire process is being destroyed. Fortunately, task-based asynchrony uses a more robust and safer cooperative cancellation pattern to terminate a thread when results are no longer needed, as discussed in the next major section, “Asynchronous Tasks.”

**Guidelines: AVOID** aborting a thread in production code; doing so will yield unpredictable results and can destabilize a program.

### Thread Pooling

 As we discussed earlier, in the Beginner Topic titled “Performance Considerations,” it is possible for an excess of threads to negatively impact performance. Threads are expensive resources, thread context switching is not free, and running two jobs in simulated parallelism via time slicing can be significantly slower than running them one after the other.   
To mitigate these problems, the BCL provides a thread pool. Instead of allocating threads directly, you can tell the thread pool which work you want to perform. When the work is finished, rather than the thread terminating and being destroyed, it is returned to the pool, saving on the cost of allocating a new thread when more work comes along. **Listing 2** shows how to do the same thing as **Listing 1**, but this time with a pooled thread. 

**Listing 2: Using `ThreadPool` Instead of Instantiating Threads Explicitly** 

```csharp
using System; 
using System.Threading; 
 
public class Program 
{ 
  public const int Repetitions = 1000; 
  public static void Main() 
  { 
      ThreadPool.QueueUserWorkItem(DoWork, '+'); 

      for(int count = 0; count < Repetitions; count++) 
      { 
          Console.Write('-'); 
      } 

      // Pause until the thread completes. 
      // This is for illustrative purposes; do not 
      // use Thread.Sleep for synchronization in 
      // production code. 
      Thread.Sleep(1000); 
  } 
  public static void DoWork(object? state) 
  { 
      for(int count = 0; count < Repetitions; count++) 
      { 
          Console.Write(state); 
      } 
  } 
} 
```

The output of **Listing 2** is similar to **Output 1**—that is, an intermingling of periods and hyphens. If we had a lot of different jobs to perform asynchronously, this pooling technique would provide more efficient execution on single-processor and multiprocessor computers. The efficiency is achieved by reusing threads over and over rather than reconstructing them for every asynchronous call. Unfortunately, thread pool use is not without its pitfalls: There are still performance and synchronization problems to consider when using a thread pool. 

To make efficient use of processors, the thread pool assumes that all the work you schedule on the thread pool will finish in a timely manner so that the thread can be returned to the thread pool and reused by another task. The thread pool also assumes that all the work will be of a relatively short duration (i.e., consuming milliseconds or seconds of processor time, not hours or days). By making this assumption, it can ensure that each processor is working full out on a task and not inefficiently time-slicing multiple tasks, as described in the Beginner Topic on performance. The thread pool attempts to prevent excessive time slicing by ensuring that thread creation is “throttled” so that no one processor is oversubscribed with too many threads. Of course, as a consequence, consuming all threads within the pool can delay execution of queued-up work. If all the threads in the pool are consumed by long-running or I/O-bound work, the queued-up work will be delayed. 

Unlike `Thread` and `Task`, which are objects that you can manipulate directly, the thread pool does not provide a reference to the thread used to execute a given piece of work. This prevents the calling thread from synchronizing with, or controlling, the worker thread via the thread management functions described earlier in the blog. In **Listing 2** we use the poor man’s synchronization that we earlier discouraged; this would be a bad idea in production code because we do not actually know how long the work will take to complete. 

In short, the thread pool does its job well, but that job does not include providing services to deal with long-running jobs or jobs that need to be synchronized with the main thread or with one another. What we really need to do is build a higher-level abstraction that can use threads and thread pools as an implementation detail; that abstraction is implemented in the new TPL API found in C# 5 or later.  

**Guidelines:** **DO** use the thread pool to efficiently assign processor time to processor-bound tasks. **AVOID** allocating a pooled worker thread to a task that is I/O bound or long-running; use TPL instead.
