
## Why pay for something you can write yourself?

As I mentioned in my previous blog, [_PGP Pipeline Component_](/pgp-pipeline-component/), I needed to perform some PGP encryption and decryption of files. I had two stipulations:

1. I didn't want to pay for it when I knew I could produce the same results for free. Specifically, I didn't want to buy another BizTalk book
2. I wanted to use something that could be automatically deployed from machine to machine

Since then, I ran across a few things that I needed to update and have posted them here as well.

Notable changes include:

- Added PGP Pipeline Component property of Extension.  This allows you to specify what extension you want to place at the end of your encrypted file.  The default value is PGP.
- Added capability to decrypt a signed message.
- Updated decryption to handle other than .PGP extension.  Previously hard coded to remove only the .pgp from the filename.
- Updated TestFixture form to be more user friendly.  You can now specify where you want your output file to be generated.
- Minor code changes that don't necessarily affect logic, but may improve performance.

Link to readme.txt: [readme.txt](/wp-content/uploads/2009/07/readme.txt) Link to dll: [BAJ.BizTalk.PipelineComponent.PGP.dll](/wp-content/uploads/2009/07/BAJ.BizTalk.PipelineComponent.PGP.dll) Link to source code:  [PGP.zip](/wp-content/uploads/2009/07/PGP.zip)

\[**UPDATED - 7/28/2009**\] - File locations have been updated and should be available for downlaod.

\[**UPDATED - 9/11/2007**\] - It was brought to my attention that I did not include instructions for obtaining the crypto.dll file.  In my original [post](https://bajwork.blogspot.com/2007/08/pgp-pipeline-component.html), I mentioned that you had to download the Bouncy Castle source code as I didn't feel it was appropriate for me to distribute it.  Also, you will need to strongly name the assembly.  I have updated the readme.txt file with the same message.  Sorry for any confusion.
