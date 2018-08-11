FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/sample.api/sample.api.csproj src/sample.api/
RUN dotnet restore src/sample.api/sample.api.csproj
COPY . .
WORKDIR /src/src/sample.api
RUN dotnet build sample.api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish sample.api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "sample.api.dll"]
