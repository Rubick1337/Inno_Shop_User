FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Inno_Shop_Product/Inno_Shop_Product.csproj", "Inno_Shop_Product/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Data/Data.csproj", "Data/"]
COPY ["Domain/Domain.csproj", "Domain/"]

RUN dotnet restore "Inno_Shop_Product/Inno_Shop_Product.csproj"

COPY . .
WORKDIR "/src/Inno_Shop_Product"
RUN dotnet build "Inno_Shop_Product.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Inno_Shop_Product.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Inno_Shop_Product.dll"]
