package com.example.proyecto.catalogo.repository;

import com.example.proyecto.catalogo.model.Producto;
import org.springframework.data.mongodb.repository.MongoRepository;

public interface ProductoRepository extends MongoRepository<Producto, String> {
}
