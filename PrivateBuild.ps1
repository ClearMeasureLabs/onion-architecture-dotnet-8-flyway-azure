param (

	# SQL Server
    [Parameter(Mandatory=$false)]
    [ValidateNotNullOrEmpty()]
    [string]$databaseServer = "(LocalDb)",
	
	# SQL Server Instance
    [Parameter(Mandatory=$false)]
    [ValidateNotNullOrEmpty()]
    [string]$databaseServerInstance = "MSSQLLocalDB",
	
	# Set to #true if you need to add an sa account to the Db.  NOTE, it must first be created within your instance of Sql Server
    [Parameter(Mandatory=$false)]
    [ValidateNotNullOrEmpty()]
    [bool]$databaseAddLoginPermissions = $false,
	
	# Database SA account in SQL Server
    [Parameter(Mandatory=$false)]
    [ValidateNotNullOrEmpty()]
    [string]$databaseUserId = "flyway_sa",
	
	# Database SA account password in SQL Server
    [Parameter(Mandatory=$false)]
    [ValidateNotNullOrEmpty()]
    [string]$databaseUserPassword = "pwd",
	
    [Parameter(Mandatory=$false)]
    [ValidateNotNullOrEmpty()]
    [bool]$migrateDbWithFlywayDocker = $false
)

. .\build.ps1

PrivateBuild