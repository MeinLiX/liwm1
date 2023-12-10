FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Core/Application/Application.csproj", "api/Core/Application/"]
COPY ["Core/Domain/Domain.csproj", "api/Core/Domain/"]
COPY ["Infrastructure/Shared/Shared.csproj", "api/Infrastructure/Shared/"]
COPY ["Infrastructure/Persistence/PostgreSQL/PostgreSQL.csproj", "api/Infrastructure/Persistence/PostgreSQL/"]
COPY ["webAPI/webAPI.csproj", "api/webAPI/"]
RUN dotnet restore "api\webAPI\webAPI.csproj"
COPY . .
WORKDIR "/src/api/webAPI"
RUN dotnet build "webAPI.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "webAPI.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "webAPI.dll"]
