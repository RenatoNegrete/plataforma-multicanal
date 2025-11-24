package com.example.proyecto.ordenes.repository;

import com.example.proyecto.ordenes.model.Orden;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface OrdenRepository extends MongoRepository<Orden, String> {
    List<Orden> findByClienteId(Long clienteId);
}
