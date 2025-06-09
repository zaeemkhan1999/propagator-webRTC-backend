# Use official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory inside container
WORKDIR /app

# Copy the project files
COPY . ./

# Restore dependencies
RUN dotnet restore src/Apsy.App.Propagator.Api/Propagator.Api.csproj

# Publish the application
RUN dotnet publish src/Apsy.App.Propagator.Api/Propagator.Api.csproj -c Release -r linux-x64 --self-contained false -o ./app

# Use a lightweight runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set working directory inside container
WORKDIR /app

# Set ASP.NET Core to listen on port 5000
ENV ASPNETCORE_URLS=http://+:5000

# Copy the published application from the build stage
COPY --from=build /app/app .

# Expose the port your API runs on (adjust if needed)
EXPOSE 5000

# Start the application
ENTRYPOINT ["dotnet", "Propagator.Api.dll"]
