# Introduction 
TODO: Give a short introduction of your project. Let this section explain the objectives or the motivation behind this project. 

This project will create all of the required infrastructure in Azure programatically. The resoureces in the TDD environment will be torn down after automated testing is completed. The UAT and Prod resources will remain. There is an Octopus variable **EnsureEnvironmentsExist** that will tell Octopus to create all of the resources. If the variable is set to **True** Octopus will create all of the resources, if the variable is set to something else, Octopus will not create the resources. **EnsureEnvironmentsExist** should always be set to **True** for the TDD environment. This variable should be set to **False** (or anything other than **True**) for UAT and Prod to save time and preserve the existing resources during subsequent deployments.

# Onion Architecture Azure .NET 9/8 Getting Started
- [Github](#github)
- [Azure](#azure)
  - [Create an Azure Container Registry](#create-an-azure-container-registry)
  - [Connect Azure to Octopus Deploy](#connect-azure-to-octopus-deploy)
- [Octopus Deploy Environment Setup:](#octopus-deploy-environment-setup)
- [Octopus Deploy Project Setup:](#octopus-deploy-project-setup)
  - [Connect Octopus to GitHub](#connect-octopus-to-github)
  - [Create a new Version Controlled Project:](#create-a-new-version-controlled-project)
  - [Optional-Create a runbook for availability monitoring](#optional-create-a-runbook-for-availability-monitoring)
- [Azure DevOps Setup:](#azure-devops-setup)
  - [Create Service Connections](#create-service-connections)
  - [Create an artifact feed](#create-an-artifact-feed)
  - [Authorize the Pipeline to push packages to the feed](#authorize-the-pipeline-to-push-packages-to-the-feed)
  - [Add the Azure DevOps Feed to Octopus](#add-the-azure-devops-feed-to-octopus)
  - [Create the Library Variable Group](#create-the-library-variable-group)
  - [Grant the pipeline access to the variable group](#grant-the-pipeline-access-to-the-variable-group)
  - [Create a Pipeline](#create-a-pipeline)
- [Octopus Deploy Runbook Setup:](#octopus-deploy-runbook-setup)
  - [Create ContainerAppReplicas variable](#create-containerappreplicas-variable)
  - [Create Scale Up Runbook](#create-scale-up-runbook)
  - [Create Scale Down Runbook](#create-scale-down-runbook)
  - [Publish the runbooks](#publish-the-runbooks)
  - [Create Scheduled Runbook Triggers](#create-scheduled-runbook-triggers)


Requirements:

- Octopus Deploy
- Azure
- Github
- Azure DevOps

 
# Github
Create a new repository using the  [onion-architecture-maui-azure-dotnet-8](https://github.com/ClearMeasureLabs/onion-architecture-maui-azure-dotnet-8) template repo

# Azure

## Create an Azure Container Registry

[https://learn.microsoft.com/en-us/azure/container-registry/container-registry-get-started-portal?tabs=azure-cli](https://learn.microsoft.com/en-us/azure/container-registry/container-registry-get-started-portal?tabs=azure-cli)

Name the resource group something identifiable and different than the application environments. 
e.g. onion-architecture-container-apps-acr
The resource groups for the application environments will be created and destroyed programatically, the container registry should be kept separate.

When creating the container registry, a Basic SKU is sufficient. Name the container registry something identifiable. e.g. onion-architecture-container-apps

## Connect Azure to Octopus Deploy

### Create an Azure App Registration for Octopus Deploy

- In Azure AD select App Registrations

![Alt text](images/Create%20azure%20app%20registration%201.png)

- Select New Registration
    1. Name the Registration something identifiable. e.g. Onion-Containers-Octo
    2. A Redirect URI is not needed
    3. Select Register

![Alt text](images/Create%20azure%20app%20registration%202.png)

### Create a Client Secret

- Select Certificates and Secrets
- New Client Secret
- Provide a description and select add

![Alt text](images/Create%20azure%20app%20registration%203.png)

- Save the client secret Value. It will be used in Octopus.

### Set Octopus Deploy as an Azure Contributor

- In your Azure subscription, navigate to Access Control (IAM), and add a role assignment
- Select Privileged administrator roles, then Contributor

![Alt text](images/Create%20azure%20app%20registration%204.png)

- In the Members tab use the + Select Members page to select the App Registration that was created
- Press Review + assign

![Alt text](images/Create%20azure%20app%20registration%205.png)

- Press Review + assign again to save

### Create an Azure Account in Octopus Deploy

- In Octopus Deploy navigate to Infrastructure -\> Accounts
- Add an Azure Subscription Account
    1. Name the account Azure-Onion-Containers
    2. Fill in the Subscription ID
        - This can be found in the Subscription Overview page in the Azure web portal
    3. Leave the Authentication Method as 'Use a Service Principal'
    4. Fill in the Tenant ID
        - This can be found in the Overview page of the App Registration in the Azure web portal
    5. Fill in the Application ID
        - This is the Application (client) ID from the App registration that was just created. This can be found in the Overview page in the Azure web portal
    6. Fill in the Application Password / Key
        - This is the client secret value that was created previously

![Alt text](images/Create%20azure%20app%20registration%206.png)

# Octopus Deploy Environment Setup:

In Octopus Deploy create 3 environments.

- TDD
- UAT
- Prod

No Deployment targets need to be created.

Create a Lifecycle that uses those three environments promoting from TDD -\> UAT -\> Prod

# Octopus Deploy Project Setup:

## Connect Octopus to GitHub

### In GitHub

Create a Personal Access Token with repo access only.
Save the token for use in Octopus

### In Octopus

Create Git Credentials using the GitHub Personal Access Token

## Create a new Version Controlled Project:

1. Create a new project, ensure the "Use version control for this project" box is checked
2. Use the Lifecycle that was just created

![Alt text](images/new%20version%20controlled%20project%201.png)

3. Click Save AND CONFIGURE VCS

      1. Skip the "How do you intend to use this project" popup
4. Set the Git Repository URL to the URL of the forked repo
5. Use the Library Git Credentials that were created earlier

![Alt text](images/new%20version%20controlled%20project%202.png)

7. Click CONFIGURE and push the initial commit to convert the project

## Optional-Create a runbook for availability monitoring

In the deployment process Octopus will setup Azure App Insights to monitor the availability of the app. If the healthcheck endpoint returns unhealthy an alert will be created that triggers an Octopus Runbook. 
To configure the Runbook integration:
- There is a variable in the project named **OctoRunbookName** this is the name of the Runbook that Azure will run. Create a Runbook with the same name. e.g. Unhealthy app alert
- In the project create a variable named **azrunbookAPI** Set the value to Sensitive and provide an API key that has access to the project
- Update **OctoInstanceURL** with the URL of your Octopus instance



# Azure DevOps Setup:

Create a new project

Install the Octopus Deploy Integration ([https://marketplace.visualstudio.com/items?itemName=octopusdeploy.octopus-deploy-build-release-tasks](https://marketplace.visualstudio.com/items?itemName=octopusdeploy.octopus-deploy-build-release-tasks))

## Create Service Connections

To create a service connection
  - Go to Project Settings in the bottom left
  - Under the Pipelines heading, select Service Connections
  - Select Create Service Connection

### Create an Azure Resource Manager Service Connection
  - Select Azure Resource Manager as the new service connection type

![Alt text](images/service%20connections%201.png)

  - Use the Service Principal (automatic) authentication method
  - Select your Azure Subscriptoin
  - Leave the Resource Group section blank
  - Name the Service Connection: onion-architecture-maui-azure-dotnet-8
  - Check 'Grant access permission to all pipelines'

![Alt text](images/service%20connections%202.png)

  - Save the service connection

### Create an Octopus Deploy Service connection

  - In Octopus Deploy create an API key ([https://octopus.com/docs/octopus-rest-api/how-to-create-an-api-key](https://octopus.com/docs/octopus-rest-api/how-to-create-an-api-key))
  - In Azure DevOps select New Service Connection, choose Octopus Deploy as the type

![Alt text](images/service%20connections%203.png)

  - Fill in the URL of your Octopus instance
  - Fill in the API key that was created
  - Name the service connection: OctoServiceConnection
  - Check 'Grant access permission to all pipelines'

![Alt text](images/service%20connections%204.png)

  - Save the service connection

### Create an Azure Container Registry Service Connection

  - Select New Service Connection, choose Docker Registry as the type

![Alt text](images/service%20connections%205.png)

  - Configure the registry
    1. Choose Azure Container Registy as the type
    2. Choose Service Principal as the Authentication Type
    3. Select your Azure Subscription
    4. Select the container registry that was created
    5. Name the service connection: OnionArchACRServiceConnection
    6. Select 'Grant access permission to all pipelines'

![Alt text](images/service%20connections%206.png)

- Save the Service Connection

## Create an artifact feed

- In the Azure DevOps project: Go to Artifacts, then select **+ Create Feed**

![Alt text](images/feed%201.png)

- Name the feed something relevant, scope it to the current project, select create

![Alt text](images/feed%202.png)

## Authorize the Pipeline to push packages to the feed

- Set the Project Build Service identity to be a  **Contributor**  on your feed ([https://learn.microsoft.com/en-us/azure/devops/artifacts/feeds/feed-permissions?view=azure-devops#configure-feed-settings](https://learn.microsoft.com/en-us/azure/devops/artifacts/feeds/feed-permissions?view=azure-devops%23configure-feed-settings))

![Alt text](images/authorize%20feed%201.png)

## Add the Azure DevOps Feed to Octopus

### Create a Personal Access Token

1. Create an Azure DevOps Personal Access Token [https://learn.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops&tabs=Windows](https://learn.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops&tabs=Windows)
2. Give the token the Packaging Read scope
3. Save the token for use in Octopus

### In Azure DevOps

1. Navigate to the Artifacts page.
2. Select the 'Connect to Feed' button
3. Select Nuget.exe as the feed type
4. In the Project Setup section, copy the URL from the value field

![Alt text](images/add%20feed%20to%20octo%201.png)

### In Octopus Deploy

1. Navigate to Library -\> External Feeds and select ADD FEED
2. Set the Feed type to NuGet Feed
3. Name the feed Onion-Architecture-MAUI-Azure-dotnet-8
4. Paste in the URL that was copied from Azure DevOps
5. Provide something in the Feed username field. It can be anything other than an empty string. It's not actually used.
6. Provide the personal access token from Azure DevOps as the Feed Password

![Alt text](images/add%20feed%20to%20octo%202.png)

## Create and Update Project Variables

In the Octopus Project navigate to Variables -\> Project

- Create a variable named **DatabasePassword** Set the values to Sensitive and enter passwords for TDD, UAT, and Prod environments
- Update **registry\_login\_server** to the login server of the Azure Container Registry that was created
  - This login server can be found in the Overview page of the container registry in the Azure Web Portal
- Update **EnsureEnvironmentsExist** to True for Prod/UAT to ensure that all resources will be created the first time.

## Create the Library Variable Group

1. In the Azure DevOps project: Go to Pipelines -\> Library

![Alt text](images/variable%20group%201.png)

2. Create a variable group named **Integration-Build**

    1. Create a variable called **FeedName**. The value will be \<Azure DevOps project name\>/\<Azure DevOps feed name\>
    2. Create a variable called **OctoProjectGroup** with the value being the Project Group that houses your Octopus Project.
    3. Create a variable called **OctoProjectName** with the value being the name of your Octopus Project.
    4. Create a variable called **OctoSpace** with the value being the name of your Octopus Space.

![Alt text](images/variable%20group%202.png)

3. Save the variable group

## Grant the pipeline access to the variable group

1. From the variable group page select the Pipeline permissions tab at the top
2. Select the hamburger menu, and select Open Access

![Alt text](images/variable%20group%203.png)

3. Select Open access to allow all pipelines in the project to use the variable group

![Alt text](images/variable%20group%204.png)

## Create Environments

1. Go to Pipelines -\> Environments
2. Select New environment
3. Create three environments.
- TDD
- UAT
- Prod
4. In the UAT and Prod environments, add an approval check and select the users that need to approve the appropriate deploy stages.

## Create a Pipeline

1. Go to Pipelines -\> Pipelines

![Alt text](images/pipeline%201.png)

1. Select Create Pipeline
2. Select Github as the location for your code
3. Accept and allow Github and Azure DevOps to connect

![Alt text](images/pipeline%202.png)

4. Select the forked repo when asked to select a repository
5. Select Approve & Install to allow Azure Pipelines to connect to GitHub
6. When reviewing the pipeline YAML select **Run** to create and run the Pipeline for the first time

![Alt text](images/pipeline%203.png)

The pipeline will build the application, create all of the resources in the TDD environment, deploy the app to TDD, test the app, then destroy the TDD resources. Then the Azure resources in UAT will be created, and the app will be deployed to TDD. Ultimately Prod resources will be created, and the app will be deployed to Prod


# Octopus Deploy Runbook Setup:

In the ChurchBulletin.Scripts package that is created there is a script called ScaleInfrastructure.ps1. When provided with appReplicas and/or serviceObjective values the script will set the min and max number of replicas of the container app and the service objective of the database. This is used with Octopus Runbooks ([Runbooks Documentation](https://octopus.com/docs/runbooks#:~:text=To%20create%20or%20manage%20your,%E2%9E%9C%20Runbooks%20%E2%9E%9C%20Add%20Runbook.)) to scale up and down the infrastructure for day and nighttime loads.

## Create ContainerAppReplicas variable

In your Octopus Deploy project, create two new variables
- **ContainerAppScaledUpReplicas** and give it an integer value. e.g. 2
- **ContainerAppScaledUpCPU** and give it a float value. e.g. 0.5
- **ContainerAppScaledUpMem** and give it a float value. e.g. 1.0
- **DBScaledUpPerformanceLevel** and give it service objective value. e.g. S0

Commit these variables to main. **Variables not in the default branch will not be accessible to runbooks**

![Alt text](images/Replicas1.png)

## Create Scale Up Runbook

- In your Octopus Deploy project, navigate to Operations -> Runbooks and select ADD RUNBOOK
- Name the runbook Scale Up Infrastructure
- Select Save
![Alt text](images/runbook1.png)

- Select DEFINE YOUR RUNBOOK PROCESS near the upper right

![Alt text](images/runbook2.png)

- And then select ADD STEP

- Use the run az Azure Script step template

![Alt text](images/runbook3.png)

- Select ADD

- Name the step **Scale Up Infrastructure**
- Leave Execution Location, Worker Pool, Container Image, and Azure Tools as default

- Under Azure -> Account select the chain links icon to bind the account value to a variable. Then set the value to **#{AzureAccount}** 

![Alt text](images/runbook4.png)

- Under Script Source select **Script file inside a package** 
- Under Script File in Package set the Package feed to the feed that was created.
- Set the Package ID to **ChurchBulletin.Script**
- Set the Script file name to **ScaleInfrastructure.ps1**
- Set the Script parameters to **-appReplicas #{ContainerAppScaledUpReplicas} -cpu #{ContainerAppScaledUpCPU} -mem #{ContainerAppScaledUpMem} -serviceObjective #{DBScaledUpPerformanceLevel}**

![Alt text](images/runbook5.png)

Leave the rest of the settings at default, and select SAVE

## Create Scale Down Runbook

Create another runbook named Scale Down Container App using the same directions.

- Change the Step Name to **Scale Down Infrastructure**
- Leave the Script Parameters blank

## Publish the runbooks

Runbooks must be published before they can be consumed by triggers.

- Navigate to the Scale Up Infrastructure runbook. Select PUBLISH
![Alt text](images/runbook6.png)
- Leave the default settings, and select PUBLISH
![Alt text](images/runbook7.png)
- Do the same for Scale Down Infrastructure

## Create Scheduled Runbook Triggers
([Runbook Triggers Documentation](https://octopus.com/docs/runbooks/scheduled-runbook-trigger))

- Navigate to Operations -> Triggers
- Select ADD SCHEDULED TRIGGER
- Name the trigger Scale Up Morning
- Under Runbook select Scale Up Infrastructure
- Under Target Environments select Prod
- Leave the schedule at Daily
- Under Run Days uncheck Saturday and Sunday
- Set Schedule Timezone to your timezone
- Leave the Interval at once
- Set the Start Time to 8:00 AM
- Click Save

![Alt text](images/trigger1.png)

- Create another trigger named Scale Down Evening
- Use the Scale Down Container App runbook
- Under Target Environments select Prod
- Under Run Days uncheck Saturday and Sunday
- Set Schedule Timezone to your timezone
- Set the Start Time to 6:00 PM
- Click Save

Now the container app and database will automatically be scaled up every morning, and scaled down every evening

# Build and Test
TODO: Describe and show how to build your code and run the tests. 

# Contribute
TODO: Explain how other users and developers can contribute to make your code better. 

If you want to learn more about creating good readme files then refer the following [guidelines](https://docs.microsoft.com/en-us/azure/devops/repos/git/create-a-readme?view=azure-devops). You can also seek inspiration from the below readme files:
- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)
