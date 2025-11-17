# 1. Базовый рантайм для запуска
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

# 2. SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# 3. Готовый runtime
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Запуск твоего бота (замени на свою dll!)
CMD ["dotnet", "1WinTrafficBot.dll"]
