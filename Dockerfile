FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY ZavodServer/*.csproj ./ZavodServer/
COPY Models/*.csproj ./Models/
RUN dotnet restore

# copy everything else and build app
COPY ZavodServer/. ./ZavodServer/
COPY Models/. ./Models/
WORKDIR /app/ZavodServer
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
WORKDIR /app
COPY --from=build /app/ZavodServer/out ./
ENTRYPOINT ["dotnet", "ZavodServer.dll"]