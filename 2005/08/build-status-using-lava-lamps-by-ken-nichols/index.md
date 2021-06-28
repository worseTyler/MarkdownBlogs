
For about a year I managed a team of developers that built an Enhanced .NET Framework.  We were tasked with filling in the gaps that Microsoft left out of 1.X as well as establishing best practices/standards within the company.  We started out with an [Extreme Programming](https://www.extremeprogramming.org/) approach and over time streamlined into more of an [Agile methodology](https://en.wikipedia.org/wiki/Agile_software_development).  One of the Extreme Programming core practices is [continuous integration](http://www.extremeprogramming.org/rules/integrateoften.html), which essentially translates to a fully automated build process.  Not only does this require the build is automated, but that after running we have some type of notification of the build status.  Since builds are running pretty much all the time, emailing the status quickly becomes annoying.  To address this, one of the engineers, Ken Nichols, set up our build to provide notification using Lava Lamps.  Here's his write-up of what he did.  (Thanks for sharing this Ken!!!)

> [CruiseControl.NET](http://cruisecontrol.sourceforge.net/) is an automated continuous integration application, implemented using the Microsoft .NET Framework. It's open source and so ideal for modifications.  Our frameworks team uses this for our builds and I couldn't help wanting to "improve" the feedback it gives. Since our team sits in a common area (known as "The Pit"), Stephen Johnson came up with the idea to have a visual indicator for showing our build status. He had seen an article on www.pragmaticautomation.com about using lava lamps for displaying build status. Well, the article talks about using X10 devices to control the lampsâ€¦. hey we're engineers here, and I have a hardware background, we can build this ourselves. In a nutshell, all that needed to be done was build a controller circuit that was driven from the parallel port of our Cruise Control Server, to control the lava lamps, and then modify CCNET to control the parallel port. Simple!
> 
> **Hardware**
> 
> We purchased two lava lamps, one red, and one green. I built the following controller circuit to control the lava lamps:
> 
> ![LavaLamp-Circuit1.jpg](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2005/08/build-status-using-lava-lamps-by-ken-nichols/images/LavaLamp-Circuit1.jpg)
> 
> Data 0 (pin 2) of the parallel port is the control line for the circuit. The two 1N4148 diodes protect the parallel port from greater than +5v signals and wrong polarity signals. The BC337 transistor controls the base current for the 2N3053 transistor which controls the actual load current. The 1N4001 diode blocks voltage spikes from the relay, which happen when the current is cut off (this is due to the inherent inductance of relays). The relay itself is a SPDT (single pole double throw) 12V  relay with 10A @ 120V contacts. It only draws 38mA of current, so a one transistor circuit would probably have been sufficient. The relay contacts switch the AC live wire to the appropriate AC outlet connector to power the appropriate Lava lamp. The AC neutral wire is directly connected to both AC outlet connectors. The above circuit, minus the relay, is housed within the parallel port connector housing, while the AC connectors and relay are housed in a separate box. This box also contains a self contained power supply circuit (shown below) to power the control circuit shown above.
> 
> ![LavaLamp-Circuit2.jpg](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2005/08/build-status-using-lava-lamps-by-ken-nichols/images/LavaLamp-Circuit2.jpg) **Software**
> 
> Writing programs to talk with parallel port was pretty easy in the old DOS days and also in Win95/98. We could use Inporb and outportb or \_inp() or \_Outp functions in our program without any problem if we are running the program on Dos or WIN95/98. However, with WIN NT4, WIN2000, WINXP, all this simplicity goes away. Being interested in Parallel port interfacing and programming you might have experienced the problems in writing a program that can talk to a parallel port successfully in NT based operating systems. When we are trying to run a program which is written using the conventional software functions like Inporb, outportb, \_inp() or \_Outp on a WINNT or WIN2000 system, it will show an error messageâ€¦ "The exception privileged instruction occurred in the application at location ....". The solution to this? Use Inpout32.dll for WIN 98/NT/2000/XP.
> 
> Using this dll in CCNET is straight forward using DllImport...
> 
> > public class PortAccess { \[DllImport("inpout32.dll", EntryPoint="Out32")\] public static extern void Output(int adress, int value); }
> 
> I added this class to the namespace ThoughtWorks.CruiseControl.CCTray in the CCNET solution and simply modified the SystemTrayMonitorClass.PlayBuildAudio() method.
> 
> private void PlayBuildAudio(BuildTransition transition) { try { \_settings.Sounds.PlayFor(transition);
> 
> // Set appropriate output state for parallel port Data 0 line // This controls the circuit to power the appropriate lava lamp :) if (transition == BuildTransition.Broken || transition == BuildTransition.StillFailing) { // Turn on the Red lava lamp PortAccess.Output(888, 0); CurrentBuildState \= 0; } else { // Turn on the Green lava lamp PortAccess.Output(888, 1); CurrentBuildState \= 1; }
> 
> catch (Exception ex)
> 
> // only display the first exception with audio if (\_audioException == null) { MessageBox.Show(ex.Message, "Unable to initialise audio", MessageBoxButtons.OK, MessageBoxIcon.Error); \_audioException \= ex; } } } And that's it! Simple!
> 
> P.S. Just make sure to connect the lava lamps to the correct AC connectors. :)
> 
> **Improvements**
> 
> I have subsequently added a second circuit to control a disco ball that turns on when there is a build in progress. The circuit is driven from the Data 1 line of the parallel port (pin 3). CCNET (SystemTrayMonitorClass.statusMonitor\_Polled() method ) was modified to control this parallel port line and maintain the state of the last known build.
> 
> private void statusMonitor\_Polled(object sauce, PolledEventArgs e) { \_exception \= null;
> 
> // update tray icon and tooltip trayIcon.Text \= CalculateTrayText(e.ProjectStatus); trayIcon.Icon \= \_iconLoader.LoadIcon(e.ProjectStatus).Icon;
> 
> // Are we building? if(e.ProjectStatus.Activity == ProjectActivity.Building) { // Keep the current lava lamp active so we know the last build state if(CurrentBuildState == 1) { // Turn on the build globe PortAccess.Output(888, 3); } else { // Turn on the build globe PortAccess.Output(888, 2); } } if (\_statusMonitor.Settings.RemoteServerUrl !\= \_lastUrl) InitialiseProjectMenu(); }

Thanks again Ken for sharing this!!!!

Here's a picture of the results:

![LavaLamps.jpg](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2005/08/build-status-using-lava-lamps-by-ken-nichols/images/LavaLamps.jpg)

Not surprisingly, the state is green. :)
