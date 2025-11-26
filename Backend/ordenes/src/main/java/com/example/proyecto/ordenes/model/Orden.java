package com.example.proyecto.ordenes.model;

import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.mapping.Document;

import com.example.proyecto.ordenes.dto.ItemOrden;

import java.time.LocalDate;
import java.util.List;

@Document(collection = "ordenes")
public class Orden {

    @Id
    private String id;  // MongoDB usa String (ObjectId)

    private Long clienteId;
    private LocalDate fecha;
    private Double total;
    private List<ItemOrden> productos;

    // Getters y setters
    public String getId() { return id; }
    public void setId(String id) { this.id = id; }

    public Long getClienteId() { return clienteId; }
    public void setClienteId(Long clienteId) { this.clienteId = clienteId; }

    public LocalDate getFecha() { return fecha; }
    public void setFecha(LocalDate fecha) { this.fecha = fecha; }

    public Double getTotal() { return total; }
    public void setTotal(Double total) { this.total = total; }

    public List<ItemOrden> getProductos() { return productos; }
    public void setProductos(List<ItemOrden> productos) { this.productos = productos; }
}
