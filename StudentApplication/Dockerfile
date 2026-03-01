# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY StudentApplication.API/StudentApplication.API.csproj StudentApplication.API/
COPY StudentApplication.Business/StudentApplication.Business.csproj StudentApplication.Business/
COPY StudentApplication.Contracts/StudentApplication.Contracts.csproj StudentApplication.Contracts/
COPY StudentApplication.Data/StudentApplication.Data.csproj StudentApplication.Data/
RUN dotnet restore StudentApplication.API/StudentApplication.API.csproj

# Copy everything else and publish
COPY . .
RUN dotnet publish StudentApplication.API/StudentApplication.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---------- Run ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Render sets PORT; bind Kestrel to it
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

COPY --from=build /app/publish .

# Optional: document default port; Render will still inject PORT
EXPOSE 10000

ENTRYPOINT ["dotnet", "StudentApplication.API.dll"]