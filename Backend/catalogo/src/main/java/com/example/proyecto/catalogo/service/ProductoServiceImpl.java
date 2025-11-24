package com.example.proyecto.catalogo.service;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.example.proyecto.catalogo.model.Producto;
import com.example.proyecto.catalogo.repository.ProductoRepository;

@Service
public class ProductoServiceImpl implements ProductoService {

    @Autowired
    private ProductoRepository repository;

    @Override
    public List<Producto> listar() {
        return repository.findAll();
    }

    @Override
    public Producto obtenerPorId(String id) {
        return repository.findById(id).orElse(null);
    }

    @Override
    public Producto guardar(Producto producto) {
        return repository.save(producto);
    }

    @Override
    public Producto actualizar(String id, Producto producto) {
        Producto existente = repository.findById(id).orElse(null);
        if (existente == null) return null;

        existente.setNombre(producto.getNombre());
        existente.setPrecio(producto.getPrecio());
        existente.setImagen(producto.getImagen());
        existente.setDescripcion(producto.getDescripcion());
        existente.setCategoria(producto.getCategoria());
        existente.setStock(producto.getStock());

        return repository.save(existente);
    }

    @Override
    public void eliminar(String id) {
        repository.deleteById(id);
    }
}
