package com.example.proyecto.catalogo.controller;

import com.example.proyecto.catalogo.model.Producto;
import com.example.proyecto.catalogo.service.ProductoService;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/catalogo")
public class ProductoController {

    @Autowired
    private ProductoService service;

    @GetMapping("/all")
    public ResponseEntity<List<Producto>> listar() {
        return ResponseEntity.ok(service.listar());
    }

    @GetMapping("/{id}")
    public ResponseEntity<Producto> obtener(@PathVariable String id) {
        Producto p = service.obtenerPorId(id);
        return p == null ? ResponseEntity.notFound().build() : ResponseEntity.ok(p);
    }

    @PostMapping
    public ResponseEntity<Producto> guardar(@RequestBody Producto producto) {
        Producto nuevo = service.guardar(producto);
        return ResponseEntity.status(HttpStatus.CREATED).body(nuevo);
    }

    @PutMapping("/{id}")
    public ResponseEntity<Producto> actualizar(@PathVariable String id, @RequestBody Producto producto) {
        Producto actualizado = service.actualizar(id, producto);
        return actualizado == null ? ResponseEntity.notFound().build() : ResponseEntity.ok(actualizado);
    }

    @DeleteMapping("/{id}")
    public void eliminar(@PathVariable String id) {
        service.eliminar(id);
    }

    @GetMapping("/proveedor/{proveedorId}")
    public ResponseEntity<List<Producto>> listarPorProveedor(@PathVariable String proveedorId) {
        List<Producto> productos = service.listarPorProveedor(proveedorId);
        
        if (productos.isEmpty()) {
            return ResponseEntity.noContent().build();
        }
        
        return ResponseEntity.ok(productos);
    }

    @GetMapping("/categoria/{categoria}")
    public ResponseEntity<List<Producto>> listarPorCategoria(@PathVariable String categoria) {
        // Normalizamos para evitar errores de mayúsculas
        categoria = categoria.toLowerCase();
        // Validamos la categoría
        if (!categoria.equals("fisico") && 
            !categoria.equals("servicio") && 
            !categoria.equals("suscripcion")) {
            return ResponseEntity.badRequest().body(null);
        }

        List<Producto> productos = service.listarPorCategoria(categoria);

        if (productos.isEmpty()) {
            return ResponseEntity.noContent().build();
        }
        return ResponseEntity.ok(productos);
    } 
}
