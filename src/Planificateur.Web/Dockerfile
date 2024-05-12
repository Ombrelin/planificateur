FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM ombrelin/dotnet-node:8-18 AS build
WORKDIR /app
COPY . .
RUN dotnet build "src/Planificateur.Web/Planificateur.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Planificateur.Web/Planificateur.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Planificateur.Web.dll"]
