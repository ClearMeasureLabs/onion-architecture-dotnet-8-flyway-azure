. .\BuildFunctions.ps1
. .\FlywayFunctions.ps1

$projectName = "ChurchBulletin"
$base_dir = resolve-path .\
$source_dir = "$base_dir\src"
$unitTestProjectPath = "$source_dir\UnitTests"
$integrationTestProjectPath = "$source_dir\IntegrationTests"
$acceptanceTestProjectPath = "$source_dir\AcceptanceTests"
$uiProjectPath = "$source_dir\UI\Server"
$databaseProjectPath = "$source_dir\Database"
$databaseFlywayProjectPath = "$source_dir\DatabaseFlyway"
$mauiProjectPath = "$source_dir\UI\Maui"
$projectConfig = $env:BuildConfiguration
$framework = "net8.0"
$version = $env:BUILD_BUILDNUMBER

$verbosity = "minimal"

$build_dir = "$base_dir\build"
$test_dir = "$build_dir\test"

$aliaSql = "$source_dir\Database\scripts\AliaSql.exe"
$flywayCliDir = "$base_dir\flyway"
$flywayCli = "$flywayCliDir\flyway.cmd"
$dockerSettingsDir = "$base_dir\Docker"

$databaseAction = $env:DatabaseAction
if ([string]::IsNullOrEmpty($databaseAction)) { $databaseAction = "Rebuild"}

#We will create 3x databases for Flyway
$databaseName = $projectName
if ([string]::IsNullOrEmpty($databaseName)) { $databaseName = $projectName}

#We may need three separate servers, but for initialization and/or testing, just use the same, root SQL Server for the 3x test Flyway dbs.
$script:databaseServer = $databaseServer
if ([string]::IsNullOrEmpty($script:databaseServer)) { $script:databaseServer = "(LocalDb)"}

$script:databaseServerInstance = $databaseServerInstance
if ([string]::IsNullOrEmpty($script:databaseServerInstance)) { $script:databaseServerInstance = "MSSQLLocalDB"}

$databaseScripts = "$source_dir\Database\scripts"

if ([string]::IsNullOrEmpty($version)) { $version = "1.0.0"}
if ([string]::IsNullOrEmpty($projectConfig)) {$projectConfig = "Release"}
 
Function Init {
	& cmd.exe /c rd /S /Q build
	
	mkdir $build_dir > $null

	exec {
		& dotnet clean $source_dir\$projectName.sln -nologo -v $verbosity
		}
	exec {
		& dotnet restore $source_dir\$projectName.sln -nologo --interactive -v $verbosity  
		}
	
	if ($migrateDbWithFlywayDocker) {
		Setup-FlywayDocker
	}

    Write-Output $projectConfig
    Write-Output $version
}

Function Compile{
	exec {
		& dotnet build $source_dir\$projectName.sln -nologo --no-restore -v `
			$verbosity -maxcpucount --configuration $projectConfig --no-incremental `
			/p:TreatWarningsAsErrors="true" `
			/p:Version=$version /p:Authors="Programming with Palermo" `
			/p:Product="Church Bulletin"
	}
}

Function UnitTests{
	Push-Location -Path $unitTestProjectPath

	try {
		exec {
			& dotnet test /p:CollectCoverage=true -nologo -v $verbosity --logger:trx `
			--results-directory $test_dir\UnitTests --no-build `
			--no-restore --configuration $projectConfig `
			--collect:"XPlat Code Coverage"
		}
	}
	finally {
		Pop-Location
	}
}

Function IntegrationTest{
	Push-Location -Path $integrationTestProjectPath

	try {
		exec {
			& dotnet test /p:CollectCoverage=true -nologo -v $verbosity --logger:trx `
			--results-directory $test_dir\IntegrationTests --no-build `
			--no-restore --configuration $projectConfig `
			--collect:"XPlat Code Coverage"
		}
	}
	finally {
		Pop-Location
	}
}

