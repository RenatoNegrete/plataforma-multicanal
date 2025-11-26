# ============================
# 1. Imagen base ligera
# ============================
FROM python:3.11-slim

# Evitar buffering
ENV PYTHONUNBUFFERED=1

# ============================
# 2. Instalar dependencias SO
# ============================
RUN apt-get update && apt-get install -y --no-install-recommends \
    curl \
    build-essential \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# ============================
# 3. Crear directorio app
# ============================
WORKDIR /app

# ============================
# 4. Copiar requirements
# ============================
COPY requirements.txt .

# ============================
# 5. Instalar dependencias Python
# ============================
RUN pip install --no-cache-dir -r requirements.txt

# ============================
# 6. Copiar el código fuente
# ============================
COPY . .

# ============================
# 7. Exponer puerto FastAPI
# ============================
EXPOSE 8000


# ============================
# 8. Ejecutar FastAPI
# ============================
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]
