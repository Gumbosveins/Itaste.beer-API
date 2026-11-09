FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ItbApi.csproj .
RUN dotnet restore ItbApi.csproj
COPY . .
RUN dotnet publish ItbApi.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ItbApi.dll"]
