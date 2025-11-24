# Makefile para tareas comunes de desarrollo
# - aliases para dotnet ef / watch dentro de `./bolsafeucn_back`
# hice esto con claude pq me da flojera apreneder makefile
DOTNET_DIR := ./bolsafeucn_back
CONTAINER_NAME := bolsafeucn-docker

# Extraer variables de la cadena de conexión en appsettings.json
# Valores esperados (deben coincidir con appsettings.json):
#   POSTGRES_USER := bolsafeucn-user
#   POSTGRES_PASSWORD := bolsafeucn-password
#   POSTGRES_DB := bolsafeucn-db
APPSETTINGS_FILE := $(DOTNET_DIR)/appsettings.json # Pueden cambiar el appsettings.json por sus variables.
CONNECTION_STRING := $(shell grep -o '"DefaultConnection": *"[^"]*"' $(APPSETTINGS_FILE) | sed 's/"DefaultConnection": *"\(.*\)"/\1/')
POSTGRES_USER := $(shell echo '$(CONNECTION_STRING)' | grep -o 'Username=[^;]*' | cut -d= -f2)
POSTGRES_PASSWORD := $(shell echo '$(CONNECTION_STRING)' | grep -o 'Password=[^;]*' | cut -d= -f2)
POSTGRES_DB := $(shell echo '$(CONNECTION_STRING)' | grep -o 'Database=[^;]*' | cut -d= -f2)

.PHONY: help db-restart run watch docker-create docker-rm docker-start docker-stop

help:
	@echo "Makefile - targets disponibles:"
	@echo "  make db-restart      -> drop, update DB y ejecutar dotnet watch (no-hot-reload)"
	@echo "  make run             -> ejecutar dotnet run"
	@echo "  make watch           -> ejecutar dotnet watch (no-hot-reload)"
	@echo "  make docker-create   -> crear y ejecutar contenedor PostgreSQL"
	@echo "  make docker-rm       -> detener y eliminar contenedor PostgreSQL"
	@echo "  make docker-start    -> iniciar contenedor PostgreSQL existente"
	@echo "  make docker-stop     -> detener contenedor PostgreSQL"

# 1) Drop DB, update y lanzar dotnet watch (sin hot reload)
db-restart:
	@echo "Entrando en $(DOTNET_DIR) y reiniciando la BD..."
	cd $(DOTNET_DIR) && \
		dotnet ef database drop --force && \
		dotnet ef database update && \
		dotnet watch --no-hot-reload

# 2) dotnet run simple
run:
	@echo "Ejecutando: cd $(DOTNET_DIR) && dotnet run"
	cd $(DOTNET_DIR) && dotnet run

# 3) dotnet watch simple
watch:
	@echo "Ejecutando: cd $(DOTNET_DIR) && dotnet watch --no-hot-reload"
	cd $(DOTNET_DIR) && dotnet watch --no-hot-reload

# 4) Crear y ejecutar contenedor PostgreSQL
docker-create:
	@echo "Creando contenedor PostgreSQL: $(CONTAINER_NAME)"
	docker run --name $(CONTAINER_NAME) \
		-e POSTGRES_USER=$(POSTGRES_USER) \
		-e POSTGRES_PASSWORD=$(POSTGRES_PASSWORD) \
		-e POSTGRES_DB=$(POSTGRES_DB) \
		-p 5432:5432 \
		-d postgres

# 5) Detener y eliminar contenedor PostgreSQL (si existe)
docker-rm:
	@echo "Deteniendo y eliminando contenedor PostgreSQL: $(CONTAINER_NAME)"
	-@docker stop $(CONTAINER_NAME) >/dev/null 2>&1 || true
	-@docker rm $(CONTAINER_NAME) >/dev/null 2>&1 || true
	@echo "Contenedor $(CONTAINER_NAME) eliminado (si existía)."

# 6) Iniciar contenedor PostgreSQL existente
docker-start:
	@echo "Iniciando contenedor PostgreSQL: $(CONTAINER_NAME)"
	docker start $(CONTAINER_NAME)

# 7) Detener contenedor PostgreSQL
docker-stop:
	@echo "Deteniendo contenedor PostgreSQL: $(CONTAINER_NAME)"
	docker stop $(CONTAINER_NAME)