Function MigrateDatabaseLocal {
	param (
	    [Parameter(Mandatory=$true)]
		[ValidateNotNullOrEmpty()]
		[string]$databaseServerFunc,
		
	    [Parameter(Mandatory=$true)]
		[ValidateNotNullOrEmpty()]
		[string]$databaseServerInstanceFunc,
		
	    [Parameter(Mandatory=$true)]
		[ValidateNotNullOrEmpty()]
		[string]$databaseNameFunc
	)
	
	#Run Docker Migration
	if ($migrateDbWithFlywayDocker) {
		if ($databaseServerFunc -ceq "localhost")
		{
			Log-Message "Changing the serverName"
			$databaseServerFunc = "host.docker.internal"
		}
		else {
			Log-Message "Did not change the value"
		}
		Log-Message "ATTEMPTING TO EXEC DOCKER MIGRATION FOR $databaseNameFunc" "INFO"
				# -e "FLYWAY_URL=jdbc:sqlserver://192.168.1.14;instanceName=$databaseServerInstanceFunc;databaseName=$databaseNameFunc;encrypt=false;user=$databaseUserId;password=$databaseUserPassword" `
		exec {
			& docker run `
				-v "${databaseFlywayProjectPath}\migrations:/flyway/migrations" `
				-v "${dockerSettingsDir}:/flyway/conf" `
				-e "FLYWAY_URL=jdbc:sqlserver://$databaseServerFunc\$databaseServerInstanceFunc;databaseName=$databaseNameFunc;encrypt=false;user=$databaseUserId;password=$databaseUserPassword" `
				redgate/flyway migrate
		}
		#Drop/Create the DB
	} else {
		$databaseServerFull = "$databaseServerFunc\$databaseServerInstanceFunc"
		exec{
			& $aliaSql $databaseAction $databaseServerFull $databaseNameFunc $databaseScripts
		}
		Log-Message "** NOTICE ** The database was just dropped and re-created. Before running the Flyway Docker Migration, you will need to give your Sql login account owner priviledges on the new database." "WARNING"
	}
}

Function PackageUI {    
    exec{
        & dotnet publish $uiProjectPath -nologo --no-restore --no-build -v $verbosity --configuration $projectConfig
    }
	exec{
		& dotnet-octo pack --id "$projectName.UI" --version $version --basePath $uiProjectPath\bin\$projectConfig\$framework\publish --outFolder $build_dir  --overwrite
	}
}

Function PackageDatabase {    
    exec{
		& dotnet-octo pack --id "$projectName.Database" --version $version --basePath $databaseProjectPath --outFolder $build_dir --overwrite
	}
}

Function PackageAcceptanceTests {       
    # Use Debug configuration so full symbols are available to display better error messages in test failures
    exec{
        & dotnet publish $acceptanceTestProjectPath -nologo --no-restore -v $verbosity --configuration Debug
    }
	exec{
		& dotnet-octo pack --id "$projectName.AcceptanceTests" --version $version --basePath $acceptanceTestProjectPath\bin\Debug\$framework\publish --outFolder $build_dir --overwrite
	}
}

Function PackageScript {    
    exec{
        & dotnet publish $uiProjectPath -nologo --no-restore --no-build -v $verbosity --configuration $projectConfig
    }
	exec{
		& dotnet-octo pack --id "$projectName.Script" --version $version --basePath $uiProjectPath --include "*.ps1" --outFolder $build_dir  --overwrite
	}
}


Function Package{
	Write-Output "Packaging nuget packages"
	dotnet tool install --global Octopus.DotNet.Cli | Write-Output $_ -ErrorAction SilentlyContinue #prevents red color is already installed
    PackageUI
    PackageDatabase
    PackageAcceptanceTests
	PackageScript
}

Function PrivateBuild{
	$projectConfig = "Debug"
	[Environment]::SetEnvironmentVariable("containerAppURL", "localhost:7174", "User")
	$sw = [Diagnostics.Stopwatch]::StartNew()
	Init
	Compile
	UnitTests
	MigrateDatabaseLocal -databaseServerFunc $databaseServer -databaseServerInstanceFunc $databaseServerInstance -databaseNameFunc $databaseName
	IntegrationTest
	
	$sw.Stop()
	write-host "BUILD SUCCEEDED - Build time: " $sw.Elapsed.ToString() -ForegroundColor Green
}

Function CIBuild{
	$sw = [Diagnostics.Stopwatch]::StartNew()
	Init
	Compile
	UnitTests
	MigrateDatabaseLocal $databaseName
	IntegrationTest
	Package
	$sw.Stop()
	write-host "BUILD SUCCEEDED - Build time: " $sw.Elapsed.ToString() -ForegroundColor Green
}