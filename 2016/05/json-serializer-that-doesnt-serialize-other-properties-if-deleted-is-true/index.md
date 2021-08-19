## Search for Serialization Without Serialization of Other Properties When Deleted 
#
I was recently working on a project using Azure Mobile Services (AMS) as the backend for our Android and iOS mobile apps. Using the built-in offline sync feature of AMS, you can delete records on the backend and when the mobile app synchronizes its data, the record will be deleted from the mobile device. While taking a look at some performance issues with our implementation, I discovered that the mechanism to delete a record actually sends the entire record back with the Deleted flag set to true.

```csharp
{
  "__version":"AAAAAAAAB9I=",    
  "__deleted":false,
  "__updatedAt":"2016-04-29T03:31:38.546Z",
  "__createdAt":"2016-04-29T03:31:38.538Z",
  "Id":"cf6e5a35-e5c4-4d54-a4d2-aee116d59bdd",
  "Complete":false,
  "text":"Second item"
},
{
  "__version":"AAAAAAAAB9Y=",
  "__deleted":true,
  "__updatedAt":"2016-04-29T03:32:11.63Z",
  "__createdAt":"2016-04-29T03:31:38.593Z",
  "Id":"dfd1ff45-9970-4f70-9320-efbcc05a3727",
  "Complete":false,
  "text":"First item"
}
```

The only fields required by the client to delete a record are the ‘Id’ and ‘Deleted’ fields - any other fields are unnecessary. While it may not seem like much in this example, if you have a table with a large binary field, sending the binary blob over the wire just so the record can be deleted off the mobile seems awfully inefficient.

This started my quest of figuring out how to customize AMS so that the additional fields would not be returned to the client. There are at least three different ways to solve this problem.

1. Only select ‘Id’ and ‘Deleted’ from the database. This would be ideal, because you’re saving CPU cycles by not even pulling the unwanted data from the database.
2. Pull all the data from the database, but set all the unwanted fields to null before or during the JSON serialization process. This isn’t ideal because we still have to pull all the data from the database, but is effective at keeping the data from being transferred over the wire.
3. Don’t try to mess with the OData query or values - instead get in the middle of the JSON serializer and exclude the unwanted fields from being serialized.

Unfortunately, figuring out how to filter out these extra fields from the AMS response was not as simple as I had hoped. However, my new best friend at Microsoft Brett Samblanet offered the following solution that implements the third option that I share with his permission.

The solution uses a few types that are internal to the AMS framework, so we have to copy them.

First, we need an IPropertyMapper class - this will be used to map “Deleted” to “__deleted”, etc.

```csharp
public class CustomPropertyMapper : IPropertyMapper
{
    private readonly IDictionary<string, string> map;
         
    public CustomPropertyMapper(IDictionary<string, string> map)
    {
        if (map == null)
        {
            throw new ArgumentNullException("map");
        }
        this.map = map;
    }
 
    /// <inheritdoc />
    public string MapProperty(string propertyName)
    {
        string value;
        if (this.map.TryGetValue(propertyName, out value))
        {
            return value;
        }
        return propertyName;
    }
}
```

Next, we need a customized JsonConverter class. This is mostly copied internal code from AMS. The interesting section is in lines 36-43 below:

```csharp
public class CustomSelectExpandWrapperConverter : JsonConverter
{
    private readonly IPropertyMapper propertyMapper = null;
 
    public CustomSelectExpandWrapperConverter()
    {
        IDictionary<string, string> map = new Dictionary<string, string>();
        map.Add("Deleted", "__deleted");
        map.Add("Version", "__version");
        map.Add("UpdatedAt", "__updatedAt");
        map.Add("CreatedAt", "__createdAt");
        this.propertyMapper = new CustomPropertyMapper(map);
    }
 
    public override bool CanConvert(Type objectType)
    {
        return objectType != null && typeof(ISelectExpandWrapper).IsAssignableFrom(objectType);
    }
 
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
 
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (serializer == null)
        {
            throw new ArgumentNullException("serializer");
        }
 
        ISelectExpandWrapper wrapper = value as ISelectExpandWrapper;
        if (wrapper != null)
        {
            var convertedValue = wrapper.ToDictionary((model, type) => this.propertyMapper);
            if ((bool)convertedValue["__deleted"])
            {
                var keysToRemove = convertedValue.Where(item => (item.Key != "__deleted") && (item.Key != "Id")).Select(item => item.Key).ToList();
                foreach (string key in keysToRemove)
                {
                    convertedValue.Remove(key);
                }
            }
            serializer.Serialize(writer, convertedValue);
        }
    }
}
```

The 'if' block on line 36 determines if the record is deleted, and if so, removes all fields other than ‘Id’ and ‘Deleted’.

We need to keep the AMS TableContractResolver logic intact because it sets up some Delta<T> handling. So here we swap out our internal SelectExpandWrapperConverter for our new one.

```csharp
public class CustomContractResolver : TableContractResolver
{
    public CustomContractResolver(MediaTypeFormatter formatter)
        : base(formatter)
    {
    }
 
    protected override JsonContract CreateContract(Type objectType)
    {
        JsonContract contract = base.CreateContract(objectType);
 
        if (typeof(ISelectExpandWrapper).IsAssignableFrom(objectType))
        {
            contract.Converter = new CustomSelectExpandWrapperConverter();
        }
 
        return contract;
    }
}
```

And finally, register the ContractResolver. Do this somewhere in your WebApiConfig.Register method.

```csharp
config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CustomContractResolver(config.Formatters.JsonFormatter);
```

And now, with all that in place, if you watch the raw data sent to the mobile app from the backend services you will see that the payload for deleted records has been reduced to include only the ‘Id’ and ‘Deleted’ fields.

```csharp
{
  "__version":"AAAAAAAAB9I=",
  "__deleted":false,
  "__updatedAt":"2016-04-29T03:31:38.546Z",
  "__createdAt":"2016-04-29T03:31:38.538Z",
  "Id":"cf6e5a35-e5c4-4d54-a4d2-aee116d59bdd","complete":false,
  "text":"Second item"
},
{
  "__deleted":true,
  "Id":"dfd1ff45-9970-4f70-9320-efbcc05a3727"
}
```

To see this in action, you can run the project from github, use the built-in ‘help’ pages to execute a GET tables/TodoItem command with the following URL:

> tables/TodoItem?__includeDeleted=true&__systemproperties=\*

[![headers_deleteditems](https://intellitect.com/wp-content/uploads/2016/05/headers_deleteditems.png)](https://intellitect.com/wp-content/uploads/2016/05/headers_deleteditems.png "JSON serializer that doesn’t serialize other properties if Deleted is true")

[https://github.com/creasewp/BlogSamples/tree/master/AMSDelete](https://github.com/creasewp/BlogSamples/tree/master/AMSDelete)

_Written by Wayne Creasey._
