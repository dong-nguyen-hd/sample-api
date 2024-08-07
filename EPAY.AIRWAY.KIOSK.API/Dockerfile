FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EPAY.AIRWAY.KIOSK.API/EPAY.AIRWAY.KIOSK.API.csproj", "EPAY.AIRWAY.KIOSK.API/"]
RUN dotnet restore "EPAY.AIRWAY.KIOSK.API/EPAY.AIRWAY.KIOSK.API.csproj"
COPY . .
WORKDIR "/src/EPAY.AIRWAY.KIOSK.API"
RUN dotnet build "EPAY.AIRWAY.KIOSK.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EPAY.AIRWAY.KIOSK.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EPAY.AIRWAY.KIOSK.API.dll"]
