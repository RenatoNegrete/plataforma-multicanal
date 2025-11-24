package com.example.proyecto.catalogo.model;

import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.mapping.Document;

@Document(collection = "productos")
public class Producto {

    @Id
    private String id;

    private String nombre;
    private double precio;

    // Nuevos atributos
    private String imagen;       // URL de imagen
    private String descripcion;  // Descripción del producto
    private String categoria;    // Categoría: ropa, electrónica...
    private int stock;           // Cantidad disponible

    public Producto() {}

    public Producto(String id, String nombre, double precio, String imagen, 
                    String descripcion, String categoria, int stock) {
        this.id = id;
        this.nombre = nombre;
        this.precio = precio;
        this.imagen = imagen;
        this.descripcion = descripcion;
        this.categoria = categoria;
        this.stock = stock;
    }

    // Getters y setters
    public String getId() { return id; }
    public void setId(String id) { this.id = id; }

    public String getNombre() { return nombre; }
    public void setNombre(String nombre) { this.nombre = nombre; }

    public double getPrecio() { return precio; }
    public void setPrecio(double precio) { this.precio = precio; }

    public String getImagen() { return imagen; }
    public void setImagen(String imagen) { this.imagen = imagen; }

    public String getDescripcion() { return descripcion; }
    public void setDescripcion(String descripcion) { this.descripcion = descripcion; }

    public String getCategoria() { return categoria; }
    public void setCategoria(String categoria) { this.categoria = categoria; }

    public int getStock() { return stock; }
    public void setStock(int stock) { this.stock = stock; }
}
