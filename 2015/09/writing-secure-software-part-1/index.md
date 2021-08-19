## Part 1: Writing Secure Software
#
In today’s world of online theft and cyber attacks, writing secure software is more important than ever. For many developers, however, security is typically an afterthought, if it is considered at all. In the next few blogs, I’m going to walk through one possible process for developing secure software in an attempt to demystify the process.

**Security by design**

Ideally, security should be architected into software from day one. Waiting until your project is nearly complete before doing a threat analysis on the system design can cost substantially more due to the amount of refactoring necessary to mitigate those threats. Identifying and mitigating potential security threats early in your development process will save you countless hours.

‘Security by design’ means that you take steps to protect your software from known security threats, but also expect a malicious user to attack your system and take the appropriate measures to minimize the impact if a security vulnerability is exploited. One process for developing secure software follows these steps:

- Understand potential threats
- Create a threat model
- Identify potential threats
- Prioritize the potential threats
- Mitigate potential threats
- Maintaining security over time

**Understand Potential Threats**

The first step to properly analyze security threats to your software is to understand the potential threats. A popular acronym for potential security threats is “S.T.R.I.D.E.”.

- **Spoofing Identity** \- Pretending to be a user other than yourself.
- **Tampering with Data** - Malicious modification of data. This could be done directly in a database, or modification of data as it is transferred between a client and the server.
- **Repudiation** - Occurs when a user makes a change or initiates some action in the system, but later denies performing the action and the system lacks the necessary tracking or audit logs to prove otherwise.
- **Information Disclosure** \- Exposure of data to users that should not be allowed to view it
- **Denial of service** \- These attacks aim to deny service to valid users.
- **Elevation of privilege** \- A scenario where a user has gained privileged access to the system and can perform actions to compromise the entire system.

**Create A Threat Model**

Keeping the 6 classifications of STRIDE in mind, you can then evaluate your system architecture looking for potential risks. A good way to visualize this is to develop a threat model. The intent behind the threat model diagram is to create a visual representation of the components of your system and the interactions between them. Let’s take a look at a simple example.



This system is composed of a web service, a client that interacts with the service and a database from which the web service serves data.. For this example, I’ve specified a Windows Communication Foundation (WCF) service although you could think of this as any web service. The lines between each component represent the interactions between components. The dashed red lines represent trust boundaries. The trust boundaries indicate which processes can trust each other. Any line that crosses a trust boundary must be analyzed as a potential security vulnerability.

Obviously, this is a very high level diagram. For each of the components, you should create additional diagrams with more detail. Figure 2 is an example of a more detailed diagram of the WCF Service component.



In this case, you can see that I chose to draw a trust boundary between the service and its configuration file because the configuration file is outside of the compiled code. We may decide later that this is a low risk area, but it is important to include it on the initial diagram to enumerate as many potential threats as possible.

**Identify Potential Threats**

Once we have developed our threat model diagrams, it is time to identify potential threats. Applying the STRIDE acronym for each component can help identify potential threats. Ask yourself questions like “How might an attacker spoof a valid identity here?” or “Could someone tamper with data sent or received?”. Treat this phase like a brainstorming session - write down all potential risks, no matter how unlikely. The prioritization phase will help identify which risks are worthwhile to address.

While this list of potential threats is not meant to be exhaustive, hopefully it gives you an idea of things to consider.

_WCF Client_

1. WCF Client settings might be stored in a config file. This file could be tampered with to redirect the client to a man in the middle attack.
2. Data sent across the wire to the WCF Service could be visible to an attacker using a packet capture tool.
3. Is authentication with the WCF Service required? Does the user provide those credentials? Or are they embedded in the code?
4. Data sent from the WCF Client could be redirected to an alternate server and captured without the user’s knowledge.
5. A malicious user might do a memory dump of the client process, looking for sensitive information.

_WCF Service_

1. WCF settings file could be tampered with to reduce security on an endpoint.
2. Data in the incoming request might attempt a SQL injection attack.
3. Assuming the WCF Service requires authentication, the caller might have stolen user credentials and are not who they say they are.
4. If the database is remote, a malicious user might monitor the traffic between the WCF Service and the database.
5. If the service is running as an administrative level user and the service is compromised, it could be used to perform more destructive actions on the server or even network.

In the [next blog](/writing-secure-software-part-2/), I will discuss how to prioritize these potential risks and develop strategies for mitigating the high-priority risks.

_Written by Wayne Creasey._
