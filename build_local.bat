:: This is a copy of build.bat, but passing in local parameters for the developer workstation. 
:: parameters
::  -databaseServer - Your local SQL Server Instance, if not a default Instance
::  -migrateDbWithFlyway - Pass in $true if you want to run the Flyway migration demo. After AliaSQL creates the ChurchBulletin db, Flyway will add a column, as a test migration.

@echo off
setlocal

:: Step #1 - Run this to build the Db through AliasSql
powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "& { .\PrivateBuild.ps1 -databaseServer 'RLHDELL01' -databaseServerInstance 'SQLEXPRESS' %*; if ($lastexitcode -ne 0) {write-host 'ERROR: $lastexitcode' -fore RED; exit $lastexitcode} }"

:: Step #2 
::   - Comment the line above uncomment this one and run again.
::   - Open Sql Server and add a new users, if it does not exist, named "flyway_sa" and make the password "pwd"
::   - Provide "Owner" rights to the flyway_sa account in Sql Server to the database(s).
::   - Last, uncomment the line below and re-run to get migrations from Flyway with Docker

:: 192.168.1.14
::powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "& { .\PrivateBuild.ps1 -databaseServer 'localhost' -databaseServerInstance 'SQLEXPRESS' -databaseAddLoginPermissions $true -migrateDbWithFlywayDocker $true %*; if ($lastexitcode -ne 0) {write-host 'ERROR: $lastexitcode' -fore RED; exit $lastexitcode} }"

		
