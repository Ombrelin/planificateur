FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Planificateur.Web/Planificateur.Web.csproj", "Planificateur.Web/"]
RUN dotnet restore "src/Planificateur.Web/Planificateur.Web.csproj"
COPY . .
WORKDIR "/src/Planificateur.Web"
RUN dotnet build "Planificateur.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Planificateur.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Planificateur.Web.dll"]
