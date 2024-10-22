FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /build

COPY src/NuGet-CI.Config ./NuGet.Config
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p "${file%.*}/" && mv "$file" "${file%.*}/"; done

# This is only needed because swagger uses .NET 7
ENV DOTNET_ROLL_FORWARD=LatestMajor

ARG projectFile
RUN test -n "$projectFile"
RUN --mount=type=secret,id=GITHUB_TOKEN \
    GITHUB_TOKEN="$(cat /run/secrets/GITHUB_TOKEN)" \
    dotnet restore --packages "$HOME/app/.nuget" "$projectFile"

COPY ./src ./

RUN dotnet publish "$projectFile" -c Release -o /app/publish --no-restore --source "$HOME/app/.nuget"

FROM mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled as final
EXPOSE 8080

WORKDIR /app
COPY --from=build /app/publish .

# 64198
USER $APP_UID
ENTRYPOINT ["dotnet", "Cheetah.WebApi.dll"]
