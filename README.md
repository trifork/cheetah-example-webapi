# Cheetah.WebApi

This project contains the demo version of a Cheetah.WebApi built with .NET 10.  
It showcases:

* Produce a message to Kafka
* Consume a message from Kafka
* Query the OpenSearch database
* OAuth2 security model
* Prometheus metrics
* Health checks

## Prerequisites

Before running this application, ensure the following requirements are met:

### 1. Infrastructure Services

Local infrastructure (Kafka and OpenSearch) must be running. These can be started using Docker Compose from the [cheetah-development-infrastructure](https://github.com/trifork/cheetah-development-infrastructure) repository.

### 2. Kafka Topic

A topic named `cheetahwebapi` must be created in Kafka. Create this through Redpanda Console at http://localhost:9898

### 3. GitHub Personal Access Token

The project uses NuGet packages from Trifork's GitHub Packages repository. You need a Personal Access Token (PAT) for a GitHub account with access to this repository.

See: https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry#authenticating-with-a-personal-access-token

### 4. Development Certificate

A local HTTPS development certificate is required. Run:

```bash
# For Linux/macOS
dotnet dev-certs https -ep ~/.aspnet/https/aspnetapp.pfx -p "password"
dotnet dev-certs https -t

# For Windows (PowerShell)
dotnet dev-certs https -ep "$env:APPDATA/ASP.NET/https/aspnetapp.pfx" -p "password"
dotnet dev-certs https -t
```

### 5. Docker Desktop

Ensure you have [Docker Desktop](https://www.docker.com/products/docker-desktop) installed and running.  
You may need to enable virtualization in your BIOS.  
**Note:** This project uses Linux containers.

## Running Inside Visual Studio

### 1. Configure NuGet Package Source

#### Option A: Visual Studio UI

1. Navigate to **Tools** → **Options**
2. Find **NuGet Package Manager** → **Package Sources**
3. Add the following source: `https://nuget.pkg.github.com/trifork/index.json`
4. Click **OK**
5. Restart Visual Studio
6. Open NuGet Package Manager and select the new source
7. When prompted, enter your GitHub username and use your PAT as the password

#### Option B: Manual Configuration

Edit your `$HOME/.nuget/NuGet/NuGet.Config` file (recommended approach using environment variables):

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="github" value="https://nuget.pkg.github.com/trifork/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <github>
      <add key="Username" value="%GITHUB_ACTOR%" />
      <add key="ClearTextPassword" value="%GITHUB_TOKEN%" />
    </github>
  </packageSourceCredentials>
</configuration>
```

Set the environment variables:

```bash
# Linux/macOS
export GITHUB_ACTOR='<your_github_username>'
export GITHUB_TOKEN='<your_personal_access_token>'

# Windows (PowerShell)
[System.Environment]::SetEnvironmentVariable('GITHUB_ACTOR', '<your_github_username>', [System.EnvironmentVariableTarget]::User)
[System.Environment]::SetEnvironmentVariable('GITHUB_TOKEN', '<your_personal_access_token>', [System.EnvironmentVariableTarget]::User)
```

### 2. Build and Run

1. Open `src/Cheetah.WebApi.sln` in Visual Studio
2. Restore NuGet packages
3. Set `Cheetah.WebApi` as the startup project
4. Press F5 to run with debugging or Ctrl+F5 to run without debugging

### 3. Access the Application

- **Swagger UI (HTTPS)**: https://localhost:1851/swagger
- **Swagger UI (HTTP)**: http://localhost:1751/swagger
- **Health Check**: https://localhost:1851/health
- **Prometheus Metrics**: http://localhost:1861/metrics
- **OAuth Simulator**: https://localhost:1852/swagger/index.html

## Running Outside Visual Studio

This approach is useful when you don't need debugging and want to use fewer resources.

### 1. Configure NuGet Credentials

Set environment variables for NuGet authentication:

```bash
# Linux/macOS
export GITHUB_ACTOR='<your_github_username>'
export GITHUB_TOKEN='<your_personal_access_token>'

# Windows (PowerShell)
[System.Environment]::SetEnvironmentVariable('GITHUB_ACTOR', '<your_github_username>', [System.EnvironmentVariableTarget]::User)
[System.Environment]::SetEnvironmentVariable('GITHUB_TOKEN', '<your_personal_access_token>', [System.EnvironmentVariableTarget]::User)
```

### 2. Run with Docker Compose

```bash
# Build and start the container
cd src
docker compose up --build

# OR run in detached mode (background)
docker compose up -d --build

# View logs
docker compose logs -f

# Stop the container
docker compose down
```

### 3. Access the Application

- **Swagger UI (HTTPS)**: https://localhost:1851/swagger
- **Swagger UI (HTTP)**: http://localhost:1751/swagger
- **Health Check**: https://localhost:1851/health
- **Prometheus Metrics**: http://localhost:1861/metrics

## Authentication

For PoC purposes, this application uses an OAuth2 simulator provided by the local infrastructure.

## Additional Resources

- [Docker Basics](https://docs.docker.com/get-started/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [.NET 10 Documentation](https://docs.microsoft.com/en-us/dotnet/)
