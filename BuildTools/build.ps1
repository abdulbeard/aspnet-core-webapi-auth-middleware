param(
	[Parameter(Position=0)][string[]]$tasksToRun
	)
$testOutput = ""
function test {
	if(proceed){
        Log("Executing 'test'", "info")
		$testOutput = ."C:\Program Files\dotnet\dotnet.exe" test "..\TokenAuth.sln"
        Log("Done executing 'test'", "info")
        return $testOutput
	}
}

function writeTestResultsJson {
	if(proceed){
        Log("Executing 'Write Test Results Json'", "info")
		$matches = ($testOutput | Select-String -Pattern "^Total tests: (.*). Passed: (.*). Failed: (.*). Skipped: (.*).$").Matches
        Log($matches, "debug")
		$testsSuccess = ($testOutput | Select-String -Pattern "Test Run Successful.").Matches.Success
		$testsRunTime = ($testOutput | Select-String -Pattern "Test execution time: (.*)").Matches.Groups[1].Value
		$json = @{
            TestsPassing= $testsSuccess
			TotalTests= $matches.Groups[1].Value
			TestsPassed= $matches.Groups[2].Value
			TestsFailed= $matches.Groups[3].Value
			TestsSkipped= $matches.Groups[4].Value
            ExecutionTime=$testsRunTime
            TestsPercentage= $matches.Groups[2].Value+"/"+$matches.Groups[1].Value
		} | ConvertTo-Json
		Set-Content .\Reports\UnitTestsSummary.json $json
        Log("Done with 'Write Test Results Json'", "info")
	}
}

function build {
	if(proceed){
        Write-Host "Executing 'build'"
		."C:\Program Files\dotnet\dotnet.exe" build "..\TokenAuth.sln"
        Write-Host "Done executing 'build'"
	}
}

function runCodeCoverageAnalysis {
	if(proceed){
        Write-Host "Executing 'Code Coverage Analysis'"

        Write-Host "Cleaning solution first"
        clean

        Write-Host "Doing debug/pdb build"
		# running build with full debug type 
		."C:\Program Files\dotnet\dotnet.exe" build "..\TokenAuth.sln" /p:DebugType=Full

        if(proceed){
            Write-Host "Analyzing with OpenCover"
		    # running opencover        
	 	    .\OpenCover\OpenCover.Console.exe -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:"test ..\TokenAuth.sln --configuration Debug --no-build" -filter:"+[*]* -[*.Test*]*" -oldStyle -register:user -output:".\Reports\OpenCover.xml";
        }

        if(proceed){
            Write-Host "Generating html report and badges"
	 	    # generating coverage reports	 	
		    .\ReportGenerator_3.1.2.0\ReportGenerator.exe -reports:".\Reports\OpenCover.xml" -targetdir:"Reports" -reporttypes:"Html;Badges"
        }        
        Write-Host "Done executing 'Code Coverage Analysis'"
	}
}

function clean {
    ."C:\Program Files\dotnet\dotnet.exe" clean "..\TokenAuth.sln"
}

function proceed {
	return ($? -eq $True -or $lastExitCode -ge 0)
}

function Log($message, $level){
    if($level -eq "info"){
        Write-Host $message -ForegroundColor Green
    }
    if($level -eq "debug"){
        Write-Host $message -ForegroundColor Blue
    }
    if($level -eq "error"){
        Write-Host $message -ForegroundColor Red
    }
    
}

if($tasksToRun -eq $null){
	build
    $testOutput = test
    writeTestResultsJson
    runCodeCoverageAnalysis
}
else{
    foreach($task in $tasksToRun){
        if($task -eq "build"){
            build
        }
        elseif($task -eq "runCodeCoverageAnalysis"){
            runCodeCoverageAnalysis
        }
        elseif($task -eq "writeTestResultsJson"){
            writeTestResultsJson
        }
        elseif($task -eq "test"){
            $testOutput = test
        }
    }
    Write-Host $testOutput
}


