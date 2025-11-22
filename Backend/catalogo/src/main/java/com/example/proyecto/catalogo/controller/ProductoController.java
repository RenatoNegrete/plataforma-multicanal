package com.example.proyecto.catalogo.controller;

import com.example.proyecto.catalogo.model.Producto;
import com.example.proyecto.catalogo.repository.ProductoRepository;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/productos")
public class ProductoController {

    private final ProductoRepository repository;

    public ProductoController(ProductoRepository repository) {
        this.repository = repository;
    }

    @GetMapping("/all")
    public List<Producto> listar() {
        return repository.findAll();
    }

    @SuppressWarnings("null")
    @PostMapping
    public Producto guardar(@RequestBody Producto producto) {
        return repository.save(producto);
    }
}
