# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# Establece el directorio de trabajo base para toda la solución
WORKDIR /src

# 1. Copia el archivo de solución (.sln) a /src
# El contexto de build está en backend-PIS, por eso la ruta es bolsafeucn_back/
COPY bolsafeucn_back/bolsafeucn_back.sln .

# 2. Crea la subcarpeta del proyecto y copia el .csproj dentro
RUN mkdir -p bolsafeucn_back/
COPY bolsafeucn_back/bolsafeucn_back.csproj bolsafeucn_back/

# 3. Restaura las dependencias (busca el .csproj en la subcarpeta que acabamos de crear)
RUN dotnet restore bolsafeucn_back/bolsafeucn_back.csproj

# 4. Copia el resto del código fuente a la subcarpeta del proyecto
COPY bolsafeucn_back/. bolsafeucn_back/

# 5. Publica la aplicación, usando la ruta del proyecto
WORKDIR /src/bolsafeucn_back
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Create the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "bolsafeucn_back.dll"]

