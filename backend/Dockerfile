# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution, central package management, and project files first for better layer caching
COPY DhammaSession.sln ./
COPY Directory.Packages.props ./
COPY src/Core/Core.csproj src/Core/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/API/API.csproj src/API/

# Restore dependencies
RUN dotnet restore

# Copy source code and build
COPY src/ ./src/
RUN dotnet publish src/API/API.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "API.dll"]
