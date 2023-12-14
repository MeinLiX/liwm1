FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["webAPI/webAPI.csproj", "webAPI/"]
COPY ["Core/Application/Application.csproj", "Core/Application/"]
COPY ["Core/Domain/Domain.csproj", "Core/Domain/"]
COPY ["Infrastructure/Shared/Shared.csproj", "Infrastructure/Shared/"]
COPY ["Infrastructure/Persistence/PostgreSQL/PostgreSQL.csproj", "Infrastructure/Persistence/PostgreSQL/"]
RUN dotnet restore "webAPI/webAPI.csproj"
COPY . .
WORKDIR "/src/webAPI"
RUN dotnet build "webAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "webAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "webAPI.dll"]