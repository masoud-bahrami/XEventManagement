#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Host/XEvent.Host/XEvent.Host.csproj", "src/Host/XEvent.Host/"]
RUN dotnet restore "src/Host/XEvent.Host/XEvent.Host.csproj"
COPY . .
WORKDIR "/src/src/Host/XEvent.Host"
RUN dotnet build "XEvent.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "XEvent.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "XEvent.Host.dll"]