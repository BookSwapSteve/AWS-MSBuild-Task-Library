About
-----
MSBuild task(s) to publish files to S3 using MSBuild.

License
-------
MS-PL

Debugging
---------
In Project Properties->Debug
Start External Program -> c:\windows\Microsoft.NET\Framework\v3.5\MSBuild.exe
Command Line Argument -> Path_to\bin\Debug\PublishData.proj

More info on MSBuild tasks
--------------------------
http://bartdesmet.net/blogs/bart/archive/2008/02/15/the-custom-msbuild-task-cookbook.aspx

Usage
-----
You will need a AWS account with S3 enabled.  

On first pass modify the StoreKeys target to include your AWS keys.  ues MSBuild to execute that target
MSBuild PublishData.proj /t:StoreKeys

Once the keys have been stored, remove them from the file to prevent accidental checkin/distribution.

Subsequent uses, call the Debug target to copy files.
MSBuild PublishData.proj /t:Debug

Notes
-----
Don't use .'s in bucket names as they cause a "The remote certificate is invalid according to the validation procedure." error.

Limitations
-----------
This does not handle sub directories.

Tasks:
-----
Implement CloudFront tasks to allow publish/update of CloudFront files as part of MSBuild.

Notes:
-----
If you stored your keys with a version before 1.0.0.10 they will need to be updated as salting has been added to the container.
