
OK, it is about time I wrote my thoughts down on this topic so here goes:

**Web Services**

> Features

- - Microsoft's direction based on their marketing
    - The stuff Don Box always talks about and, bear in mind, he is the key player on that team
    - Standards compliant - GXA/WS-\* etc

> Issues

- - No plugability for varying channels and formatters
    - A nightmare over wireless channels like GPRS

**.NET Remoting**

> Features

- - Separates out the format from the transport channel  (way cool)!
    - More extensible
    - Support events
    - Simply configuration
    - The coolest geek architecture available

> Issues

- - No standards... .NET required on both sides
    - BinaryFormatter isn't actually that efficient (especially for datsets)
    - Meta data flows on every call
    - No support on compact framework

**Enterprise Services**

> Features

- - The stinkin'. fastest of all the communication options short of going to a lower level (System.Net)
    - Queued components
    - Transaction support (with a performance cost)
    - Authorization/Authentication
    - Resource pooling (object pooling, just-in-time activation) etc.
    - DllHost.exe
    - Component activation

> Issues

- - Let's face it... this was developed before .NET... it isn't cool anymore (that doesn't mean you should no longer buy my COM+ book.)
    - No standards
    - No cross platform (without the web service add-on)
    - No marketing from Microsoft indicating future direction.
    - No support on compact framework

**So were do we go from here?**

1. [Professional Developers Conference](https://msdn.microsoft.com/events/pdc/)
    
2. [Professional Developers Conference](https://msdn.microsoft.com/events/pdc/)
    
3. Consider the following decision matrix:
    

- Whatever you are doing now: - Keep....it will eventually have some kind of future.
    
- If you are starting to do development today and you can't wait for Whidbey: - Analyze the above feature/issues list and follow your gut.
    
- If you can wait for the Whidbey release then: - Go with Web Services and figure out a way to hoist in the Enterprise Service and Remoting features that don't come in the Whidbey box.
