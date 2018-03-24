Write-Host (Get-Item -Path ".\" -Verbose).FullName;
.\OpenCover.Console.exe 
.\OpenCover.Console.exe -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:"test ..\..\MisturTee.TestMeFool\MisturTee.TestMeFool.csproj --configuration Debug --no-build" -filter:"+[*]* -[*.Test*]*" -oldStyle -register:user -output:"..\..\OpenCover.xml";