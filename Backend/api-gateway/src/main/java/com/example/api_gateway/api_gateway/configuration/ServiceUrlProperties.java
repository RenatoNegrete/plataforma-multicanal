package com.example.api_gateway.api_gateway.configuration;

import org.springframework.boot.context.properties.ConfigurationProperties;
import org.springframework.context.annotation.Configuration;

@Configuration
@ConfigurationProperties(prefix = "services")
public class ServiceUrlProperties {
    private String usuarios;
    private String catalogo;
    private String ordenes;
    private String pagos;
    private String proveedores;
    private String notificaciones;
    private String reportes;
    private String recomendaciones;
    private String logistica;

    public String getUsuarios() {
        return usuarios;
    }
    public void setUsuarios(String usuarios) {
        this.usuarios = usuarios;
    }
    public String getCatalogo() {
        return catalogo;
    }
    public void setCatalogo(String catalogo) {
        this.catalogo = catalogo;
    }
    public String getOrdenes() {
        return ordenes;
    }
    public void setOrdenes(String ordenes) {
        this.ordenes = ordenes;
    }
    public String getPagos() {
        return pagos;
    }
    public void setPagos(String pagos) {
        this.pagos = pagos;
    }
    public String getProveedores() {
        return proveedores;
    }
    public void setProveedores(String proveedores) {
        this.proveedores = proveedores;
    }
    public String getNotificaciones() {
        return notificaciones;
    }
    public void setNotificaciones(String notificaciones) {
        this.notificaciones = notificaciones;
    }
    public String getReportes() {
        return reportes;
    }
    public void setReportes(String reportes) {
        this.reportes = reportes;
    }
    public String getRecomendaciones() {
        return recomendaciones;
    }
    public void setRecomendaciones(String recomendaciones) {
        this.recomendaciones = recomendaciones;
    }
    public String getLogistica() {
        return logistica;
    }
    public void setLogistica(String logistica) {
        this.logistica = logistica;
    }
}
