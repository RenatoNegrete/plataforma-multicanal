# ============================================
# Etapa 1: Build de la aplicación
# ============================================
FROM maven:3.9-eclipse-temurin-17 AS builder

WORKDIR /app

# Copiar archivos de Maven
COPY pom.xml .
COPY src ./src

# Compilar el proyecto
RUN mvn clean package -DskipTests

# ============================================
# Etapa 2: Imagen final con WildFly
# ============================================
FROM quay.io/wildfly/wildfly:31.0.0.Final-jdk17

# Cambiar a usuario root temporalmente para configurar
USER root

# Descargar driver MySQL a una ubicación temporal
ADD --chown=jboss:jboss https://repo1.maven.org/maven2/com/mysql/mysql-connector-j/8.2.0/mysql-connector-j-8.2.0.jar \
    /tmp/mysql-connector-java.jar

# Crear directorio para el módulo MySQL
RUN mkdir -p /opt/jboss/wildfly/modules/com/mysql/main && \
    chown -R jboss:jboss /opt/jboss/wildfly/modules/com/mysql

# Mover el driver al módulo
RUN mv /tmp/mysql-connector-java.jar /opt/jboss/wildfly/modules/com/mysql/main/mysql-connector-java.jar && \
    chown jboss:jboss /opt/jboss/wildfly/modules/com/mysql/main/mysql-connector-java.jar

# Crear module.xml para el driver MySQL
RUN echo '<?xml version="1.0" encoding="UTF-8"?>' > /opt/jboss/wildfly/modules/com/mysql/main/module.xml && \
    echo '<module xmlns="urn:jboss:module:1.9" name="com.mysql">' >> /opt/jboss/wildfly/modules/com/mysql/main/module.xml && \
    echo '  <resources>' >> /opt/jboss/wildfly/modules/com/mysql/main/module.xml && \
    echo '    <resource-root path="mysql-connector-java.jar"/>' >> /opt/jboss/wildfly/modules/com/mysql/main/module.xml && \
    echo '  </resources>' >> /opt/jboss/wildfly/modules/com/mysql/main/module.xml && \
    echo '  <dependencies>' >> /opt/jboss/wildfly/modules/com/mysql/main/module.xml && \
    echo '    <module name="javax.api"/>' >> /opt/jboss/wildfly/modules/com/mysql/main/module.xml && \
    echo '    <module name="javax.transaction.api"/>' >> /opt/jboss/wildfly/modules/com/mysql/main/module.xml && \
    echo '  </dependencies>' >> /opt/jboss/wildfly/modules/com/mysql/main/module.xml && \
    echo '</module>' >> /opt/jboss/wildfly/modules/com/mysql/main/module.xml && \
    chown jboss:jboss /opt/jboss/wildfly/modules/com/mysql/main/module.xml

# Copiar script de configuración del datasource (SIMPLIFICADO)
COPY --chown=jboss:jboss configure-datasource.cli /opt/jboss/wildfly/

# Copiar WAR
COPY --chown=jboss:jboss --from=builder /app/target/usuarios-service.war \
    /opt/jboss/wildfly/standalone/deployments/

# Volver al usuario jboss
USER jboss

# Ejecutar configuración del datasource
RUN /opt/jboss/wildfly/bin/jboss-cli.sh --file=/opt/jboss/wildfly/configure-datasource.cli

# Exponer puertos
EXPOSE 8080 9990

# Comando de inicio
CMD ["/opt/jboss/wildfly/bin/standalone.sh", "-b", "0.0.0.0"]