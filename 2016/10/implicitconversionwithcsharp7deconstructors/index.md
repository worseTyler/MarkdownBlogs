

While the deconstruct syntax is interesting for its convenience in assigning to a set of variables, what I believe has far broader implications is its potential to provide an “implicit conversion operator” and in a syntax that, quite frankly, is far easier to recall than the implicit cast operator.  For example, I can provide deconstructors that map to a string (the full path), a FileInfo, and a DirectoryInfo:

```csharp
public void Deconstruct(out string path)
{
    path = Path;
}

public void Deconstruct(out FileInfo file)
{
    file = new FileInfo(Path);
}

public void Deconstruct(out DirectoryInfo directory)
{
    directory = new DirectoryInfo(Path);
}
```

Alas, at least at this time, this doesn’t work.
