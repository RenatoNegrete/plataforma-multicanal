package com.example.proyecto.ordenes.dto;

public class ItemOrden {
    private String productoId;
    private Integer cantidad;

    public ItemOrden() {}

    public ItemOrden(String productoId, Integer cantidad) {
        this.productoId = productoId;
        this.cantidad = cantidad;
    }

    public String getProductoId() {
        return productoId;
    }

    public void setProductoId(String productoId) {
        this.productoId = productoId;
    }

    public Integer getCantidad() {
        return cantidad;
    }

    public void setCantidad(Integer cantidad) {
        this.cantidad = cantidad;
    }
}
