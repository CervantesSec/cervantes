FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Cervantes.Web/Cervantes.Web.csproj", "Cervantes.Web/"]
COPY ["Cervantes.Application/Cervantes.Application.csproj", "Cervantes.Application/"]
COPY ["Cervantes.Contracts/Cervantes.Contracts.csproj", "Cervantes.Contracts/"]
COPY ["Cervantes.CORE/Cervantes.CORE.csproj", "Cervantes.CORE/"]
COPY ["Cervantes.DAL/Cervantes.DAL.csproj", "Cervantes.DAL/"]
COPY ["Cervantes.IFR/Cervantes.IFR.csproj", "Cervantes.IFR/"]
RUN dotnet restore "Cervantes.Web/Cervantes.Web.csproj"
COPY . .
WORKDIR "/src/Cervantes.Web"
RUN dotnet build "Cervantes.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cervantes.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cervantes.Web.dll"]
