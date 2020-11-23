FROM microsoft/dotnet:3.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:3.1-sdk AS build
WORKDIR /src
COPY FilesWebDepot/FilesWebDepot.csproj FilesWebDepot/
RUN dotnet restore FilesWebDepot/FilesWebDepot.csproj
COPY . .
WORKDIR /src/FilesWebDepot
RUN dotnet build FilesWebDepot.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish FilesWebDepot.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FilesWebDepot.dll"]
