package com.example.proyecto.proveedores.controller;

import com.example.proyecto.proveedores.model.Proveedor;
import com.example.proyecto.proveedores.model.Producto;
import com.example.proyecto.proveedores.service.ProveedorService;

import reactor.core.publisher.Flux;
import reactor.core.publisher.Mono;

import org.springframework.web.bind.annotation.*;
import org.springframework.web.reactive.function.client.WebClient;

import java.util.List;
import java.util.Map;

@RestController
@RequestMapping("/api/proveedores")
public class ProveedorController {

    private final ProveedorService proveedorService;
    private final WebClient webClient;

    public ProveedorController(ProveedorService proveedorService, WebClient webClient) {
        this.proveedorService = proveedorService;
        this.webClient = webClient;
    }

    @PostMapping
    public Proveedor crearProveedor(@RequestBody Proveedor proveedor) {
        return proveedorService.guardarProveedor(proveedor);
    }

    @GetMapping
    public List<Proveedor> obtenerTodos() {
        return proveedorService.listarProveedores();
    }

    @GetMapping("/{id}")
    public Proveedor obtenerPorId(@PathVariable Long id) {
        return proveedorService.obtenerProveedor(id);
    }

    @DeleteMapping("/{id}")
    public void eliminar(@PathVariable Long id) {
        proveedorService.eliminarProveedor(id);
    }

    @PutMapping("/{id}")
    public Proveedor actualizarProveedor(
            @PathVariable Long id,
            @RequestBody Proveedor proveedor) {
        return proveedorService.actualizarProveedor(id, proveedor);
    }

    @GetMapping("/{id}/productos")
    public Mono<List<Object>> obtenerProductosProveedor(@PathVariable Long id) {

        Proveedor proveedor = proveedorService.obtenerProveedor(id);
        String endpoint = proveedor.getUrl() + "/api/Provider";

        return proveedorService.consultarProveedor(endpoint)
            .flatMapMany(Flux::fromIterable)
            .flatMap(productoProv -> {
                Producto producto = new Producto();
                producto.setProveedorId(String.valueOf(id));
                producto.setNombre(productoProv.getNombre());
                producto.setPrecio(productoProv.getPrecio());
                producto.setImagen(productoProv.getImagen());
                producto.setDescripcion(productoProv.getDescripcion());
                producto.setCategoria(productoProv.getCategoria());
                producto.setStock(productoProv.getStock());

                return webClient.post()
                    .uri("http://catalogo-service:8080/api/catalogo")
                    .bodyValue(producto)
                    .retrieve()
                    .bodyToMono(Producto.class)
                    .cast(Object.class) // Convierte a Object
                    .onErrorResume(e -> 
                        Mono.just(Map.of(
                            "producto", productoProv.getNombre(),
                            "error", "No se pudo registrar en catálogo",
                            "detalle", e.getMessage()
                        ))
                    );
            })
            .cast(Object.class) // Asegura que todo sea Object
            .collectList()
            .onErrorResume(e ->
                Mono.just(List.of(Map.of(
                    "error", "No se pudo consultar el proveedor", 
                    "detalle", e.getMessage()
                )))
            );
    }
}
