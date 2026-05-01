FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["CarAuctionManagement.API/CarAuctionManagement.API.csproj", "CarAuctionManagement.API/"]
RUN dotnet restore "CarAuctionManagement.API/CarAuctionManagement.API.csproj"
COPY . .
WORKDIR "/src/CarAuctionManagement.API"
RUN dotnet build "CarAuctionManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CarAuctionManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "CarAuctionManagement.API.dll"]