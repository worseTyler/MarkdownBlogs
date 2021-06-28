
Estimated reading time: 3 minutes

I recently made a web services call into WebMethods using basic authentication.  This authentication meant that we needed to modify the WSDL generated classes to handle the authentication.

Here's how it works.  I add a reference to the Web Service (Visual Studio generates the client code for calling the web service).  Then, to this generated class I need to add the following method:

protected override System.Net.WebRequest GetWebRequest(Uri uri)
{
    HttpWebRequest request;
    request = (HttpWebRequest)base.GetWebRequest(uri);

    if (PreAuthenticate)
    {
        NetworkCredential networkCredentials =
        Credentials.GetCredential(uri, "Basic");

        if (networkCredentials != null)
        {
            byte\[\] credentialBuffer = new UTF8Encoding().GetBytes(
            networkCredentials.UserName + ":" +
            networkCredentials.Password);
            request.Headers\["Authorization"\] =
            "Basic " + Convert.ToBase64String(credentialBuffer);
        }
        else
        {
            throw new ApplicationException("No network credentials");
        }
    }
    return request;
}

This overrides the GetWebRequest() method of the System.Web.Services.Protocols.SoapHttpClientProtocol class that the web service client code derived from.

With [Visual Studio 2005](https://intellitect.com/accessibility-of-new-types-in-visual-studio-2005/) the generated code is a C# 2.0 partial class.  As a result, regenerating the web services client code does not over-write the additional method.  To enable this, add a class file to your project and give it the same namespace and name as the generated System.Web.Services.Protocols.SoapHttpClientProtocol derived class.  The key is to use the partial modifier on the class header so that the GetWebRequest() method is added to the generated class.  (**partial** class Michaelis.MockService{...})

Regardless of using Visual Studio.NET 2005 or earlier, the client code requires that the network credentials are set and the PreAuthenticate property is assigned true.  Here is a sample client call:

Michaelis.MockService service = new Michaelis.MockService();

// Create the network credentials and assign
// them to the service credentials
NetworkCredential netCredential = new NetworkCredential("Inigo.Montoya", "Ykmfptd");
Uri uri = new Uri(service.Url);
ICredentials credentials = netCredential.GetCredential(uri, "Basic");
service.Credentials = credentials;

// Be sure to set PreAuthenticate to true or else
// authentication will not be sent.
service.PreAuthenticate = true;

// Make the web service call.
service.Method();

### UPDATE for Calling Web Services - 4/14/2005

Comments on the post raised the question, "Why cant you just say request.Credentials = new NetworkCredential(username,password)."

The reason relates to interoperating with WebMethods specifically.  When just setting Credentials, the HTTP header looks like this:

> POST /soap/rpc HTTP/1.1  
> User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; MS Web Services Client Protocol 2.0.50113.0)  
> Content-Type: text/xml; charset=utf-8  
> SOAPAction: ""  
> Host: <servername>:<port>  
> Content-Length: 779  
> Expect: 100-continue  
> Accept-Encoding: gzip

Notice, there is no Authentication item even though PreAuthenticate is set to true.

The reply back from WebMethods is as follows:

> HTTP/1.0 500 Internal Server Error  
> Set-Cookie: ssnid=11747k5Rwchr3vW0s23vcaCP1wCA2NDc=555590; path=/;  
> Content-Type: text/xml;charset=utf-8  
> Connection: Keep-Alive  
> Content-Length: 849

The problem is that .NET is expecting a challenge response from WebMethods, specifically a 401 error of "Invalid credentials."  However, if the clientâ€™s credentials are not specified (there is not Authentication part to the header) then WebMethods returns an HTTP 500 status code (Internal Server Error) indicating that the request could not be fulfilled.

To fix the problem you can either change the .NET client or else the WebMethods server.  In my original posting, I demonstrated how to control the .NET client.  The result was an HTTP header that includes the Authentication portion as shown below:

> POST /soap/rpc HTTP/1.1  
> User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; MS Web Services Client Protocol 2.0.50113.0)  
> Authorization: BasicbGRwcm86bGRwcm8=  
> Content-Type: text/xml; charset=utf-8  
> SOAPAction: ""  
> Host: spo-wm-py-srvr:5555  
> Content-Length: 779  
> Expect: 100-continue  
> Accept-Encoding: gzip

However, it is also possible to change the WebMethods side (assuming you control that side) by creating an access controlled SOAP processor that checks the credentials for each client request against a specified ACL and returns an HTTP 401 status code even if there are no credentials passed.

By the way, the tool I use for tracing HTTP is [YATT](https://www.pocketsoap.com/yatt/).

### Want More?

Check out this blog on fully-managed [passwordless authentication](http://intellitect.com/passwordless-authentication-azure-sql/)!

![](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2005/03/calling-web-services-using-basic-authentication/images/Blog-job-ad-1024x127.png)
