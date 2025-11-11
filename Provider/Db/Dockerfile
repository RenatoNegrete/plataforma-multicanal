# Imagen base de MySQL
FROM mysql:8.0

# Variables de entorno
ENV MYSQL_ROOT_PASSWORD=root
ENV MYSQL_DATABASE=providerdb
ENV MYSQL_USER=provider_user
ENV MYSQL_PASSWORD=provider_pass

# Copiar el archivo SQL de inicialización
COPY init.sql /docker-entrypoint-initdb.d/

EXPOSE 3306
