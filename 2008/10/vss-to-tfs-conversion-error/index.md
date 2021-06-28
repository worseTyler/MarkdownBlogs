
Recently I had the pleasure of migrating several VSS databases to Team Foundation Server 2008.  One of the databases was pretty large (90k+ files) and took between 12 - 16 hours to migrate.  However, about 6 hours in to the migration, I kept running across an error that would halt the migration process.

**The error:** Could not find file 'C:\\Conv\\Tfs\\<TeamProjectName>\\blah\\blah\\somefile.txt'.

After searching and searching for a solution, I finally ran across this [forum post](https://social.msdn.microsoft.com/Forums/en-US/tfsversioncontrol/thread/60803fbb-33bc-4983-8b03-6c6ff901e5af/) that got me back on track again.  While I cannot take credit for the solution, I would like to add some additional information.

Essentially, [Visual Studio Team Foundation Server 2008](https://msdn.microsoft.com/en-us/tfs2008/default.aspx) will not allow files that use the DOS 8.3 naming convention.  For whatever reason, the VSS database had a reference to the file using the "fully qualified" name as well as the DOS 8.3 name.

During the migration process, VssConvert creates a database on the SQL Server, specified in your MigrationSettings.xml file, called **VSStoVersionControlConverterDB**.  Within this database, there are about 4 tables, but the ones you need to pay attention to are **SCMHistory** and **VerificationTable**.  Based on what I experienced, the SCMHistory table is populated when VssConvert  begins scanning the VSS database for files.  Once the migration piece is actually underway, the VerifcationTable is populated.

As the forum post indicates, you need to delete the file that is causing your error from both of these tables.  What it doesn't say is how...

> DECLARE @fileName VARCHAR(255) SET @fileName = '$/VssRoot/blah/blah/somefi~1.txt'
> 
> DELETE FROM SCMHistory WHERE ItemName = @fileName DELETE FROM VerificationTable WHERE ItemName = @fileName

The only concern that I had was that I didn't want to fix this problem, rerun the migration, and have it fail again in another 8 hours because of some other @#$% file.  While I had the opportunity, I tried a few more options.

At first, I figured I should just delete anything with a ~ in the filename, because that just isn't normal. :)

> SELECT \* FROM SCMHistory WHERE ItemName LIKE '%~%' SELECT \* FROM VerificationTable WHERE ItemName LIKE '%~%'

Fortunately, I ran SELECT statements first because, to my surprise, there were a ton of files that contained ~.  Some of the files started with ~ and others were based on the DOS 8.3 format.  However, not all of the files using DOS 8.3 format were raising errors as I had verified they migrated over successfully.  I suppose the problem really only exists if a single file is stored in the VSS database in both formats. I did notice that the only time I got a migration error was when the DOS 8.3 file was in VerificationTable.

Ultimately, I decided to run this SQL script and it worked like a charm.

> DELETE FROM SCMHistory WHERE ItemName IN (SELECT DISTINCT ItemName FROM VerificationTable WHERE ItemName LIKE '%~%') DELETE FROM VerificationTable WHERE ItemName LIKE '%~%'

Of course, this is what worked for ME.  You may encounter a different scenario, so be careful to see what you will be deleting before you delete it.  Once you get the error, you should have the opportunity to do an incremental migration provided you didn't serialize your migration scripts. ;)

Also, assuming you want to incrementally migrate, you can open a connection to the VSStoVersionControlConverterDB within SQL and the migration will raise an error that the database could not be deleted.  This will allow you to do some research on your files and prepare for what needs to be deleted during your next migration attempt.

**_NOTE:_** Depending on the number of files being converted, you may have to be very quick on the draw in order to delete your contaminated database records.
