FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ENV DOTNET_EnableWriteXorExecute=0
ARG BUILD_CONFIGURATION=Release
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
RUN dotnet build "Cervantes.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Cervantes.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cervantes.Web.dll"]

# update packages
RUN apt-get update -y && apt-get upgrade -y && apt-get autoremove -y
# create new user with lower privileges
RUN useradd -ms /bin/bash cervantes
# change permissions of particular directories
RUN chown -R cervantes /app/wwwroot

# switch to the newly created user
USER cervantes