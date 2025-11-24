# ============================
#      STAGE 1: BUILDER
# ============================
FROM python:3.11-slim AS builder

WORKDIR /app

# Instalar dependencias del sistema necesarias
RUN apt-get update && apt-get install -y --no-install-recommends \
    gcc \
    && rm -rf /var/lib/apt/lists/*

# Copiar requirements
COPY requirements.txt .

# Crear entorno aislado
RUN python -m venv /venv && /venv/bin/pip install --upgrade pip && \
    /venv/bin/pip install -r requirements.txt


# ============================
#      STAGE 2: RUNNER
# ============================
FROM python:3.11-slim

WORKDIR /app

# Copiar entorno del builder
COPY --from=builder /venv /venv

# Copiar código del servicio
COPY . .

# Puerto interno del servicio
EXPOSE 8000

# Crear usuario no-root
RUN useradd -m myuser
USER myuser

ENTRYPOINT ["./wait-for-kafka.sh"]

# Comando para ejecutar FastAPI con Uvicorn
CMD ["/venv/bin/uvicorn", "main:api", "--host", "0.0.0.0", "--port", "8000"]
