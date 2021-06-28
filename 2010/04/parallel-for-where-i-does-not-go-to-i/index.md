
A problem was recent posed that I hadn't thought to do using `Parallel.For` in C#. How do you parallelize: \[code language="csharp"\]for(int i = min + offset; i < max; i+=increment) { DoSomething(i); }\[/code\]

Typically, I would simply iterate over a enumerable collection and run against the data in parallel using `Parallel.For` and `Parallel.ForEach`

\[code language="csharp"\]int min = 0; int max = 500; Parallel.For( min, max, DoSomething ); Parallel.ForEach( Enumerable.Range( min, max ), DoSomething ); Parallel.ForEach( ParallelEnumerable.Range( min, max ), DoSomething );\[/code\]

It turns out that there is no support for this in the PFX API. `Parallel.For` does not support incrementing other than one and it does not support reverse iteration. Also, `Enumerable` is a static class - which means we can't do extension methods on `Enumerable`. However, we can use a custom iterator to generate the sequence for us. By using a `Func<int, int>` allows us to use our familiar `i+=` syntax for the increment function. We can quickly create a very simple iterator that will help us out.

\[code language="csharp" wraplines="false"\]public class Sequence { public static IEnumerable<int> From( int min, int max, int increment = 1, int offset = 0 ) { return From( min, max, i => i += increment, offset ); }

public static IEnumerable<int> From( int min, int max, Func<int, int> increment, int offset = 0 ) { return From( min, max, increment, i => true, offset ); }

// ... The rest of the API hidden for brevity ...

public static IEnumerable<int> From( int min, int max, Func<int, int> increment, Func<int, bool> filter ) { for ( int i = min; i < max; i = increment( i ) ) { if ( filter( i ) ) { yield return i; } } } }\[/code\]

This is great, but it only works for integers. We have two options, create overloads for all types we want to generate sequences for, or write a generic version. I have chosen to implement this as a generic implementation, but it means we have a few issues. First, we need to be able to detect the terminating condition `( i < max )` but we can't use the `<` operator. Second, we need to be able to increment the loop indexer by an arbitrary amount; we have to be able to take a numeric or a lambda expression to increment the target type, but C#s type system gets in the way. Third, we need to be able to apply the offset to the minimum without the use of the `+` operator.

In order to solve these problems, I have chosen to use expression trees to create delegates that I can invoke to do the work. This does slow our code down as we are invoking method calls on delegate pointers instead of being able to make simple comparisons on primitive types.

\[code language="csharp" wraplines="false"\]public class Sequence { public static IEnumerable<T> From<T>( T min, T max ) { return From( min, max, i => Operator.Add( i, 1 ) ); }

public static IEnumerable<T> From<T>( T min, T max, T increment ) { return From( min, max, i => Operator.Add<T>( i, increment ), default( T ) ); }

public static IEnumerable<T> From<T>( T min, T max, T increment, T offset ) { return From( min, max, i => Operator.Add<T>( i, increment ), offset ); }

public static IEnumerable<T> From<T>( T min, T max, T increment, Func<T, bool> filter, T offset ) { return From( Operator.Add<T>( min, offset ), i => Operator.LessThan( i, max ), i => Operator.Add<T>( i, increment ), filter ); }

// ... The rest of the API hidden for brevity ...

public static IEnumerable<T> From<T>( T min, Func<T, bool> max, Func<T, T> increment, Func<T, bool> filter ) { for ( T i = min; max( i ); i = increment( i ) ) { if ( filter( i ) ) { yield return i; } } } // ... The Expression tree operator implementation available in the source ... }\[/code\]

Now we can get to the fun part and use the code to generate sequences. If you were to write overloads for int, float, double, decimal, uint, ulong, BigInteger, etc, like the first version, the code would be up to +20x faster than generating the sequence using Enumerable. We incur a lot of overhead using the expression trees and delegates as we invoke method calls instead of simple IL comparisons. However, I think that this version is much more maintainable and much smaller. Another downside to the use of generics is that we can't use C# 4.0 default parameters which really cut down on the number of overloads we had to track manually. Keep in mind that like Enumerable, we are leveraging deferred execution in order to generate the sequences.

With the foundation laid, we can now generate sequences without having to do a post processing filter. Below is an example showing a typical usage of LINQ grabbing the first N odd numbers.

\[code language="csharp"\]public void GetOddNumbers() { int min = 1, max = Int32.MaxValue, take = 200; Func<int, bool> isOdd = i => i % 2 == 1;

Enumerable.Range( min, max ).Where( isOdd ).Take( take ); // becomes Sequence.From( min, max, 2 ).Take( take ); // or Sequence.From( min, max, i => i+=2 ).Take( take ); // or Sequence.From( min, max, 1, isOdd).Take( take ); // or Sequence.From( min, max, i=>i+=2, isOdd).Take( take ); // or Sequence.From( min, max, i=>i+=1, isOdd).Take( take ); }

// In addition to this, we can also generate the collection in reverse. public void SequencesCanGoInReverse() { int min = 0, max = 10; IEnumerable<int> sequence = Sequence.From( max - 1, i => i >= min, i => --i); // generates {9, 8, 7, 6, 5, 4, 3, 2, 1, 0}; sequence = Sequence.From( max - 1, i => i >= min, i => i-=2); // generates { 9, 7, 5, 3, 1 }; }\[/code\]

We can now go forward and backward, skip, and filter when generating sequences with a wide variety of types. Given this power, we can process our sequences in parallel using `Parallel.ForEach` just as we would any other collection. Our original goal of using `Parallel.For` is not possible so far as I can tell. But leveraging the versatility and deferred execution provided by this method, we can do just about anything we want now.

A word of warning. Do not use `i => i++` or `i => i--` as your incrementing function as you are not returning the incremented value and you will most likely generate an infinite sequence and an `OutOfMemoryException`!

If we look back to our original set of problems to solve, we have taken care of them all and can now run our variable incrementing series using `Parallel.ForEach`. Any of the examples show above can be run in parallel, but I will give you a few more examples.

\[code language="csharp"\]Parallel.ForEach( Sequence.From( min, max, increment, offset ), Console.WriteLine ); Parallel.ForEach( Sequence.From( 3, N, i => i += 2, isPrime ), Console.WriteLine ); Parallel.ForEach( Sequence.From( 0, 6 \* Math.PI, i => i + Math.PI ), Console.WriteLine ); Parallel.ForEach( Sequence.From( new BigInteger( min ), new BigInteger( max ), i => i += 3 ), i => Console.WriteLine( i.ToString() ) );\[/code\]

If you want to take look at the full source to the [Sequence class](/wp-content/uploads/2010/04/Sequence.cs_.txt), you can click here to view it. Remember that this is code not rugged at all and is just a spike, but demonstrates what would be needed in order to create generic sequence implementation for parallelization.
