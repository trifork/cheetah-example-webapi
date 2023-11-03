#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

ARG GITHUB_ACTOR
ARG GITHUB_TOKEN

WORKDIR /src
COPY "src/NuGet-CI.Config" "NuGet.config"
COPY "src/.config/" ".config/"
COPY ["src/Cheetah.WebApi/Cheetah.WebApi.csproj", "Cheetah.WebApi/"]
RUN --mount=type=secret,id=GITHUB_TOKEN \
    GITHUB_TOKEN="$(cat /run/secrets/GITHUB_TOKEN)" \
    dotnet restore "Cheetah.WebApi/Cheetah.WebApi.csproj"
COPY src .
WORKDIR "/src/Cheetah.WebApi"
RUN dotnet build "Cheetah.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cheetah.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
# Create a new user with bash and a homedir
RUN useradd -r -u 1001 -m -s /bin/bash dotnetuser

# Set the user for subsequent commands
USER 1001

ENV COMPlus_EnableDiagnostics=0

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT dotnet "Cheetah.WebApi.dll"