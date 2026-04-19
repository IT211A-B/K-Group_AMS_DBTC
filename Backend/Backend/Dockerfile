# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS base
USER $APP_UID
WORKDIR /ams
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
#FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["Backend.csproj", "Backend/"]
#RUN dotnet restore "./Backend.csproj"
#COPY . .
#WORKDIR "/src/Backend"
#RUN dotnet build "./Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /ams

# Restore in buld stage is the first one
COPY . ./
RUN dotnet restore ./Backend.csproj

# Publish in build stage which is  the second one
RUN dotnet publish ./Backend.csproj -c Release -o out

# Runtime stage -- aspnet
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /ams
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "Backend.dll"]

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /ams
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backend.dll"]