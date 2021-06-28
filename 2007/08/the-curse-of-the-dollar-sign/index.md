
Recently, I was working on a project that inserted records from a flat file in to a SQL table. For the most part, this was pretty simple and straight forward.

1. Create Flat File Schema (using the wizard)
2. Create SQL Insert Schema (again, using the wizard)
3. Create Mapping between file and SQL schemas.
    1. This was only slightly trickier as I had multiple source schemas. Not sure why you must begin those types of maps within the Orchestration editor, but that is another topic.

1. String it all together inside an Orchestration and deploy.

Things were moving along very well, and due to the simplicity of the integration, I didn't expect any errors.

UNTIL… All of a sudden, I began receiving this error message.

An error occurred while processing the message, refer to the details section for more information Message ID: {5D53CDDD-518A-41CE-920C-763A691EEACF} Instance ID: {FC64BC77-A22A-4FDA-BF1C-9A1F181230FC} Error Description: HRESULT="0x80004005" Description="Invalid XML elements found inside sync block" ?<Root xmlns:ns00="urn:schemas-microsoft-com:xml-updategram"><?MSSQLError HResult="0x80004005" Source="Microsoft XML Extensions to SQL Server" Description="Invalid XML elements found inside sync block"?></Root>

My variety of Google searches turned up very few viable hits and none provided many clues.

Ultimately, I wrote a quick method to read in the original file and stream it out to individual files, one record per file. Yes, I realize I could have used pipelines in BizTalk to handle the splitting/debatching of the file, but I have my reasons. Again, that will probably be another topic.

Once I began to run the individual files through BizTalk, I was able to identify exactly which record was causing my error. As it turns out, there was an attribute containing the value of "$some text". This attribute was of type xs:string and represented VARCHAR(50) column in the table.

Further research lead me to [http://msdn2.microsoft.com/en-us/library/ms946341.aspx](https://msdn2.microsoft.com/en-us/library/ms946341.aspx) which has a topic entitled **Using money data type columns in updategrams** which discusses some nuances of the MONEY data type and how it is represented in XSD as xs:decimal.

My final result was to use a scripting functoid to replace the dollar sign ($) with an empty string. Again, I was surprised to see the absence of the String.Replace() functoid.

Sorry I don't have any concrete solution as not all scenarios will allow you to remove the $. As time permits, I plan to revisit this issue and hopefully provide a better solution.
