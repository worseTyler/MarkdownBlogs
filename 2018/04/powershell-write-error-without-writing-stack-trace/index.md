

Recently, I was trying to display the errors and warning from a DotNet Build. While the warnings all displayed correctly, the errors always included the stack trace:

![](https://intellitect.comhttps://intellitect.com/wp-content/uploads/2018/04/2018-04-16_21-25-30-1024x226.webp)

I was able to control the error output slightly by varying the global $ErrorView variable, but the only two options were NormalView or CategoryView, neither of which gave me the clean, message-only, look I desired.

# Solution

To resolve the problem I wrote a Write-ErrorMessage function. The essence of the function was to write out the error message using $Host.UI.WriteErrorLine() and to also call Write-Error -ErrorAction SilentlyContinue so as to populate the error stack, $error (see Listing 1).

```
Function Write-ErrorMessage {
      \[CmdletBinding(DefaultParameterSetName='ErrorMessage')\]
      param(
           \[Parameter(Position=0,ParameterSetName='ErrorMessage',ValueFromPipeline,Mandatory)\]\[string\]$errorMessage
           ,\[Parameter(ParameterSetName='ErrorRecord',ValueFromPipeline)\]\[System.Management.Automation.ErrorRecord\]$errorRecord
           ,\[Parameter(ParameterSetName='Exception',ValueFromPipeline)\]\[Exception\]$exception
      )

      switch($PsCmdlet.ParameterSetName) {
      'ErrorMessage' {
           $err = $errorMessage
      }
      'ErrorRecord' {
           $errorMessage = @($error)\[0\]
           $err = $errorRecord
      }
      'Exception'   {
           $errorMessage = $exception.Message
           $err = $exception
      }
      }

      Write-Error -Message $err -ErrorAction SilentlyContinue
      $Host.UI.WriteErrorLine($errorMessage)
};

Write-ErrorMessage  "Error message"
Write-Error "This is a sample error" -ErrorAction SilentlyContinue # Log an error
Write-ErrorMessage $error\[0\]
Write-ErrorMessage (New-Object Exception  "Exception message")
```

The last four lines (above) verified the expected behavior which I ran both in PowerShell for Windows, Bash for Windows and ISE.

Note that one drawback to this approach was that  $Host.UI.WriteErrorLine()  didn't write to StdError so I couldn't redirect the error with something like  $Host.UI.WriteErrorLine() 2>&1. The reason was that $Host.UI.WriteErrorLine() is a UI method (like Write-Host ) and did not affect the pipeline.

# Colorizing the Output

An alternative approach to solve this issue is to change the console color, write the text to the output and then change the color back. However, this doesn't populate the $error stack and likely has unintended consequences for output colorization later on in the pipeline.

One other problem is that the $host.UI.RawUI colors use System.Media.Color in ISE and ConsoleColor in the standard command line. As a result, a fairly cumbersome (ugly) conversion between the color data types is required. Here's an attempt at the function:

```
Function Format-ColorizeOutput {
\[CmdletBinding()\]
param(
\[ValidateSet('Warning','Error',$null)\]\[AllowNull()\]\[AllowEmptyString()\]\[string\]$severity,
\[Parameter(ValueFromPipeline)\]$output
)
try{
# NOTE: Do not cast use System.ConsoleColor as on occasions, such as ISE, the data type is System.Windows.Media.Color
$consoleForegroundColor = $host.UI.RawUI.ForegroundColor
$consoleBackgroundColor = $host.UI.RawUI.BackgroundColor
switch ($severity) {
'Warning' {
$host.UI.RawUI.ForegroundColor = $host.PrivateData.WarningForegroundColor
$host.UI.RawUI.BackgroundColor = $host.PrivateData.WarningBackgroundColor
}
'Error' {
$host.UI.RawUI.ForegroundColor = $host.PrivateData.ErrorForegroundColor
$host.UI.RawUI.BackgroundColor = $host.PrivateData.ErrorBackgroundColor
}
}
Write-Output $output
}
finally {
$host.UI.RawUI.ForegroundColor = $consoleForegroundColor
$host.UI.RawUI.BackgroundColor = $consoleBackgroundColor
}
}

```

Note: this is not compatible with ISE (But with Visual Studio Code, does anyone care that much anymore?).
