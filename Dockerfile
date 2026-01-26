# 1. ใช้ SDK สำหรับการ Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy ไฟล์โปรเจกต์และ Restore package
COPY *.csproj ./
RUN dotnet restore

# Copy ไฟล์ทั้งหมดและ Build
COPY . ./
RUN dotnet publish -c Release -o out

# 2. ใช้ Runtime สำหรับการ Run (ทำให้ Image มีขนาดเล็ก)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# ตั้งค่า Port ตามที่คุณต้องการ (5000)
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "MyBackend.dll"]