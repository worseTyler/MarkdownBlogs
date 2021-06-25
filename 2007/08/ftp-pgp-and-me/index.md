---
title: "FTP, PGP, and Me"
date: "2007-08-07"
categories: 
  - "biztalk"
  - "blog"
  - "pipeline-components"
tags: 
  - "biztalk"
  - "pipeline-components"
---

So let me break this down for you...

Company A (Acme, Inc) wants to exchange data with Company C (Charlie Company).  However, I represent Company B (BizTalk United) and we want to collect some of the data as well.  A requirement has been made that all data will be transmitted using FTP and will also be encrypted.  So, as the broker of the integration, I must resolve how to get data from point A to C and still be able to read the data myself.

The solution:

- Company A will encrypt their file using the public key from Company B and transmit the file via FTP.
- Company B will decrypt the file using their private key, encrypt the file again using the public key from Company C and transmit the file via FTP.

Sounds simple enough once you get passed the PGP Pipeline Component.  However, there is another interesting wrinkle in this process.  Because both companies (A and C) want to prevent retrieving a file from the FTP server in mid-stream, they have decided to upload a 0 byte file immediately after posting the data file.  The existence of the 0 byte file indicates the data file is ready for download.  Once again, you can certainly update your FTP ReceiveLocation properties to only get the 0 byte files based on the proper mask, but how do you get the actual .pgp file from the FTP Server?

Here is how I did it.  Please let me know if you have any other recommendations or suggestions as I am always open to improvement.

**1 - Download the 0 byte file (and PGP file)** I download the 0 byte file from the FTP server using the FTP receive adapter.  The message is received by an Orchestration, and all processes execute from within this single Orchestration.  Upon receipt of the file, I strip off the FTP server name, port, directory, and filename from the BTS.InboundTransportLocation and FILE.ReceivedFileName message properties.  These values, along with some appSettings keys containing the FTP credentials, are used to execute a FTP download request using the [FtpWebRequest class](https://msdn2.microsoft.com/en-us/library/system.net.ftpwebrequest(VS.80).aspx).  Luckily, the 0 byte file and the data file share the same filename with the exception of the extension.  The downloaded file is stored on the local hard drive in a temporary location of your choosing.

**2 - Decrypt the PGP file** Once the PGP file is stored locally, I used some code ([courtesy of MSDN](https://msdn2.microsoft.com/en-us/library/aa995576.aspx)) to load the file in to an XLANGMessage.  With my PGP file now safely tucked away in a BizTalk message (of type XmlDocument of course), I can use the ExecuteReceivePipeline method to disassemble the encrypted message in to plain text.

[![clip_image0012_thumb](images/clip_image0012_thumb.png "clip_image0012_thumb")](/wp-content/uploads/2009/07/clip_image00122.png)

Note:  This must be done within an Atomic scope.

The Disassemble shape contains the following code:

> pipeOutput = Microsoft.XLANGs.Pipeline.XLANGPipelineManager.ExecuteReceivePipeline( typeof(Namespace.Pipelines.Receive.Decrypt), msgEncrypted);

The Decrypt File shape contains the following code:

> pipeOutput.MoveNext(); msgDecrypted = new System.Xml.XmlDocument(); pipeOutput.GetCurrent(msgDecrypted);

And the Name File shape simply strips the ".pgp" from the filename.

Now I have a decrypted version of the file.

**3 - Re-Encrypt the decrypted file** Since I have a message containing the decrypted version of the file, I can use the ExecuteSendPipeline method to encrypt the file for Company C.

[![clip_image0013_thumb[4]](images/clip_image0013_thumb4.png "clip_image0013_thumb[4]")](/wp-content/uploads/2009/07/clip_image00132.png)

Note:  This must be done within an Atomic scope.

The Assemble File shape contains the following code:

> msgEncrypted\_New = new System.Xml.XmlDocument();
> 
> pipeInput.Add(msgDecrypted);
> 
> Microsoft.XLANGs.Pipeline.XLANGPipelineManager.ExecuteSendPipeline( typeof(Namespace.Pipelines.Send.Encrypt), pipeInput, msgEncrypted\_New);

The Name File shape simply appends the ".pgp" to the filename.

**4 - Upload the Encrypted File (and 0 byte file)** Now that I have a newly encrypted file, I can use the FTP send adapter to transfer the files.  Obviously, I will first send the PGP file, and using the same port, transfer the 0 byte file.

**5 - Process the data** Once I have dealt with the housekeeping of decrypting, encrypting and transferring files from A to C, I am able to process my data.  Similar to the process used in Step 2, I will disassemble the file within the Orchestration and farm out the work from there.

**Sub-Topic: Oh crap, the decrypted file is empty** I found out the hard way that Company A was going to send an encrypted file and 0 byte file, regardless of whether the encrypted file, once decrypted, actually contained data.  This started throwing massive kinks into my process.  The main error I received was:

> Inner exception: The part 'part' of message 'msgDecrypted' contains zero bytes of data. Exception type: EmptyPartException Source: Microsoft.XLANGs.Engine

After banging my head against a brick wall for 2 days, I came up with a workaround (HACK) that accomplishes my goal.

Instead of decrypting the file inside the orchestration, I am using a dynamic port along with the decrypt pipeline to create a physical version of the file in a temporary location.  I then get the length of the file to determine my next steps.

- If the file length > 0, I use the IStreamFactory method to construct my decrypted message.
- If the file length = 0, I simply assign the 0 byte file (from the initial receive) to my decrypted message.

Now I am able to proceed successfully.  Once all files have been Decrypted, Encrypted, and transferred successfully, I delete the local copies from the hard drive.

Of the many things I tried with unsuccessful results, this one still puzzles me as to why it didn't work.  From what I can tell, once the EmptyPartException bell has been rung, you can't un-ring it.

I used an exception handler to catch the Microsoft.XLANGs.BaseTypes.EmptyPartException.  This error was only thrown if I reached a persistence point within my scope.  In my exception handler, I attempted to reassign the decrypted message with either the initial receive message, and empty XmlDocument, a dummy XmlDocument, contents from another file, but regardless of how I tried to assign the value, once the error was raised, I couldn't get passed it.

Again, please let me know if you see a better way

of handling this, I am always looking for improvement.
