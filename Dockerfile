FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PetShop.Web/PetShop.Web.csproj", "PetShop.Web/"]
COPY ["PetShop.Data/PetShop.Data.csproj", "PetShop.Data/"]
COPY ["PetShop.Models/PetShop.Models.csproj", "PetShop.Models/"]
RUN dotnet restore "PetShop.Web/PetShop.Web.csproj"
COPY . .
WORKDIR "/src/PetShop.Web"
RUN dotnet build "PetShop.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PetShop.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Copy pre-existing images from source to container
COPY PetShop.Web/wwwroot/images ./wwwroot/images
ENTRYPOINT ["dotnet", "PetShop.Web.dll"]
