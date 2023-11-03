# Cheetah.WebApi

This project contains the demo version of a Cheetah.WebApi.  
It showcases:

* Produce a message to kafka
* Consume a message from kafka
* Query the opensearch database

With the OAuth2 security model.

## Running the application

Please setup a local certificate:

```shell
dotnet dev-certs https -ep "$env:APPDATA/ASP.NET/https/aspnetapp.pfx" -p "password"
dotnet dev-certs https -t # trust
```

### Authentication

For PoC purpose, we are using a `OAuthSimulator`.  
It can be accessed at https://localhost:1852/swagger/index.html


## Docker prerequisites

Ensure that you have the latest version of Visual Studio and [Docker Desktop](https://www.docker.com/products/docker-desktop) installed.  
You may need to enable virtualization in your BIOS.  
_NB:_ We are using Linux containers.

## Dependencies

Currently the project uses a nuget package from Trifork's repository Called Cheetah.Shared-{versionNumber}

To source this dependency you will first need to create a Personal Access Token to to a Github-account with access to this repository.

<https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry#authenticating-with-a-personal-access-token>

### Option A) Configure nuget source inside Visual studio

1. Navigate to **Tools**->**Options**
2. Find the **NuGet Package Manager** -> **Package Sources**
3. Add following source: "https://nuget.pkg.github.com/trifork/index.json"
4. click OK
5. Restart Visual Studio, if it's open.
6. Open NuGet Pack Manager and select the new source. You will now be prompted for a username and password. Enter your account-name into username. And use your PAT as your password

### Option B) Configure NuGet config directly

Add the source in your private `$HOME/.nuget/NuGet/NuGet.Config` file, e.g.

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <config>
    <add key="globalPackagesFolder" value="/Users/hhravn/.nuget/cache" />
  </config>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="github" value="https://nuget.pkg.github.com/trifork/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <github>
        <add key="Username" value="myuser" />
        <add key="ClearTextPassword" value="github-personal-access-token" />
    </github>
  </packageSourceCredentials>
</configuration>
```

### Running Docker outside Visual Studio

The project can be run with `docker-compose` outside Visual Studio with a few prerequisites.  
This can be useful when you are not interested in debugging the service and want to use less computer resources.

Supply NuGet credentials through environment variables:

```powershell
[System.Environment]::SetEnvironmentVariable('GITHUB_ACTOR', '<github_username>', [System.EnvironmentVariableTarget]::User)
[System.Environment]::SetEnvironmentVariable('GITHUB_TOKEN', '<personal_access_token>', [System.EnvironmentVariableTarget]::User)
```

Commands:

```shell
# Start container using docker-compose.yml
docker-compose up
# OR start container using docker-compose.yml as detached (will run in background)
docker-compose up -d
```

#### Sources

[Docker basics](https://docs.docker.com/get-started/)
[Docker-compose](https://docs.docker.com/compose/)
