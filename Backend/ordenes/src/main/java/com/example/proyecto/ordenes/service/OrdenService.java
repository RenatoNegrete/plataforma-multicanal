package com.example.proyecto.ordenes.service;

import com.example.proyecto.ordenes.model.Orden;
import com.example.proyecto.ordenes.repository.OrdenRepository;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class OrdenService {

    private final OrdenRepository repository;

    public OrdenService(OrdenRepository repository) {
        this.repository = repository;
    }

    public Orden crearOrden(Orden orden) {
        return repository.save(orden);
    }

    public List<Orden> listarOrdenes() {
        return repository.findAll();
    }

    public Orden obtenerPorId(String id) {
        return repository.findById(id).orElse(null);
    }

    public List<Orden> listarPorCliente(Long clienteId) {
        return repository.findByClienteId(clienteId);
    }
}
