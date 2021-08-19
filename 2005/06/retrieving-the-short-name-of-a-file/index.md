## Retrieving the Short Name of a File
#
I recently came across the ``` Delay.exe ``` Console program listed here.  I wanted to use the program to delay the loading of some of the auto-run applications that started when I logged in.  (I like to have start-up be as fast as possible and, although I want the some programs to start at some point, I wanted to delay a little until I got started with whatever.)  The problem is that some of the programs that I was delay-starting took parameters and delay didn't seem to accommodate this when the application path contained spaces.  ``` Dir ``` can give you a files short name, but not the short name for the entire path.  I decided to quickly build a [ShortName.exe](https://intellitect.com/wp-content/uploads/binary/7e8537f6-6d57-4f3e-8f92-4e5dad5f6db3/ShortName.zip) utility.

I wanted the program to be "instant" and, since I sometimes wanted short name on systems where .NET was not installed, I went with unmanaged C++.  (If the truth be told I probably did it in C++ to see if I still new that language.  It was fun just for the reminder of how good life is.  Error handling was just slightly more complicated in the "old" days.  Without it this program would have been four statements.)

The source code is shown below.  The entire project source code and executable may be downloaded from the [enclosure](https://intellitect.com/wp-content/uploads/binary/7e8537f6-6d57-4f3e-8f92-4e5dad5f6db3/ShortName.zip).

```clike
#include #include  #if  _UNICODE  #define  cout wcout  #endif

using namespace std;

int\ _tmain(int argc, _TCHAR\ * argv[]) {
  TCHAR lpszShortPath[1024];
  if (argc 0) {
    GetShortPathName(argv[1], lpszShortPath, 1024);
    TCHAR szBuf[80];
    LPVOID lpMsgBuf;
    DWORD dw = GetLastError();

    if (dw 0) {
      FormatMessage(FORMAT\ _MESSAGE\ _ALLOCATE\ _BUFFER | FORMAT\ _MESSAGE\ _FROM\ _SYSTEM, NULL, dw, MAKELANGID(LANG\ _NEUTRAL, SUBLANG\ _DEFAULT), (LPTSTR) & lpMsgBuf, 0, NULL);

      wsprintf(szBuf, _T("ERROR(%d): %s - '%s'"), dw, lpMsgBuf, argv[1]);

      cout << szBuf;

      LocalFree(lpMsgBuf);
    } else {
      cout << lpszShortPath;
    }
    ExitProcess(dw);
  }
}
```

P.S. It is strange not to select the ".NET" category for a computer related post.
