Param(
[string]$sourcesDirectory
)
Write-Host (Get-Item -Path ".\" -Verbose).FullName;
Write-Host $sourcesDirectory
$filter = "+[*]* -[*.Test*]*";
Write-Host $filter;
#.\OpenCover.Console.exe 
.\OpenCover.Console.exe -target:"C:/Program Files/dotnet/dotnet.exe" -targetargs:"test ..\..\MisturTee.TestMeFool\MisturTee.TestMeFool.csproj --configuration Debug --no-build" -filter:$filter -oldStyle -register:user -output:"$sourcesDirectory\OpenCover.xml" | Write-Host;
#.\OpenCover.Console.exe -target:"C:/Program Files/dotnet/dotnet.exe" -targetargs:"test ..\..\MisturTee.TestMeFool\MisturTee.TestMeFool.csproj --configuration Debug --no-build" -filter:"+[*]* -[*.Test*]*" -oldStyle -register:user -output:"..\..\OpenCover.xml";