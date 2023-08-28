#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebhookRelayService/WebhookRelayService.csproj", "WebhookRelayService/"]
RUN dotnet restore "WebhookRelayService/WebhookRelayService.csproj"
COPY . .
WORKDIR "/src/WebhookRelayService"
RUN dotnet build "WebhookRelayService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebhookRelayService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebhookRelayService.dll"]