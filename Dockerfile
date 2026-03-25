# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the solution file and restore dependencies
COPY EtlSolution/EtlSolution.sln ./
COPY EtlSolution/EtlApp/EtlApp.fsproj EtlApp/
COPY EtlSolution/EtlCore/EtlCore.fsproj EtlCore/
COPY EtlSolution/EtlTests/EtlTests.fsproj EtlTests/
RUN dotnet restore

# Copy the rest of the source code
COPY EtlSolution/ ./

# Build the app
RUN dotnet publish EtlApp/EtlApp.fsproj -c Release -o /app/publish

# Use the runtime image for the final stage
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Copy data files
COPY EtlSolution/data/ ./data/

# Set the entry point
ENTRYPOINT ["dotnet", "EtlApp.dll"]