FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app


ENV ASPNETCORE_URLS=http://+:5299
ENV ASPNETCORE_ENVIRONMENT=Development

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Debug
WORKDIR /src
COPY ["src/SchoolProject.Api/SchoolProject.Api.csproj", "src/SchoolProject.Api/"]
RUN dotnet restore "src/SchoolProject.Api/SchoolProject.Api.csproj"
COPY . .
WORKDIR "/src/src/SchoolProject.Api"
RUN dotnet build "SchoolProject.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Debug
RUN dotnet publish "SchoolProject.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
EXPOSE 5299
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SchoolProject.Api.dll"]
