

## Azure Server-less
#
**Hybrid and multi-cloud scenarios are present in all companies to some degree, so consider utilizing Azure server-less technologies within Azure to migrate simple to complex projects to the cloud.**

If you're not leveraging the cloud in your company, then you're behind. Azure server-less can help your company stay current or maybe even get ahead.

Utilizing Azure server-less saves time and enhances developer productivity by eliminate the need for you, the developer, to provision the operating systems and hardware required to run your code. You don't have to worry about the servers running your code because Azure abstracts them away. In addition, every service has security and "intelligent" suggestions for improving performance and reducing spend and increasing security built int.

_\*NOTE - You can augment applications with add ons and separately deployed services._

### What Sets Azure Apart from other Cloud Providers?

Providing cloud services isn't the primary function of Azure's main competitors: Amazon and Google. Amazon is an online retailer or "e-tailer" and Google is an advertising company.

Microsoft Azure provides:

- Thousands of services and solutions, including Azure server-less solutions
- Spends $1 billion on security annually
- Has 54 regions worldwide that support data sovereignty requirements as well as Government Cloud specifically for use by government institutions

Azure is always improving, is the primary focus of its parent company and is simple to work with.

Let's see what we can accomplish with some demos! Here's the **full video** from my Azure server-less talk.

<iframe src="https://www.youtube.com/embed/A-Yp-QCyswI" allowfullscreen width="560" height="315"></iframe>

### What We'll Cover:

In this talk, we'll cover server-less functionality via demos as well as some new Azure capabilities. We'll quickly build a simple micro services-based solution leveraging Azure features.

![](https://intellitect.com/wp-content/uploads/2019/11/Serverless-Microservice-1024x562.png)

#### Service Bus

The [Service Bus](https://serverless.com/framework/docs/providers/azure/events/servicebus/) is a cloud-hosted, public, publish and subscribe service that lives in Azure. You provision the service and are only concerned with service configuration, not with the underlying servers required to run the service.

<iframe src="https://www.youtube.com/embed/rBm1twC6pYM" allowfullscreen width="560" height="315"></iframe>

Features include:

- Cloud-hosted service bus
- Securely connect to on-prem resources
- Scalable “push/pull”
- Client platform-independent: Rest API and SDK .NET, Java, Node, Python, etc.
- Support for transactions and security
- Use of EventGrid to trigger events
- Topic based messages and queues

Azure Storage Queues are another option for sending and receiving messages. Storage queues are durable and persistent but are not as performant and do not directly support guaranteed sequencing, forwarding, dead letters, etc.

#### Azure Functions

[Functions](https://azure.microsoft.com/en-us/services/functions/) is an event-driven server-less compute platform for application development that can also solve complex orchestration issues.

<iframe src="https://www.youtube.com/embed/AQGY6P3vx_s" allowfullscreen width="560" height="315"></iframe>

You can send something simple and small out the door very quickly using Azure's flexible deployment model.

This tool is cheap, easy and comes with a lot of CPU time right out of the box. However, if you need something more sophisticated, you might want to consider premium versions of Azure Function implementations.

Be aware that it's so easy to work with that you can create an unmanageable solution-sprawl unless you're thinking ahead. Remember, there are other services for big-ticket enterprise architecture projects.

Azure Functions support with multiple languages, including C#, Java, PowerShell, JavaScript, Python, F# and others.

_\*Note - the Function host lags a bit behind currently available technology. Core 3.0 support is in preview._

#### Logic Apps

Azure [Logic Apps](https://docs.microsoft.com/en-us/azure/logic-apps/logic-apps-overview) is a workflow engine hosted in Azure. Microsoft Flow (now "Power Automate") is built on top of Azure Logic Apps. Flow is part of Office 365 and is the replacement for SharePoint workflows.

<iframe src="https://www.youtube.com/embed/8vlesV_awTM" allowfullscreen width="560" height="315"></iframe>

Logic Apps is a server-less component that requires no code but is still fairly sophisticated.

_\*Note - A little known secret is that with an extension, Logic Apps can be edited in Visual Studio._

#### App Service

Azure [App Service](https://azure.microsoft.com/en-us/services/app-service/) allows users to build and deploy web apps that can be run on your local machine or in Azure.

<iframe src="https://www.youtube.com/embed/RvskZ8DFngY" allowfullscreen width="560" height="315"></iframe>

Microsoft claims that they spend a [billion dollars annually](https://www.microsoft.com/security/blog/2017/08/02/5-reasons-why-microsoft-should-be-your-cybersecurity-ally/) on security. That effort is visible in the API and present yet invisible in perimeter protection, security staff, in each data center across all 54 regions, includes network security and physical security, as well as the protection of each resource.

This security is also prevalent in App Service. A few years ago, security was "below the fold" so to speak - now they want developers to think about security all the time. App Service is continuously looking for threats, so if it detects something wrong with your configuration, or an attack on your server, Azure will attempt to mitigate the attack and it will let you know.

#### Azure SQL

<iframe src="https://www.youtube.com/embed/6LKr94U2_pg" allowfullscreen width="560" height="315"></iframe>

[Azure SQL](https://azure.microsoft.com/en-us/free/sql-database/search/?&ef_id=EAIaIQobChMIptfvo6XW5QIVLB6tBh1I3w_CEAAYASAAEgLFc_D_BwE:G:s&OCID=AID2000128_SEM_X03LYNV7&MarinID=X03LYNV7_287508794136_azure%20sql_e_c__46775456579_kwd-294748416662&lnkd=Google_Azure_Brand&gclid=EAIaIQobChMIptfvo6XW5QIVLB6tBh1I3w_CEAAYASAAEgLFc_D_BwE) is the cloud-hosted version of Microsoft SQL Server that continues to add features and functions like:

- Data protection
- Encryption at rest
- Data masking and transparent data encryption
- Data security

#### App Insights

<iframe src="https://www.youtube.com/embed/Q7WYw7KEtV8" allowfullscreen width="560" height="315"></iframe>

Consider adding the critical service, [App Insights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview), especially when deploying to Azure because it monitors your live web app and provides real-time, sophisticated analytics and events.

### Want More Azure Server-less?

That was as much Azure as can fit in a short presentation, but a talk on Azure isn't complete without mentioning Containers, Kubernetes or Micro Services, so consider further exploration by:

- Reading IntelliTect's recent [blog on Kubernetes](https://intellitect.com/kubernetes/)
- Following Microsoft's [Azure blog](https://azure.microsoft.com/en-us/blog/) for more information
- [Checking out Azure](https://azure.microsoft.com/en-us/free/) for free
- Utilizing the Visual Studio Subscriber's [monthly credit](https://my.visualstudio.com )

Feel free to ask me about Azure in the comments, also here's the [link to all the code](https://github.com/IntelliTect-Samples/2019-10-23.VisualStudio2019Launch) for IntelliTect's Visual Studio 2019 event.
