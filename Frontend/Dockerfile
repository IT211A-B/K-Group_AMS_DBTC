# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the .csproj file and restore dependencies
COPY Frontend/Frontend.csproj .
RUN dotnet restore Frontend.csproj

# Copy all source code
COPY Frontend/. .

# Publish (explicitly specify the project file)
RUN dotnet publish Frontend.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Frontend.dll"]
