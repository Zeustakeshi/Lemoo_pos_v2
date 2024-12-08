# Use the official .NET Core SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything and restore dependencies
COPY . .
RUN dotnet restore

# Build the project
RUN dotnet publish -c Release -o /out

# Use a smaller runtime image for deployment
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Expose the port and start the application
EXPOSE 8080
ENTRYPOINT ["dotnet", "Lemoo_pos.dll"]