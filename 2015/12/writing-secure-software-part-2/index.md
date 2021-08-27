

## Part 2: Writing Secure Software 
#
In my [last blog post](/writing-secure-software-part-1/), I discussed one possible process for developing secure software. We learned about potential security threats using the S.T.R.I.D.E. acronym, creating a threat model and identifying potential threats. Next, I’ll discuss prioritizing the potential threats you have identified and discuss ways to mitigate these threats.

**Prioritizing Potential Threats**

In my last blog post I used a simple system as an example to create a threat model and identify potential threats. Not all of the potential security threats that we brainstormed are necessarily worth our time and effort to address - but how do we know which ones are worth tackling? One method of analyzing potential threats uses another handy acronym: D.R.E.A.D.

- **Damage** - If this threat is exploited, how much damage will occur?
- **Reproducibility** - Is it easy to reproduce the exploit?
- **Exploitability** \- What is needed to exploit this threat?
- ****Affected Users**** \- How many users will be affected?
- **Discoverability** - Is the threat easy to discover?

For each potential threat, you assign a score in each of these areas, then sum these scores for each threat. The threats with the highest scores become your highest priority risks to address. Let’s look at a few examples using a simple High (3), Medium (2) Low (1) scheme.

**One of the potential threats we came up with last time was:**

> **Data sent across the wire to the WCF Service could be visible to an attacker using a packet capture tool.**

Let’s assess this threat in each of the D.R.E.A.D. categories.

<table><tbody><tr><td>Category</td><td>Notes</td><td>Score</td></tr><tr><td>Damage</td><td>Sensitive data such as user passwords could be compromised, leading to further compromises in security</td><td>High (3)</td></tr><tr><td>Reproducibility</td><td>This threat is reproducible every time.</td><td>High (3)</td></tr><tr><td>Exploitability</td><td>Packet capture tools are freely downloadable.</td><td>High (3)</td></tr><tr><td>Affected Users</td><td>All.</td><td>High (3)</td></tr><tr><td>Discoverability</td><td>Without taking appropriate steps, this is easily discoverable.</td><td>High (3)</td></tr><tr><td>Total</td><td></td><td>15</td></tr></tbody></table>

**A second potential threat we identified was:**

> **A malicious user might do a memory dump of the client process, looking for sensitive information.**

<table><tbody><tr><td>Category</td><td>Notes</td><td>Score</td></tr><tr><td>Damage</td><td>This depends on the nature of the client application\, but let’s suppose that the user types in an authentication password which is stored in memory. An attacker could use that password to gain access to the system.</td><td>Medium (2)</td></tr><tr><td>Reproducibility</td><td>This threat is reproducible every time.</td><td>High (3)</td></tr><tr><td>Exploitability</td><td>Getting a memory dump for a process is fairly easy, but parsing through it to find sensitive information could be difficult.</td><td>Low (1)</td></tr><tr><td>Affected Users</td><td>Probably only affects this one user whose password was stored in memory</td><td>Low (1)</td></tr><tr><td>Discoverability</td><td>Without direct access to the client machine, or installing some malicious software, this would be difficult to exploit.</td><td>Low (1)</td></tr><tr><td>Total</td><td></td><td>8</td></tr></tbody></table>

**A third potential threat was:**

> **Data in the incoming request might attempt a SQL injection attack.**
> 
> <table><tbody><tr><td>Category</td><td>Notes</td><td>Score</td></tr><tr><td>Damage</td><td>If exploited, SQL injection attacks can be very damaging.</td><td>High (3)</td></tr><tr><td>Reproducibility</td><td>This threat is reproducible every time.</td><td>High (3)</td></tr><tr><td>Exploitability</td><td>Successfully exploiting this threat probably requires a skilled programmer.</td><td>Medium (2)</td></tr><tr><td>Affected Users</td><td>If exploited, it could potentially affect all users.</td><td>High (3)</td></tr><tr><td>Discoverability</td><td>Getting the syntax correct for a successful attack can be tricky.</td><td>Medium (2)</td></tr><tr><td>Total</td><td></td><td>13</td></tr></tbody></table>
> 
> As you evaluate your potential threats, you are likely to find a wide range of threats. Some will be able to inflict a great deal of damage, but require expert knowledge in some area in order to exploit. Others may be easy to discover, but only risk leaking trivial information. Using the principles behind the D.R.E.A.D. acronym can help you to identify the high risk threats that should be discussed in the next phase - security threat mitigation.
> 
> _Written by Wayne Creasey._
