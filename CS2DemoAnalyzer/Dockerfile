FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DemoAnalyzer/CS2DemoAnalyzer/CS2DemoAnalyzer.csproj", "DemoAnalyzer/CS2DemoAnalyzer/"]
RUN dotnet restore "DemoAnalyzer/CS2DemoAnalyzer/CS2DemoAnalyzer.csproj"
COPY . .
WORKDIR "/src/DemoAnalyzer/CS2DemoAnalyzer"
RUN dotnet build "CS2DemoAnalyzer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CS2DemoAnalyzer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CS2DemoAnalyzer.dll"]
