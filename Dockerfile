# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Src/Presentation/SlideCloud.Web/SlideCloud.Web.csproj", "Src/Presentation/SlideCloud.Web/"]
COPY ["Src/Core/SlideCloud.Application/SlideCloud.Application.csproj", "Src/Core/SlideCloud.Application/"]
COPY ["Src/Core/SlideCloud.Domain/SlideCloud.Domain.csproj", "Src/Core/SlideCloud.Domain/"]
COPY ["Src/Infrastructure/SlideCloud.Infrastructure/SlideCloud.Infrastructure.csproj", "Src/Infrastructure/SlideCloud.Infrastructure/"]
RUN dotnet restore "Src/Presentation/SlideCloud.Web/SlideCloud.Web.csproj"
COPY . .
WORKDIR "/src/Src/Presentation/SlideCloud.Web"
RUN dotnet build "SlideCloud.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SlideCloud.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SlideCloud.Web.dll"]
