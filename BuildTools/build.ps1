param(
	[Parameter(Position=0)][string[]]$tasksToRun
	)
$testOutput = ""
$solutionPath = "..\TokenAuth.sln"
$dotnetExePath = "C:\Program Files\dotnet\dotnet.exe"
$pathToOpenCoverExe = (Get-Location).Path + "\OpenCover"
function test {
	if(proceed){
        Log("Executing 'test'", "info")
		$testOutput = .$dotnetExePath test $solutionPath
        Log("Done executing 'test'", "info")
        return $testOutput
	}
}

function writeTestResultsJson {
	if(proceed){
        Log("Executing 'Write Test Results Json'", "info")
        $testsPassed = 0;
        $totalTests = 0;
        $testsFailed = 0;
        $testsSkipped = 0;
        $testsRunTime = 0.0;
        foreach($testResult in ($testOutput | Select-String -Pattern "^Total tests: (.*). Passed: (.*). Failed: (.*). Skipped: (.*).$" -AllMatches)){
            Write-Host $testResult
            $totalTests += $testResult.Matches.Groups[1].Value
            $testsPassed += $testResult.Matches.Groups[2].Value
            $testsFailed += $testResult.Matches.Groups[3].Value
            $testsSkipped += $testResult.Matches.Groups[4].Value
        }
        $allTestsPassed = $testsPassed -eq $totalTests;

        foreach($runtime in ($testOutput | Select-String -Pattern "Test execution time: (.*) Seconds" -AllMatches)){
            $testsRunTime += [double] $runtime.Matches.Groups[1].Value
        }
        Log($matches, "debug")
		$json = @{
            TestsPassing= $allTestsPassed
			TotalTests= $totalTests
			TestsPassed= $testsPassed
			TestsFailed= $testsFailed
			TestsSkipped= $testsSkipped
            ExecutionTime= $testsRunTime.ToString() + " Seconds"
            TestsPercentage= $testsPassed.ToString() +"/"+ $totalTests.ToString()
		} | ConvertTo-Json
		Set-Content .\Reports\UnitTestsSummary.json $json
        Log("Done with 'Write Test Results Json'", "info")
	}
}

function build {
	if(proceed){
        Write-Host "Executing 'build'"
		.$dotnetExePath build $solutionPath
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
		.$dotnetExePath build $solutionPath /p:DebugType=Full

        if(proceed){
            Write-Host "Analyzing with OpenCover"
		    # running opencover        
	 	    .\OpenCover\OpenCover.Console.exe -target:$dotnetExePath -targetargs:"test $solutionPath --configuration Debug --no-build" -filter:"+[*]* -[*.Test*]*" -oldStyle -register:user -output:".\Reports\OpenCover.xml";
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
    .$dotnetExePath clean $solutionPath
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


