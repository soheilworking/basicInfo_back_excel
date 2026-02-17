# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
# For more information, please see https://aka.ms/containercompat

# Use the official ASP.NET Core runtime as a parent image for Windows
FROM mcr.microsoft.com/dotnet/aspnet:9.0-nanoserver-ltsc2025 AS base
WORKDIR /app
EXPOSE 7113


# Use the official .NET SDK as a parent image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0-nanoserver-ltsc2025 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["Services/BasicInfo_Org/ORG.BasicInfo.API/Extensions/publicKeyServiceManagment.pem", "Services/BasicInfo_Org/ORG.BasicInfo.API/Extensions/"]
COPY ["Services/BasicInfo_Org/ORG.BasicInfo.API/ORG.BasicInfo.API.csproj", "Services/BasicInfo_Org/ORG.BasicInfo.API/"]
COPY ["Services/BasicInfo_Org/ORG.BasicInfo.Data/ORG.BasicInfo.Data.csproj", "Services/BasicInfo_Org/ORG.BasicInfo.Data/"]
COPY ["Services/BasicInfo_Org/Domain/ORG.BasicInfo.Domain.csproj", "Services/BasicInfo_Org/Domain/"]
RUN dotnet restore "Services/BasicInfo_Org/ORG.BasicInfo.API/ORG.BasicInfo.API.csproj"
COPY . .
WORKDIR "/src/Services/BasicInfo_Org/ORG.BasicInfo.API"
RUN dotnet build "ORG.BasicInfo.API.csproj" -c %BUILD_CONFIGURATION% -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "ORG.BasicInfo.API.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["Services/ORG.BasicInfo.API/certificate/", "./"]
# Create logs directory
RUN mkdir logs

# Health check
#HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
#  CMD powershell -Command "try { Invoke-WebRequest -Uri http://localhost/health -UseBasicParsing } catch { exit 1 }"

ENTRYPOINT ["dotnet", "ORG.BasicInfo.API.dll"]