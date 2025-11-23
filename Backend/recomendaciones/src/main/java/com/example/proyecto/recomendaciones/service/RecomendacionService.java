package com.example.proyecto.recomendaciones.service;

import java.util.List;
import com.example.proyecto.recomendaciones.model.Recomendacion;

public interface RecomendacionService {
    List<Recomendacion> listar();
    Recomendacion obtenerPorId(String id);
    Recomendacion guardar(Recomendacion recomendacion);
    Recomendacion actualizar(String id, Recomendacion recomendacion);
    void eliminar(String id);
}
