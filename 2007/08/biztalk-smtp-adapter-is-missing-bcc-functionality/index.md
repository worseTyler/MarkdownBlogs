---
title: "BizTalk SMTP Adapter is Missing BCC Functionality"
date: "2007-08-07"
categories: 
  - "adapters"
  - "biztalk"
  - "blog"
tags: 
  - "adapters"
  - "biztalk"
---

Ok, so this probably comes to no surprise to many of you.  I remember running in to this problem when I was using BTS 2004.  However, I thought that the community's cries would be answered with BTS 2006.  I have not had a need for it until now, so I never checked, but again the ability to BCC using the SMTP Adapter does not exist.  Fortunately, there are many ways to skin this cat.  The first 2 that come to mind are:

1. Create 2 separate email messages within BizTalk: 1 for your original recipients, and 1 for your BCC recipients
    1. Don't forget to tell them they received this as a BCC and list the original recipients.
    2. Yeah... won't end up biting you in the butt later on.
2. Create a referenced assembly to manage your SMTP needs.
    1. Again, there are tons of SMTP DLLs floating around in the ether, but you can easily create your own with very little code.  Depending how basic your needs are, you can accomplish it with very few lines of code.
        
        > using System.Net.Mail;
        > 
        > SmtpClient client = new SmtpClient("server"); using (MailMessage message = new MailMessage("from", "to", "subject", "Body")) { client.Send(message); }
        
        But if your needs were that simple, you could just use the SMTP Adapter.

Anyway, I whipped up some code that would handle my needs.  I did not need to support attachments, although it shouldn't be too difficult to modify my code to include them.

Essentially, I created 2 classes:  SMTPHelper and SMTPMessage.

SMTPMessage contains the necessary property values used to send an email. SMTPHelper contains a single method (SendMail) that accepts a SMTPMessage parameter.  SendMail uses the values within the SMTPMessage object to create SmtpClient and MailMessage objects and perform the necessary actions to deliver the email.  Basic error handling is included, but can surely be expanded upon.

Within BizTalk, I created a variable called smtpMessage of type BAJ.Utilities.SMTPMessage.  Inside my orchestration, I placed the following code within an expression shape.

> smtpMessage = new BAJ.Utilities.SMTPMessage();
> 
> smtpMessage.SMTPServer = smtpServer; smtpMessage.FromAddress = smtpFromAddress; smtpMessage.FromName = smtpFromName; smtpMessage.ToList = strToList; smtpMessage.CCList = ""; smtpMessage.BCCList = strBCCList; smtpMessage.Subject = "\[enter subject here\]"; smtpMessage.Body = "\[enter message here\]"; smtpMessage.IsHTML = true; smtpMessage.Priority = System.Net.Mail.MailPriority.Normal;
> 
> BAJ.Utilities.SMTPHelper.SendMail(smtpMessage);

In my process, smtpServer, smtpFromAddress, and smtpFromName are all string variables whose values are stored as AppSettings.

Here is the code for my SMTPHelper class:

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

