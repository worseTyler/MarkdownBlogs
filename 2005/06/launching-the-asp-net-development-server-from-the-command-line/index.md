

## Launching ASP NET From Command Line
#
By default, Visual Studio 2005 does not run or debug web projects using IIS.  Rather, web sites are hosted with a new ASP.NET Development Server.  This web host environment dynamically selects a port and begins hosting the web site in a manner that is only accessible to the local host.

The web hosting process is provided as part of the 2.0 framework using a program called ``` WebDev.WebServer.EXE ```, which is located in the framework directory (``` %WINDIR%\\Microsoft.NET\\Framework\\v2.0.XXXXX ```).  Rather than relying on Visual Studio 2..5 to launch it, however, you can do so manually as follows:

> ``` WebDev.WebServer [/port:<port number>] /path:<physical path> [/vpath:<virtual path>] ```

On its' own the command line won't return until the process is stopped, however.  Therefore, you will want to launch it using "``` start /b ```" rather than waiting for it to exit.

Here's an example:

> ``` start /B webdev.webserver.exe /port:4955 /path:"c:\\documents and settings\\MMichael\\Local Settings\\Temp\\HelloWorldWebSite"  /vpath:/HelloWorldWebSite ```

Now you can arbitrarily host any directory as a web site accessible only from the local machine.
