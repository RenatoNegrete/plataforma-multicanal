package com.example.proyecto.proveedores.service;

import org.springframework.stereotype.Service;
import org.springframework.web.reactive.function.client.WebClient;

import com.example.proyecto.proveedores.model.ProductoProveedor;
import com.example.proyecto.proveedores.model.Proveedor;
import com.example.proyecto.proveedores.repository.ProveedorRepository;

import reactor.core.publisher.Mono;

import java.util.List;

@Service
public class ProveedorService {

    private final ProveedorRepository proveedorRepository;
    private final WebClient webClient;

    public ProveedorService(ProveedorRepository proveedorRepository) {
        this.proveedorRepository = proveedorRepository;
        this.webClient = WebClient.builder().build();
    }

    public Proveedor guardarProveedor(Proveedor proveedor) {
        return proveedorRepository.save(proveedor);
    }

    public List<Proveedor> listarProveedores() {
        return proveedorRepository.findAll();
    }

    public Proveedor obtenerProveedor(Long id) {
        return proveedorRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("Proveedor no encontrado"));
    }

    public void eliminarProveedor(Long id) {
        proveedorRepository.deleteById(id);
    }

    public Proveedor actualizarProveedor(Long id, Proveedor proveedorActualizado) {
        Proveedor existente = proveedorRepository.findById(id)
                .orElseThrow(() -> new RuntimeException("Proveedor no encontrado"));

        existente.setNombre(proveedorActualizado.getNombre());
        existente.setEmail(proveedorActualizado.getEmail());
        existente.setTelefono(proveedorActualizado.getTelefono());
        existente.setDireccion(proveedorActualizado.getDireccion());
        existente.setUrl(proveedorActualizado.getUrl());

        return proveedorRepository.save(existente);
    }

    public Mono<List<ProductoProveedor>> consultarProveedor(String url) {
        return webClient.get()
                .uri(url)
                .retrieve()
                .bodyToFlux(ProductoProveedor.class)
                .collectList();
    }
}
