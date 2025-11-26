package com.example.proyecto.recomendaciones.service;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.example.proyecto.recomendaciones.model.Recomendacion;
import com.example.proyecto.recomendaciones.repository.RecomendacionRepository;
import com.example.proyecto.recomendaciones.service.RecomendacionService;

@Service
public class RecomendacionServiceImpl implements RecomendacionService {

    @Autowired
    private RecomendacionRepository repository;

    @Override
    public List<Recomendacion> listar() {
        return repository.findAll();
    }

    @Override
    public Recomendacion obtenerPorId(String id) {
        return repository.findById(id).orElse(null);
    }

    @Override
    public Recomendacion guardar(Recomendacion recomendacion) {
        return repository.save(recomendacion);
    }

    @Override
    public Recomendacion actualizar(String id, Recomendacion recomendacion) {
        Recomendacion existente = repository.findById(id).orElse(null);
        if (existente == null) return null;

        existente.setProductoId(recomendacion.getProductoId());
        existente.setMensaje(recomendacion.getMensaje());
        existente.setPrioridad(recomendacion.getPrioridad());

        return repository.save(existente);
    }

    @Override
    public void eliminar(String id) {
        repository.deleteById(id);
    }
}
