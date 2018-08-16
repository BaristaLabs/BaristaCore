FROM microsoft/dotnet:2.1-runtime-alpine AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk-alpine AS build
WORKDIR /app
COPY ./BaristaServer.csproj src/BaristaServer/
RUN dotnet restore src/BaristaServer/BaristaServer.csproj
COPY . .
WORKDIR /app/src/BaristaServer
RUN dotnet build BaristaServer.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish BaristaServer.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BaristaServer.dll"]