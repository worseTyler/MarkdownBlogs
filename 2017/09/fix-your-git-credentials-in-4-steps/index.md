
# Struggling to get Git to connect to VisualStudio.com? Here's a possible solution.

_Tip: A proxy/firewall can be one of the causes of this frustration._

A big shout out to Andrew Scott for working this with me.

## 1\. Install the Git Credential Manager for Windows

Go to [https://github.com/Microsoft/Git-Credential-Manager-for-Windows/releases](https://github.com/Microsoft/Git-Credential-Manager-for-Windows/releases), or you can also get this from the Git Installer for Windows at [https://git-scm.com/download/win.](https://git-scm.com/download/win)

## 2\. Get your Personal Access Token

Go to [xxx.visualstudio.com](https://xxx.visualstudio.com) where 'xxx' is your Visual Studio Online tenant name.

Click on your user icon then choose “Security.” You should now be on the Personal Access Token page.

Click the “Add” button then type a description and select “Other Options.” Choosing an expiration of a year will reduce the pain of having to do this process more often.

Click “Create Token” at the bottom. Now you should see your token.

**Copy this**. _Note: that your token will only display once._

## 3\. Add your credential to the Windows Credential Manager

Open the Credential Manager in Windows by searching for “Credential Manager” from the Start menu.

Click “Add a Generic Credential.”

Add your Internet or network address: git:https://\[your vs site name\].[visualstudio.com](https://visualstudio.com).

Your user name is “Personal Access Token” (yes, this is the user name).

Paste your personal access token as the “Password.”

## 4\. Use Visual Studio or your tool of choice to clone your repo

These steps should get you up and running!

Note: You may have to periodically clean these items out if things stop working for you. You can always create a new token and make a new entry in the Credential Manager.

_What other issues would you like the IntelliTect team to break down for you?_

###### [Sign up for our quarterly newsletter](https://bit.ly/2Nhro9T) [![](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2017/09/fix-your-git-credentials-in-4-steps/images/Click-here-to-sign-up-1-300x69.jpg)](https://bit.ly/2Nhro9T)
