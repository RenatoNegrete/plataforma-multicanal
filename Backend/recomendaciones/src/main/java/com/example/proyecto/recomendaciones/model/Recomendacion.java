package com.example.proyecto.recomendaciones.model;

import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.mapping.Document;

@Document(collection = "recomendaciones")
public class Recomendacion {

    @Id
    private String id;

    private String productoId;
    private String mensaje;
    private int prioridad;

    public Recomendacion() {}

    public Recomendacion(String id, String productoId, String mensaje, int prioridad) {
        this.id = id;
        this.productoId = productoId;
        this.mensaje = mensaje;
        this.prioridad = prioridad;
    }

    public String getId() { return id; }
    public void setId(String id) { this.id = id; }

    public String getProductoId() { return productoId; }
    public void setProductoId(String productoId) { this.productoId = productoId; }

    public String getMensaje() { return mensaje; }
    public void setMensaje(String mensaje) { this.mensaje = mensaje; }

    public int getPrioridad() { return prioridad; }
    public void setPrioridad(int prioridad) { this.prioridad = prioridad; }
}
