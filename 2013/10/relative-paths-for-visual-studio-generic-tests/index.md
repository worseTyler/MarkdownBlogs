

To make generic tests robust enough to run for all developers on your team and on the build server, you are likely going to want to avoid hard coding the path to the executable.  To do this successfully, you need to have a list of all the environment variables that are available when the test executes.  These can be obtained by creating a generic test with the **existing program** as %COMSPEC% (the fully pathed location for cmd.exe) and setting "/C set" as the **Commnd-line arguments**.  The result, with all standard environment variables pulled out, is as follows:

AgentId=1
AgentLoadDistributor=Microsoft.VisualStudio.TestTools.Execution.AgentLoadDistributor
AgentName=<ComputerName>
AgentWeighting=100
ControllerName=localhost:6901
COR\_ENABLE\_PROFILING=1
COR\_PROFILER={<UUID1>}
DataCollectionEnvironmentContext=Microsoft.VisualStudio.TestTools.Execution.DataCollectionEnvironmentContext
DeploymentDirectory=<SolutionDirectory>\\TestResults\\<TestRunDirectory>\\Out
MSBuildLoadMicrosoftTargetsReadOnly=true
MSBuildTreatAllToolsVersionsAsCurrent=true
PkgDefApplicationConfigFile=<LOCALAPPDATA>\\Microsoft\\VisualStudio\\12.0\\devenv.exe.config
ResultsDirectory=<SolutionDirectory>\\TestResults\\<TestRunDirectory>\\In
TestDeploymentDir=<SolutionDirectory>\\TestResults\\<TestRunDirectory>\\Out
TestDir=<SolutionDirectory>\\TestResults\\<TestRunDirectory>\\
TestLocation=<ProjectDirectory>
TestLogsDir=<SolutionDirectory>\\TestResults\\<TestRunDirectory>\\In\\<ComputerName>
TestOutputDirectory=<SolutionDirectory>\\TestResults\\<TestRunDirectory>\\In\\<UUID2>\\<ComputerName>
TestResultsDirectory=<SolutionDirectory>\\TestResults\\<TestRunDirectory>\\In\\<UUID2>\\<ComputerName>
TestRunDirectory=<SolutionDirectory>\\TestResults\\<TestRunDirectory>\\
TESTRUNNER\_DATACOLLECTOR\_INSTANCE=<UUID3>
TestRunResultsDirectory=<SolutionDirectory>\\TestResults\\<TestRunDirectory>\\In\\<ComputerName>
TotalAgents=1
VisualStudioDir=<UserProfile>\\Documents\\Visual Studio 2013
VisualStudioEdition=Microsoft Visual Studio Ultimate 2013
VisualStudioVersion=12.0
VSLANG=1033
VSLOGGER\_CPLAN=<LOCALAPPDATA>\\Microsoft\\VisualStudio\\12.0\\TraceDebugger\\Settings\\qorgnywy.n4l
VSTS\_PROFILER\_NOT\_CLEAR\_ENVVARS=1
\_\_UNITTESTEXPLORER\_VSINSTALLPATH\_\_=C:\\Program Files (x86)\\Microsoft Visual Studio 12.0\\Common7\\IDE\\

Where I have substituted the following values:

<SolutionDirectory>: The hard coded path to the full solution directory
<TestRunDirectory>: The unique MSTest generated directory for each test run
<UUIDX>: A unique id value.
<ProjectDirectory>: The directory of the project that contains the generic test.
<ComputerName>: The name of the computer executing the tests.
<LocalAppData>: The users local app data directory.
<UserProfile>: The directory of the users local profile.

Of these, the most important in my experience is %TestLocation% as this identifies the directory of the test project, and therefore, you can find relative directories from that.  For example, if you want to find a file located in the nuget packages directory you would use

> %TestLocation%\\..\\packages\\
