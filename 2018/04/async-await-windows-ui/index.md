

## Async Awaits Windows UI
#
### The async/await pattern ensures that continuation works without switching back to the UI thread to update a control.

One place where synchronization is especially important is in the context of UI and Web programming. With the Windows UI, for example, a message pump processes messages such as mouse click and move events. Furthermore, the UI is single-threaded, so that event interaction with any UI components (a text box, for example) must always occur from the single UI thread. One of the key advantages of the async/await pattern is that it leverages the synchronization context to ensure that continuation work—work that appears after the await statement—will always execute on the same synchronization task that invoked the await statement. This approach is of significant value because it eliminates the need to update a control through the UI thread.

To better appreciate this benefit, consider the example of a UI event for a button click in WPF, as shown in Listing 1.

### Listing 1: Synchronous High-Latency Invocation in WPF

```csharp
using System; 
private void PingButton_Click( 
  object sender, RoutedEventArgs e) 
{
   StatusLabel.Content = "Pinging…";
   UpdateLayout();
   Ping ping = new Ping();
   PingReply pingReply =
       ping.Send("www.IntelliTect.com");
   StatusLabel.Text = pingReply.Status.ToString(); }
```

Given that StatusLabel is a WPF `System.Windows.Controls.TextBlock` control and we have updated the Content property twice within the `PingButton_Click()` subscriber, it would be a reasonable assumption that first “Pinging…” would be displayed until `Ping.Send()` returned, and then the label would be updated with the status of the `Send()` reply.

Those experienced with Windows UI frameworks know this is not what happens. Rather, a message is posted to the Windows message pump to update the content with “Pinging…” but, because the UI thread is busy executing the `PingButton_Click()` method, the Windows message pump is not processed.

By the time the UI thread frees up to look at the Windows message pump, a second Text property update request has been queued, and the only message that the user can observe is the final status.

To fix this problem using TAP, we change the code highlighted in Listing 2.

### Listing 2: Synchronous High-Latency Invocation in WPF Using await

```csharp
using System;
async private void PingButton_Click(
  object sender, RoutedEventArgs e)
{
  StatusLabel.Content = "Pinging...";
  UpdateLayout();
  Ping ping = new Ping();
  PingReply pingReply =
      await ping.SendPingAsync("www.IntelliTect.com");
  StatusLabel.Text = pingReply.Status.ToString();
}
```

This change offers two advantages:

- The asynchronous nature of the ping call frees up the caller thread to return to the Windows message pump caller’s synchronization context and processes the update to `StatusLabel.Content` so that “Pinging…” appears to the user.
- When awaiting `ping.SendTaskAsync()` completes, it will always execute on the same synchronization context as the caller.

Also, because the synchronization context is specifically appropriate for Windows UI, it is single-threaded and, therefore, the return will always be to the same thread—the UI thread. In other words, rather than immediately executing the continuation task, the TPL consults the synchronization context, which instead posts a message regarding the continuation work to the message pump. Next, because the UI thread monitors the message pump, upon picking up the continuation work message, it invokes the code following the await call. (As a result, the invocation of the continuation code is on the same thread as the caller that processed the message pump.)

There is a key code readability feature built into the TAP language pattern. Notice in Listing 2 that the call to return `pingReply.Status` appears to flow naturally after the await, providing a clear indication that it will execute immediately following the previous line.

This should keep your project moving forward!

Questions or comments? Post them below.
