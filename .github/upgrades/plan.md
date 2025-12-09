# .NET 10 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 10 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10 upgrade.
3. Upgrade Cheetah.WebApi/Cheetah.WebApi.csproj
4. Upgrade Cheetah.WebApi.Test/Cheetah.WebApi.Test.csproj
5. Run unit tests to validate upgrade in the project: Cheetah.WebApi.Test/Cheetah.WebApi.Test.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

No projects are excluded from this upgrade.

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                                           | Current Version | New Version | Description                                                    |
|:-------------------------------------------------------|:---------------:|:-----------:|:---------------------------------------------------------------|
| AspNetCore.HealthChecks.UI.Client                      | 8.0.1           | 10.0.0      | Recommended for .NET 10                                        |
| Cheetah.Kafka                                          | 2.3.0           | Latest      | Compatibility verification needed for .NET 10                  |
| Cheetah.OpenSearch                                     | 1.0.2           | Latest      | Compatibility verification needed for .NET 10                  |
| FluentValidation.AspNetCore                            | 11.3.1          | Latest      | Recommended for .NET 10                                        |
| Microsoft.AspNetCore.Authentication.JwtBearer          | 8.0.22          | 10.0.0      | Recommended for .NET 10                                        |
| Microsoft.AspNetCore.Mvc.Abstractions                  | 2.3.0           |             | Functionality included with framework reference                |
| Microsoft.AspNetCore.Mvc.Versioning                    | 5.1.0           |             | Deprecated - replace with Asp.Versioning.Mvc                   |
| Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer        | 5.1.0           |             | Deprecated - replace with Asp.Versioning.Mvc.ApiExplorer       |
| Microsoft.NET.Test.Sdk                                 | 17.14.1         | Latest      | Recommended for .NET 10                                        |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets  | 1.22.1          | Latest      | Recommended for .NET 10                                        |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### Cheetah.WebApi/Cheetah.WebApi.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - AspNetCore.HealthChecks.UI.Client should be updated from `8.0.1` to compatible .NET 10 version
  - Microsoft.AspNetCore.Authentication.JwtBearer should be updated from `8.0.22` to `10.0.0` (*recommended for .NET 10*)
  - Microsoft.AspNetCore.Mvc.Abstractions should be removed (*functionality included with framework reference*)
  - Microsoft.AspNetCore.Mvc.Versioning should be replaced with Asp.Versioning.Mvc (*deprecated*)
  - Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer should be replaced with Asp.Versioning.Mvc.ApiExplorer (*deprecated*)
  - FluentValidation.AspNetCore, Cheetah.Kafka, Cheetah.OpenSearch, and other packages should be verified for .NET 10 compatibility

API changes:
  - Review code for binary incompatible APIs
  - Review code for source incompatible APIs
  - Review code for behavioral changes in .NET 10

#### Cheetah.WebApi.Test/Cheetah.WebApi.Test.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.NET.Test.Sdk should be updated to latest version compatible with .NET 10
