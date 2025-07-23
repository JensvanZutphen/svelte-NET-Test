# 1) Client build
FROM node:20-alpine AS client-build
WORKDIR /src/MySvelteApp.Client

# copy package files and install deps
COPY ../MySvelteApp.Client/package*.json ./
RUN npm ci

# copy client source and build
COPY ../MySvelteApp.Client/ .
RUN npm run build

# 2) Server build
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0-preview-alpine AS server-build
ARG TARGETARCH
WORKDIR /src/MySvelteApp.Server

# copy csproj and restore
COPY *.csproj ./
RUN dotnet restore -a $TARGETARCH

# copy server code
COPY . .

# copy built client into wwwroot
COPY --from=client-build /src/MySvelteApp.Client/build ./wwwroot

# publish server
RUN dotnet publish -c Release -a $TARGETARCH --no-restore -o /app/publish

# 3) Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview-alpine AS runtime
WORKDIR /app

# copy published server (which now includes wwwroot)
COPY --from=server-build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "MySvelteApp.Server.dll"]
