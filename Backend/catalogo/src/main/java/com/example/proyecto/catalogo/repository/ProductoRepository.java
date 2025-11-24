package com.example.proyecto.catalogo.repository;

import com.example.proyecto.catalogo.model.Producto;

import java.util.List;

import org.springframework.data.mongodb.repository.MongoRepository;

public interface ProductoRepository extends MongoRepository<Producto, String> {
    List<Producto> findByProveedorId(String proveedorId);
    List<Producto> findByCategoria(String categoria);
}
