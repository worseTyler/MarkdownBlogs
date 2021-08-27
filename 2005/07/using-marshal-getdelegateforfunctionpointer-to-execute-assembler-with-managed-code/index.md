

## Using Marshal.GetDelegateForFunctionPointer to Execute Assembler 
#
I never noticed the [Marshal.GetDelegateForFunctionPointer()](https://msdn2.microsoft.com/library/zdx6dyyh(en-us,vs.80).aspx) function in the .NET Framework 2.0 until Devin Jenson posted about using it to run native assembly code from C#.  This was a wonderfully timed post for me as I was just putting together the finishing touches on the code for my how to detect virtual machine execution.  What ``` Marshal.GetDelegateForFunctionPointer() ``` enables is certainly impressive.

One thing that Devin pointed out in his post was the need for ``` VirtualAllocEx() ``` and ``` VirtualProtectEx() ``` calls to ensure the code executed was not in a Data Execution Protection block.  Since I needed to make those calls anyway to port my C/C++ code, I decided to post how to do it here:

```csharp
class MemoryManager {
  const uint MEM\ _COMMIT = 0x1000;
  const uint MEM\ _RESERVE = 0x2000;
  const uint MEM\ _DECOMMIT = 0x4000;
  const uint PAGE\ _EXECUTE\ _READWRITE = 0x40;

  [DllImport("kernel32.dll")] public static extern IntPtr GetCurrentProcess();

  [DllImport("kernel32.dll")] static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint dwFreeType);

  public static bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize) {
    return VirtualFreeEx(hProcess, lpAddress, dwSize, MEM\ _DECOMMIT);
  }
  public static bool VirtualFreeEx(IntPtr lpAddress, UIntPtr dwSize) {
    return VirtualFreeEx(GetCurrentProcess(), lpAddress, dwSize, MEM\ _DECOMMIT);
  }

  [DllImport("kernel32", SetLastError = true)] static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flAllocationType, uint flProtect);

  [DllImport("kernel32.dll", SetLastError = true)] static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

  public static IntPtr AllocExecutionBlock(int size) {
    return AllocExecutionBlock(size, GetCurrentProcess());
  }

  public static IntPtr AllocExecutionB lock(int size, IntPtr hProcess) {
    IntPtr codeBytesPtr;
    codeBytesPtr = VirtualAllocEx(hProcess, IntPtr.Zero, (UIntPtr) size, MEM\ _RESERVE | MEM\ _COMMIT, PAGE\ _EXECUTE\ _READWRITE);

    if (codeBytesPtr == IntPtr.Zero) {
      throw new System.ComponentModel.Win32Exception();
    }

    uint lpflOldProtect;
    if (!VirtualProtectEx(hProcess, codeBytesPtr, (UIntPtr) size, PAGE\ _EXECUTE\ _READWRITE, out lpflOldProtect)) {
      throw new System.ComponentModel.Win32Exception();
    }

    return codeBytesPtr;
  }
}
 ```

Updating Devin's code to use ``` MemoryManager ``` involves replacing his ``` Marshal.AllocCoTaskMem() ``` call with ``` MemoryManager.AllocExecutionBlock() ``` and ``` Marshal.FreeCoTaskMem() ``` with ``` MemoryManager.VirtualFreeEx() ```.  However, since this is really a resource that requires disposal, I also created a struct for the purpose.

```csharp
public struct VirtualMemoryPtr: IDisposable {
  public VirtualMemoryPtr(int memorySize) {
    ProcessHandle = MemoryManager.GetCurrentProcess();
    MemorySize = (UIntPtr) memorySize;
    AllocatedPointer = MemoryManager.AllocExecutionBlock(memorySize, ProcessHandle);
    Disposed = false;
  }

  public readonly IntPtr AllocatedPointer;
  readonly IntPtr ProcessHandle;
  readonly UIntPtr MemorySize;
  bool Disposed;

  public static implicit operator IntPtr(VirtualMemoryPtr virtualMemoryPointer) {
    return virtualMemoryPointer.AllocatedPointer;
  }

  #region IDisposable Members public void Dispose() {
    if (!Disposed) {
      Disposed = true;
      GC.SuppressFinalize(this);
      MemoryManager.VirtualFreeEx(ProcessHandle, AllocatedPointer, MemorySize);
    }
  }
  #endregion
}
```

One thing I haven't figured out yet is why ``` GetDelegateForFunctionPointer() ``` is it not declared as ``` GetDelegateForFunctionPointer<TDelegate(IntPtr ptr) ``` since this avoids casting on the return.
