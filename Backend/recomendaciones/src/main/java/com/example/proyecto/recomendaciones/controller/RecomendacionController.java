package com.example.proyecto.recomendaciones.controller;

import com.example.proyecto.recomendaciones.model.Recomendacion;
import com.example.proyecto.recomendaciones.service.RecomendacionService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/recomendaciones")
public class RecomendacionController {

    @Autowired
    private RecomendacionService service;

    @GetMapping("/all")
    public List<Recomendacion> listar() {
        return service.listar();
    }

    @GetMapping("/{id}")
    public Recomendacion obtener(@PathVariable String id) {
        return service.obtenerPorId(id);
    }

    @PostMapping
    public Recomendacion guardar(@RequestBody Recomendacion recomendacion) {
        return service.guardar(recomendacion);
    }

    @PutMapping("/{id}")
    public Recomendacion actualizar(@PathVariable String id, @RequestBody Recomendacion recomendacion) {
        return service.actualizar(id, recomendacion);
    }

    @DeleteMapping("/{id}")
    public void eliminar(@PathVariable String id) {
        service.eliminar(id);
    }
}
