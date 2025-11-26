package com.example.proyecto.ordenes.controller;

import com.example.proyecto.ordenes.dto.OrdenRequest;
import com.example.proyecto.ordenes.model.Orden;
import com.example.proyecto.ordenes.service.OrdenService;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;

import java.util.List;

@RestController
@RequestMapping("/api/ordenes")
public class OrdenController {

    private final OrdenService service;

    public OrdenController(OrdenService service) {
        this.service = service;
    }

    @PostMapping
    public Orden crear(@RequestBody OrdenRequest orden) {
        return service.crearOrden(orden);
    }

    @GetMapping
    public List<Orden> listar() {
        return service.listarOrdenes();
    }

    @GetMapping("/{id}")
    public Orden obtener(@PathVariable String id) {
        return service.obtenerPorId(id);
    }

    @GetMapping("/cliente/{clienteId}")
    public List<Orden> listarPorCliente(@PathVariable Long clienteId) {
        return service.listarPorCliente(clienteId);
    }
}