namespace BAJ.Utilities
{
    \[Serializable\]
    public class SMTPHelper
    {
        public static void SendMail(SMTPMessage email)
        {
            try
            {
                //System.Diagnostics.EventLog.WriteEntry("SMTPHelper - DEBUG",
                //    "Here are the SMTPMessage property values:" + Environment.NewLine +
                //    "SMTPServer: " + email.SMTPServer + Environment.NewLine +
                //    "FromName: " + email.FromName + Environment.NewLine +
                //    "FromAddress: " + email.FromAddress + Environment.NewLine +
                //    "ToList: " + email.ToList + Environment.NewLine +
                //    "CCList: " + email.CCList + Environment.NewLine +
                //    "BCCList: " + email.BCCList + Environment.NewLine +
                //    "Subject: " + email.Subject + Environment.NewLine +
                //    "IsHTML: " + email.IsHTML + Environment.NewLine +
                //    "Priority: " + email.Priority.ToString() + Environment.NewLine);

                SmtpClient client = new SmtpClient(email.SMTPServer);
                MailMessage message = new MailMessage();
                MailAddress fromAddress = new MailAddress(email.FromAddress, email.FromName);

                email.ToList = email.ToList.Replace(";", ",");
                email.CCList = email.CCList.Replace(";", ",");
                email.BCCList = email.BCCList.Replace(";", ",");

                if (0 < email.BCCList.Length) { message.Bcc.Add(email.BCCList); }
                message.Body = email.Body;
                if (0 < email.CCList.Length) { message.CC.Add(email.CCList); }
                message.From = fromAddress;
                message.IsBodyHtml = email.IsHTML;
                message.Priority = email.Priority;
                message.Subject = email.Subject;
                if (0 < email.ToList.Length) { message.To.Add(email.ToList); }

                client.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    \[Serializable\]
    public class SMTPMessage
    {
        private string \_SMTPServer;

        /// <summary>
        /// The network address of the SMTP server. \[required\]
        /// </summary>
        public string SMTPServer
        {
            get { return (null == \_SMTPServer ? String.Empty : \_SMTPServer); }
            set { \_SMTPServer = value; }
        }
        private string \_FromName;

        /// <summary>
        /// The name of the person sending the message. \[optional\]
        /// </summary>
        public string FromName
        {
            get { return (null == \_FromName ? String.Empty : \_FromName); }
            set { \_FromName = value; }
        }
        private string \_FromAddress;

        /// <summary>
        /// The email address of the sender. \[required\]
        /// </summary>
        public string FromAddress
        {
            get { return (null == \_FromAddress ? String.Empty : \_FromAddress); }
            set { \_FromAddress = value; }
        }
        private string \_ToList;

        /// <summary>
        /// The address(es) recipients.  Multiple recipients should be separated by comma (,) or semi-colon (;). \[optional\]
        /// A value for ToList, CCList, or BCCList must be provided
        /// </summary>
        public string ToList
        {
            get { return (null == \_ToList ? String.Empty : \_ToList); }
            set { \_ToList = value; }
        }
        private string \_CCList;

        /// <summary>
        /// The address(es) recipients.  Multiple recipients should be separated by comma (,) or semi-colon (;). \[optional\]
        /// A value for ToList, CCList, or BCCList must be provided
        /// </summary>
        public string CCList
        {
            get { return (null == \_CCList ? String.Empty : \_CCList); }
            set { \_CCList = value; }
        }
        private string \_BCCList;

        /// <summary>
        /// The address(es) recipients.  Multiple recipients should be separated by comma (,) or semi-colon (;). \[optional\]
        /// A value for ToList, CCList, or BCCList must be provided
        /// </summary>
        public string BCCList
        {
            get { return (null == \_BCCList ? String.Empty : \_BCCList); }
            set { \_BCCList = value; }
        }
        private string \_Subject;

        /// <summary>
        /// The subject line of the email. \[optional\]
        /// </summary>
        public string Subject
        {
            get { return (null == \_Subject ? String.Empty : \_Subject); }
            set { \_Subject = value; }
        }
        private string \_Body;

        /// <summary>
        /// The message content of the email. \[optional\]
        /// </summary>
        public string Body
        {
            get { return (null == \_Body ? String.Empty : \_Body); }
            set { \_Body = value; }
        }
        private bool \_IsHTML;

        /// <summary>
        /// Determines if the body of the email is HTML. \[default - false\]
        /// </summary>
        public bool IsHTML
        {
            get { return \_IsHTML; }
            set { \_IsHTML = value; }
        }
        private MailPriority \_Priority = MailPriority.Normal;

        /// <summary>
        /// Determines the priority of the email. \[default - MailPriority.Normal\]
        /// </summary>
        public MailPriority Priority
        {
            get { return \_Priority; }
            set { \_Priority = value; }
        }
    }
}
