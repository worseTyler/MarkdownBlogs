

## Downloading Attachments From TFS
#
A few weeks ago I was supporting a client who had attached a significant number of files to various work items in a project and wanted to be able have them all in a folder. The time required to download these files by hand seemed daunting. Rumors around the office were that someone on the team might be able to do this with some code. As it turned out there were several teams that needed this and they had some specific requirements:

1. Each team needed a different subset of work items based multiple values in multiple fields.
2. They needed the filename to contain both the work item number and the name of the file for traceability.

Additionally, the work item types in TFS were completely customized with many custom fields and values. I searched around the internet for a premade solution, but of course couldn’t find one at the time, and hence this blog post. This post targets an on-premise TFS server using Active Directory authentication.

## Set Up References

The first thing was to find the dll references I needed to connect to TFS. It turns out that two are needed and available via Nuget packages. As of this writing, the version number was 12.x.

```
nuget-bot.Microsoft.TeamFoundation.Client
nuget-bot.Microsoft.TeamFoundation.WorkItemTracking.Client
```

```csharp
using System.Net;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
```

## Connect to TFS

The next step is to set up the connection to the TFS Team Project Collection. We need to attach to the collection because this is where the Work Item Store is located. This services all projects. This code snippet assumes that the TFS server is named ‘tfs’.

```csharp
TfsTeamProjectCollection tpc = 
      TfsTeamProjectCollectionFactory.GetTeamProjectCollection(
      new Uri("https://tfs:8080/tfs/defaultcollection));
```

Next we need to connect to the work item store in the Team Project Collection.

```csharp
WorkItemStore workItemStore = new WorkItemStore(tpc);
```

## Get the Work Items

Now we can run our query. The TFS query syntax is like SQL, but not exactly. I have added several parameters here so the options would be clear. For those familiar with Work Item Type customization, the names used can either be the display names (Responsible Group below) or the field names (System.AttachedFileCount below). If this seems a bit daunting, you can use Visual Studio to create a query and then view the ‘SQL’ by choosing Save As from the File menu and saving the query to a .wiq file.

```csharp
WorkItemCollection queryResults =
  workItemStore.Query(
  @"SELECT \*
  FROM WorkItems
  WHERE [Work Item Type] = 'Defect'
  AND [Status] IN ('New','Open','In-Progress', 'Pending Vendor', 'Re-open')
  AND [Responsible Group] IN ('AMS Delivery', 'AMS Ops')
  AND [System.AttachedFileCount] > 0
  ORDER BY [Changed Date] DESC");
```

## Download and Save the Attachments

Now that we have the query results, we can iterate through them and pull the relevant data. In this case it is an attachment which doesn’t come along in the query results by default. The first step is to set up a WebClient that will download the attachments. UseDefaultCredentials is set to true because we are using Active Directory authentication. There are other authentication options.

```csharp
WebClient webClient = new WebClient()
{
    UseDefaultCredentials = true
};
```

We are now able to iterate the list of Work Items and get the list of attachments from each. Then in turn each attachment can be downloaded. In this case, I am prefixing the filename of the attachment with the value of another field for requisite traceability.

```csharp
foreach (WorkItem workItem in queryResults)
{
  foreach (Attachment attachment in workItem.Attachments)
  {
    string filename = string.Format("D:\\\\Defects\\\\{0}-{1}",
    workItem.Fields["HP QC ID"].Value, attachment.Name);
    webClient.DownloadFile(attachment.Uri, @"D:\\Defects\\" + filename);
  }
}
```

That should give you a folder full of work item attachments that meet the specified query parameters. The TFS API is a great tool for automating TFS tasks and this is only one small aspect.

## Full Source Code

```csharp
private static void ExportVso()
{
  // Connection to the Team Project Collection
  TfsTeamProjectCollection tpc = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(
    new Uri("https://https://idahopower.visualstudio.com/defaultcollection"));
  
  // Get a WorkItemStore object for the Team Project Collection.
  WorkItemStore workItemStore = new WorkItemStore(tpc);

  // Run a query.
  WorkItemCollection queryResults = workItemStore.Query(
    @"SELECT \*
    FROM WorkItems
    WHERE [Work Item Type] = 'Defect'
    AND Status IN ('New','Open','In-Progress', 'Pending Vendor', 'Re-open')
    AND [Responsible Group] IN ('AMS Delivery', 'AMS Ops')
    ORDER BY [Changed Date] DESC");

  // Get a WebClient object to do the attachment download
  WebClient webClient = new WebClient()
  {
    UseDefaultCredentials = true
  };
  
  // Loop through each work item.
  foreach (WorkItem workItem in queryResults)
  {
    // Loop through each attachment in the work item.
    foreach (Attachment attachment in workItem.Attachments)
    {
      // Construct a filename for the attachment
      string filename = string.Format("D:\\\\Defects\\\\{0}-{1}", workItem.Fields["HP QC ID"].Value, attachment.Name);
      // Download the attachment.
      webClient.DownloadFile(attachment.Uri,filename);
    }
  }
}
```
