package com.example.proyecto.catalogo.service;

import java.util.List;
import com.example.proyecto.catalogo.model.Producto;

public interface ProductoService {
    List<Producto> listar();
    Producto obtenerPorId(String id);
    Producto guardar(Producto producto);
    Producto actualizar(String id, Producto producto);
    void eliminar(String id);
}
