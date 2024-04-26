# Use the official .NET SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory in the container
WORKDIR /app

# Copy the project files to the container
COPY . .

# Restore dependencies
RUN dotnet restore

# Build the project
RUN dotnet build -c Release -o /app/build

# Publish the project
RUN dotnet publish -c Release -o /app/publish

# Use the official .NET runtime image as the base image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory in the container
WORKDIR /app

# Copy the published output from the build stage to the runtime image
COPY --from=build /app/publish .

# Expose port 80 for the application 
EXPOSE 80

# Define the command to run the application when the container starts
ENTRYPOINT ["dotnet", "api.dll"]
