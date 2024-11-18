FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app


ENV ASPNETCORE_URLS=http://+:5252
ENV ASPNETCORE_ENVIRONMENT=Development

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Debug
WORKDIR /src
COPY ["src/UserProject.Api/UserProject.Api.csproj", "src/UserProject.Api/"]
RUN dotnet restore "src/UserProject.Api/UserProject.Api.csproj"
COPY . .
WORKDIR "/src/src/UserProject.Api"
RUN dotnet build "UserProject.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Debug
RUN dotnet publish "UserProject.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 5252
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserProject.Api.dll"]
