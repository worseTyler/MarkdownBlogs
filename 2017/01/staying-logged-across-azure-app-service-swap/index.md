

## Staying Logged Across Azure App Service Swap 
#
I love being able to deploy to slots with Azure Standard and above App Services. These allow me a main production site, a slot for testing which has a separate database, and staging slot that shares a database with production.

### The Problem

Whenever I swap the production and stage sites, all the users of the production site get logged out. This is because the token encryption keys are independent for the production and staging slots. This means that the tokens for the production are invalid when stage becomes production thus effectively logging the user out. It does seem that Microsoft would provide a simpler solution for this, but I couldn't find one.

### The Solution

After much digging I was able to patch together several articles and come up with what I thought was a reasonable solution.

I needed a way to keep the same encryption keys across both the staging and production slots. Enter Azure Blob Storage. By setting up a storage location in Azure, I could keep the same key for both environments. And thanks to the highly configurable and dependency injected ASP.NET Core, I was able to configure Data Protection with the new storage location.

The resulting code is as follows

```csharp
// Add Data Protection so that cookies don't get invalidated when swapping slots.
string storageUrl = Configuration.GetValue<string>("DataProtection:StorageUrl");
string sasToken = Configuration.GetValue<string>("DataProtection:SasToken");
string containerName = Configuration.GetValue<string>("DataProtection:ContainerName");
string applicationName = Configuration.GetValue<string>("DataProtection:ApplicationName");
string blobName = Configuration.GetValue<string>("DataProtection:BlobName");

// If we have values for all these things set up the data protection store in Azure.
if (storageUrl != null && sasToken != null && containerName != null && applicationName != null && blobName != null)
{
    // Create the new Storage URI
    Uri storageUri = new Uri($"{storageUrl}{sasToken}");

    //Create the blob client object.
    CloudBlobClient blobClient = new CloudBlobClient(storageUri);

    //Get a reference to a container to use for the sample code, and create it if it does not exist.
    CloudBlobContainer container = blobClient.GetContainerReference(containerName);
    container.CreateIfNotExists();

    services.AddDataProtection()
        .SetApplicationName(applicationName)
        .PersistKeysToAzureBlobStorage(container, blobName);
}
```

The first section just reads configuration variables from the configuration provider. This could use Azure environment variables. If the variables exist, the blob storage URI is used to connect. Once connected with the client, a check is done to open the container if it exists or create it if it doesn't. Finally, the DataProtection service is added with the container and blob name. It was very nice that ASP.NET Core had a built in ability to use blob storage for this very case.

I also wanted to include a sample configuration, in this case in an appsetting.json format. This will give some idea of what the various setting should look like.

```javascript
{
  "DataProtection": {
    "ApplicationName": "AppName",
    "StorageUrl": "https://BlobName.blob.core.windows.net",
    "SasToken": "?sv=YYYY-MM-DD&ss=x&srt=xxx&sp=xxxxxx&se=YYYY-MM-DDTHH:MM:SSZ&st=YYYY-MM-DDTHH:MM:SSZ&sip=a.b.c.d-w.x.y.z&spr=https&sig=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "ContainerName": "data-protection-container-name", // All lower case with dashes and numbers.
    "BlobName": "data-protection-blob-name"
  }
}
```
