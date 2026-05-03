# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy the project file
COPY ["BiblioApp/BiblioApp.csproj", "BiblioApp/"]

# Restore dependencies
RUN dotnet restore "BiblioApp/BiblioApp.csproj"

# Copy the entire project
COPY BiblioApp BiblioApp/

# Build the application
RUN dotnet build "BiblioApp/BiblioApp.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "BiblioApp/BiblioApp.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/publish .

# Expose the default ASP.NET Core port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "BiblioApp.dll"]
