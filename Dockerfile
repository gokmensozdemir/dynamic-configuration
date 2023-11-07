#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DynamicConfiguration.Demo/DynamicConfiguration.Demo.csproj", "DynamicConfiguration.Demo/"]
COPY ["DynamicConfiguration/DynamicConfiguration.csproj", "DynamicConfiguration/"]
RUN dotnet restore "DynamicConfiguration.Demo/DynamicConfiguration.Demo.csproj"
COPY . .
WORKDIR "/src/DynamicConfiguration.Demo"
RUN dotnet build "DynamicConfiguration.Demo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DynamicConfiguration.Demo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DynamicConfiguration.Demo.dll"]