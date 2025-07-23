FROM mcr.microsoft.com/dotnet/sdk:10.0-preview-alpine AS build
WORKDIR /src
COPY MySvelteApp.Server/*.csproj .
RUN dotnet restore
COPY MySvelteApp.Server/. .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview-alpine
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet","MySvelteApp.Server.dll"]
