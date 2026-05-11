# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the .csproj file from the subdirectory
COPY . ./
RUN dotnet restore ./Frontend/Frontend/Frontend.csproj

# Copy all source code from the subdirectory
COPY . ./

# Publish
RUN dotnet publish ./Frontend/Frontend/Frontend.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Frontend.dll"]