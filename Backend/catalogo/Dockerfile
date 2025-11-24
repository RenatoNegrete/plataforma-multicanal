# Etapa 1: Construcción del JAR con Maven
FROM maven:3.9.6-eclipse-temurin-17 AS build

WORKDIR /app

# Copiar archivos de Maven
COPY pom.xml .
COPY src ./src

# Compilar proyecto
RUN mvn -B -DskipTests clean package

# Etapa 2: Imagen final ligera con JDK
FROM eclipse-temurin:17-jdk-alpine

WORKDIR /app

# Copiar el JAR generado en la etapa anterior
COPY --from=build /app/target/catalogo-0.0.1-SNAPSHOT.jar app.jar

# Expone el puerto (cámbialo si tu microservicio usa otro)
EXPOSE 8081

# Comando para ejecutar Spring Boot
ENTRYPOINT ["java", "-jar", "/app/app.jar"]
