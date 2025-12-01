# 1. Aşama: Build (Derleme)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Proje dosyalarını kopyala
COPY . ./

# Bağımlılıkları yükle
RUN dotnet restore

# Yayınlanacak (Release) sürümü oluştur
RUN dotnet publish WebAPI/WebAPI.csproj -c Release -o out

# 2. Aşama: Runtime (Çalıştırma)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Render'ın verdiği portu dinlemesi için ayar
ENV ASPNETCORE_URLS=http://+:8080

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "WebAPI.dll"]
