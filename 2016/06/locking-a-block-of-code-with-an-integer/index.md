

I was talking with a developer recently who was convinced that you could use a static integer variable to lock a block of code merely by casting it to an object, like this

```csharp
private static int Number = 0;
…
lock( (object) Number ) { … }
```

First it must be noted that an integer cannot be used by itself in a lock statement because it is a value type. But when you cast an integer to an object, the compiler copies the value of integer into an object, which can then be used in the lock statement. This is called boxing. The lock statement is then valid. But will it really work?

The purpose of the lock statement is to protect a block of code so that only one thread can execute it at a time. In order for lock() to work, all threads sharing the code must reference the same object reference in the lock statement. In the following code where a static object it used, this is clearly the case.

```csharp
private static object LockObject = new object();
…
lock( LockObject ) { … }
```

Let’s examine the integer example. What is really being passed into the lock statement? The question comes down to “when” the integer is boxed and “where” it is stored. In this case, by casting the integer to an object within the lock statement, it is boxed, at runtime, immediately before the call to lock(). As the parameter is being pushed onto the stack, an Integer object is created in the heap, the value of the integer is copied into the object, and the object reference is what is actually pushed onto the stack as the parameter. This implies to me that lock() cannot work this way because the object to which thread A would have a reference is different than the object to which thread B would have a reference when the lock statement is executed.

We can test this with a simple program. Let’s look at the code in a C# Console application.

```csharp
using System;
using System.Threading;
namespace IntLockTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var t1 = new Thread( IntLockTest ) { Name = "IntLock-1" };
            var t2 = new Thread( IntLockTest ) { Name = "IntLock-2" };
            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();
        }
        private static int IntLock = 0;
        private static int ConcurrentSleeper = 0;
        private static void IntLockTest( )
        {
            Console.Out.WriteLine( $"Enter IntLockTest Thread {Thread.CurrentThread.Name}" );
            lock ( (object) IntLock )
            {
                ConcurrentSleeper += 1;
                Console.Out.WriteLine( $"Delay IntLockTest Thread {Thread.CurrentThread.Name} sleeping (concurrent = {ConcurrentSleeper})" );
                Thread.Sleep( 500 );
                ConcurrentSleeper -= 1;
            }
            Console.Out.WriteLine( $"Leave IntLockTest Thread {Thread.CurrentThread.Name}" );
        }
    }
}
```

Here we make two calls to IntLockTest on separate threads. Within the lock statement block we increment a counter and display that value along with the name of the calling thread. The output looks like this.

```csharp
Enter IntLockTest Thread IntLock-1
Delay IntLockTest Thread IntLock-1 sleeping (concurrent = 1)
Enter IntLockTest Thread IntLock-2
Delay IntLockTest Thread IntLock-2 sleeping (concurrent = 2)
Leave IntLockTest Thread IntLock-2
Leave IntLockTest Thread IntLock-1
```

As you can see, thread IntLock-2 enters the (assumed to be) locked block and increments ConcurrentSleeper. However, thread IntLock-1 is still sleeping inside the (supposedly) locked block of code and hasn’t released the lock yet. This proves that casting the integer to an object in the lock statement did not protect the block of code even though it compiles and runs.

For the record, I would never actually use a boxed value type as a locking object. Instead, I recommend using an object allocated specifically for that purpose, something like this,

```csharp
private static object LockObject = new object();
…
lock( LockObject ) { … }
```

But for the purpose of this article, I will show that locking with an integer can be done if it is boxed at the class level. So let’s box the integer in a static reference and try that instead.

```csharp
using System;
using System.Threading;
namespace IntLockTest
{
   class Program
   {
       static void Main(string[] args)
       {
           var t1 = new Thread( BoxedIntLockTest ) { Name = "BoxedIntLock-1" };
           var t2 = new Thread( BoxedIntLockTest ) { Name = "BoxedIntLock-2" };
           t1.Start();
           t2.Start();
           t1.Join();
           t2.Join();
       }
       private static int IntLock = 0;
       private static readonly object BoxedIntLock = (object) IntLock;
       private static int ConcurrentSleeper = 0;
       private static void BoxedIntLockTest( )
       {
           Console.Out.WriteLine( $"Enter IntLockTest Thread {Thread.CurrentThread.Name}" );
           lock ( BoxedIntLock )
           {
               ConcurrentSleeper += 1;
               Console.Out.WriteLine( $"Delay IntLockTest Thread {Thread.CurrentThread.Name} sleeping (concurrent = {ConcurrentSleeper})" );
               Thread.Sleep( 500 );
               ConcurrentSleeper -= 1;
           }
           Console.Out.WriteLine( $"Leave IntLockTest Thread {Thread.CurrentThread.Name}" );
       }
   }
}
```

Here is the new output.

```csharp
Enter IntLockTest Thread BoxedIntLock-1
Delay IntLockTest Thread BoxedIntLock-1 sleeping (concurrent = 1)
Enter IntLockTest Thread BoxedIntLock-2
Leave IntLockTest Thread BoxedIntLock-1
Delay IntLockTest Thread BoxedIntLock-2 sleeping (concurrent = 1)
Leave IntLockTest Thread BoxedIntLock-2
```

This time we see that thread BoxedIntLock-2 had to wait until thread BoxedIntLock-1 left the lock statement block before it could enter the block. Our ConcurrentSleeper count never exceeds a value of one.

The moral of the story is this: The scope of the object reference used to lock a code block must be at a level shared by all threads that will be executing that code, and must exist prior to any thread attempting to enter the code block.

Full sample: [https://github.com/IntelliTect-Samples/2016.06-Blog-CanYouLockACodeBlockWithAnInteger](https://github.com/IntelliTect-Samples/2016.06-Blog-CanYouLockACodeBlockWithAnInteger)
