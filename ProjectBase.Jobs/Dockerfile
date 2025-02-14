#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8000
EXPOSE 8001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ProjectBase.Jobs/ProjectBase.Jobs.csproj", "ProjectBase.Jobs/"]
RUN dotnet restore "./ProjectBase.Jobs/ProjectBase.Jobs.csproj"
COPY . .
WORKDIR "/src/ProjectBase.Jobs"
RUN dotnet build "./ProjectBase.Jobs.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProjectBase.Jobs.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ProjectBase.Jobs/appsettings.json ./ 
ENTRYPOINT ["dotnet", "ProjectBase.Jobs.dll"]