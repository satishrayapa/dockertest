# https://github.com/dotnet/dotnet-docker/tree/master/samples/complexapp
# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /source

# manually copy tr.ogt.library
COPY DockerNuGet/NuGet.Config .
COPY DockerNuGet ./Libraries

# copy csproj and restore as distinct layers
COPY TR.OGT.QueryExtract.Console/*.csproj TR.OGT.QueryExtract.Console/
COPY TR.OGT.QueryExtract.Common/*.csproj TR.OGT.QueryExtract.Common/
COPY TR.OGT.QueryExtract.Domain/*.csproj TR.OGT.QueryExtract.Domain/
COPY TR.OGT.QueryExtract.Data/*.csproj TR.OGT.QueryExtract.Data/
COPY TR.OGT.QueryExtract.Queue/*.csproj TR.OGT.QueryExtract.Queue/
COPY TR.OGT.QueryExtract.Infrastructure/*.csproj TR.OGT.QueryExtract.Infrastructure/
RUN dotnet restore TR.OGT.QueryExtract.Console/TR.OGT.QueryExtract.Console.csproj

# copy and build app and libraries
COPY TR.OGT.QueryExtract.Console/ TR.OGT.QueryExtract.Console/
COPY TR.OGT.QueryExtract.Common/ TR.OGT.QueryExtract.Common/
COPY TR.OGT.QueryExtract.Domain/ TR.OGT.QueryExtract.Domain/
COPY TR.OGT.QueryExtract.Data/ TR.OGT.QueryExtract.Data/
COPY TR.OGT.QueryExtract.Queue/ TR.OGT.QueryExtract.Queue/
COPY TR.OGT.QueryExtract.Infrastructure/ TR.OGT.QueryExtract.Infrastructure/
WORKDIR /source/TR.OGT.QueryExtract.Console
RUN dotnet build -c release --no-restore

FROM build AS publish
RUN dotnet publish -c release --no-build -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:3.1
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TR.OGT.QueryExtract.Console.dll"]