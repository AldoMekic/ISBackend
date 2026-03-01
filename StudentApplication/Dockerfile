# ---------- Build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore (paths must match your repo)
COPY StudentApplication/StudentApplication.csproj StudentApplication/
COPY StudentApplication.Business/StudentApplication.Business.csproj StudentApplication.Business/
COPY StudentApplication.Contracts/StudentApplication.Contracts.csproj StudentApplication.Contracts/
COPY StudentApplication.Data/StudentApplication.Data.csproj StudentApplication.Data/

RUN dotnet restore StudentApplication/StudentApplication.csproj

# Copy everything else and publish
COPY . .
RUN dotnet publish StudentApplication/StudentApplication.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---------- Run ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Render sets PORT; bind Kestrel to it
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

COPY --from=build /app/publish .

EXPOSE 10000

ENTRYPOINT ["dotnet", "StudentApplication.dll"]