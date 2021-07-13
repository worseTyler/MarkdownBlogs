

## The Build Date Problem

I have struggled to find a good way to communicate with users the version of a web site. Version numbers are tricky and only meaningful to people close to the project. I like dates but they are often tricky to implement.

My default choice has always been dates because they are easy for customers to understand. They can look and see if there has been an update recently. However, the only way I have know to do this was to have some sort of pre- or post-build step that would inject the current date and time into a file in the application. While this works it is a hassle and can have some undesirable side effects during development depending on the implementation.

## A Build Date Solution

I recently found a great solution to this problem while working on an ASP.NETCore project, but it should work with any .NET project. The key principle here is that .NET linker stamps a date on the assembly. This date is not easy to get to, but thanks to Jeff Atwood for a great post about it: [https://blog.codinghorror.com/determining-build-date-the-hard-way/](https://blog.codinghorror.com/determining-build-date-the-hard-way/)

By using his code and shaping it for a web environment I was able to create a class that will return the date as a static property making the assumption that this only needs to be determined once each time the site is started.

### A Reusable Build Date Class

Here is the code for the AssemblyInfo class that exposes a Date property to return the linker date.

```
using System;
using System.IO;
using System.Reflection;

namespace YourNamespaceHere
{
    /// <summary>
    /// Information about the executing assembly.
    /// </summary>
    public static class AssemblyInfo
    {
        private static DateTime? \_Date = null;

        /// <summary>
        /// Gets the linker date from the assembly header.
        /// </summary>
        public static DateTime Date
        {
            get
            {
                if (\_Date == null)
                {
                    \_Date = GetLinkerTime(Assembly.GetExecutingAssembly());
                }
                return \_Date.Value;
            }
        }

        /// <summary>
        /// Gets the linker date of the assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        /// <remarks>https://blog.codinghorror.com/determining-build-date-the-hard-way/>
        private static DateTime GetLinkerTime(Assembly assembly)
        {
            var filePath = assembly.Location;
            const int c\_PeHeaderOffset = 60;
            const int c\_LinkerTimestampOffset = 8;

            var buffer = new byte\[2048\];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                stream.Read(buffer, 0, 2048);

            var offset = BitConverter.ToInt32(buffer, c\_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c\_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            return linkTimeUtc;
        }
    }
}
```

### Updating Your View with the Build Date

This method can then easily be called in your \_layout.cshtml page to show the date of the deployment on every page. In addition, you can also choose to show it on an about page or other location of your choice. If you want consistent formatting for multiple locations, you could create another method that provides a formatted string.

```
@(YourNamespaceHere.AssemblyInfo.Date.ToString("yyyy-MM-dd HH-mm"))

```

I hope this helps you better communicate with your users about which version of a page they are seeing.

### Update for Visual Studio 15.4.

Builds in VS now contain information that makes the values in these fields gibberish. Fortunately, this can be easily fixed with a change to your .csproj file. in the first <PropertyGroup> section add the following element:

```
<Deterministic>False</Deterministic>

```

Thanks to Tom Puckett for the note on this below.
