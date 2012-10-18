properties {
  $testMessage = 'Executed Test!'
  $compileMessage = 'Executed Compile!'
  $cleanMessage = 'Executed Clean!'
  $solutionName = 'Simple.Logging.sln'
  $build_dir = Split-Path $psake.build_script_file	
  $nugetPackagesDirectory = Join-Path $build_dir "GeneratedPackages"
  $version = 0.1
}

task default -depends NugetPack

Task NugetPack -Depends Build {
    if((Test-Path $nugetPackagesDirectory) -eq $false){
        New-Item $nugetPackagesDirectory -type directory
    }
    gci  *.nuspec | 
        ForEach-Object {
            $expression = ".\.nuget\nuget.exe pack {0} -Build -OutputDirectory {1} -version {2} -Symbols" -f $_.Name, $nugetPackagesDirectory, $version
            Invoke-Expression $expression
        }
}

task Build -depends Clean { 
	Exec { msbuild $solutionName /t:Build /p:Configuration=Release} 
}

task Clean { 
	Exec { msbuild $solutionName /t:Clean /p:Configuration=Release} 
}