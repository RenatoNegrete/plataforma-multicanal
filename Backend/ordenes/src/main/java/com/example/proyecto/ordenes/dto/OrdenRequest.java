package com.example.proyecto.ordenes.dto;

import java.util.List;

public class OrdenRequest {

    private Long clienteId;
    private List<ItemOrden> productos;
    private String clienteMail;

    public OrdenRequest() {}

    public OrdenRequest(Long clienteId, List<ItemOrden> productos) {
        this.clienteId = clienteId;
        this.productos = productos;
    }

    public Long getClienteId() {
        return clienteId;
    }

    public void setClienteId(Long clienteId) {
        this.clienteId = clienteId;
    }

    public List<ItemOrden> getProductos() {
        return productos;
    }

    public void setProductos(List<ItemOrden> productos) {
        this.productos = productos;
    }

    public String getClienteMail() {
        return clienteMail;
    }

    public void setClienteMail(String clienteMail) {
        this.clienteMail = clienteMail;
    }

}
