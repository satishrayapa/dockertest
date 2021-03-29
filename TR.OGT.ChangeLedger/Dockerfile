# https://github.com/dotnet/dotnet-docker/tree/master/samples/complexapp
# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /source

# manually copy tr.ogt.library
COPY DockerNuGet/NuGet.Config .
COPY DockerNuGet ./Libraries

# copy csproj and restore as distinct layers
COPY TR.OGT.ChangeLedger.Console/*.csproj TR.OGT.ChangeLedger.Console/
COPY TR.OGT.ChangeLedger.Common/*.csproj TR.OGT.ChangeLedger.Common/
COPY TR.OGT.ChangeLedger.Domain/*.csproj TR.OGT.ChangeLedger.Domain/
COPY TR.OGT.ChangeLedger.Data/*.csproj TR.OGT.ChangeLedger.Data/
COPY TR.OGT.ChangeLedger.Infrastructure/*.csproj TR.OGT.ChangeLedger.Infrastructure/
RUN dotnet restore TR.OGT.ChangeLedger.Console/TR.OGT.ChangeLedger.Console.csproj

# copy and build app and libraries
COPY TR.OGT.ChangeLedger.Console/ TR.OGT.ChangeLedger.Console/
COPY TR.OGT.ChangeLedger.Common/ TR.OGT.ChangeLedger.Common/
COPY TR.OGT.ChangeLedger.Domain/ TR.OGT.ChangeLedger.Domain/
COPY TR.OGT.ChangeLedger.Data/ TR.OGT.ChangeLedger.Data/
COPY TR.OGT.ChangeLedger.Infrastructure/ TR.OGT.ChangeLedger.Infrastructure/
WORKDIR /source/TR.OGT.ChangeLedger.Console
RUN dotnet build -c release --no-restore

FROM build AS publish
RUN dotnet publish -c release --no-build -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:3.1
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TR.OGT.ChangeLedger.Console.dll"]