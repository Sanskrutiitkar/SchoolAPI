FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app


ENV ASPNETCORE_URLS=http://+:5051
ENV ASPNETCORE_ENVIRONMENT=Development

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Debug
WORKDIR /src
COPY ["src/ProjectGateway.Api/ProjectGateway.Api.csproj", "src/ProjectGateway.Api/"]
RUN dotnet restore "src/ProjectGateway.Api/ProjectGateway.Api.csproj"
COPY . .
WORKDIR "/src/src/ProjectGateway.Api"
RUN dotnet build "ProjectGateway.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Debug
RUN dotnet publish "ProjectGateway.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 5051
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectGateway.Api.dll"]
