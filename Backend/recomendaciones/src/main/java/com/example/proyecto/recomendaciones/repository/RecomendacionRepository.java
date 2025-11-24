package com.example.proyecto.recomendaciones.repository;

import org.springframework.data.mongodb.repository.MongoRepository;
import com.example.proyecto.recomendaciones.model.Recomendacion;

public interface RecomendacionRepository extends MongoRepository<Recomendacion, String> {
}
