

## Generalized Async Return Types
#
Ever since C# 5.0 when the async/await pattern was introduced, the only supported return types were [Task<TResult>](https://msdn.microsoft.com/en-us/library/dd321424(v=vs.110).aspx), [Task](https://msdn.microsoft.com/en-us/library/system.threading.tasks.task(v=vs.110).aspx), and void (the latter only in exceptional cases). The problem with this limitation is that on occasions when the result is known immediately, it is still necessary to proceed with the formality of instantiating a Task even though no task is actually required to determine the result.

Consider, for example, a function that returns the amount of space consumed by a directory.

```csharp
public async Task<long> GetDirectorySize<T>(string path, string searchPattern)
{
    if (!Directory.EnumerateFileSystemEntries(path, searchPattern).Any())
        return 0;
    else 
        return await Task.Run<long>(()=> Directory.GetFiles(path, searchPattern, 
            SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)));
}
```

If the directory is empty, the known space is 0 and there is no need for an asynchronous thread to calculate the size.  However, since Task<long>  is the return, it still needs to be instantiated.  (If you look at the IL implementation this id done via  AsyncTaskMethodBuilder<long>.Create().)

C# 7.0 introduces the ability to define custom return types on async methods.  The key requirement is implementing the GetAwaiter  method.  The System.Threading.Tasks.ValueTask<T>  provides an example of such a customer type.  It is designed for the very scenario when the return value might be known immediately - cached from a previous invocation for example.  We can leverage this type in our GetDirectorySize()  implementation, for example, to instead return a ValueTask<long>.

```csharp
public async ValueTask<long> GetDirectorySize<T>(string path, string searchPattern)
{
    if (!Directory.EnumerateFileSystemEntries(path, searchPattern).Any())
        return 0;
    else 
        return await Task.Run<long>(()=> Directory.GetFiles(path, searchPattern, 
            SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length)));
}
```

Notice that no other changes are required.

Note that internally, if you open the method up with an IL disassembler like ILDasm.exe the signature still returns a Task<T>:

```csharp
[AsyncStateMachine(typeof(CustomAsyncReturn.<GetDirectorySize>d__3<>))]
public Task<long> GetDirectorySize<T>(string path, string searchPattern)
{
    // ...
}
```
