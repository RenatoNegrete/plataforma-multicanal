package com.example.proyecto.ordenes.service;

import org.springframework.stereotype.Service;
import org.springframework.web.reactive.function.client.WebClient;

import com.example.proyecto.ordenes.dto.ProductoDto;

@Service
public class CatalogoClient {

    private final WebClient webClient;
    
    public CatalogoClient(WebClient.Builder webClientBuilder) {
        this.webClient = webClientBuilder
            .baseUrl("http://catalogo-service:8080")
            .build();
    }

    public ProductoDto obtenerProducto(String productoId) {
        try {
            return webClient.get()
                .uri("/api/catalogo/{id}", productoId)
                .retrieve()
                .bodyToMono(ProductoDto.class)
                .block();
        } catch (Exception e) {
            throw new RuntimeException(
                "Error al obtener producto: " + productoId, e);
        }
    }

    public void actualizarStock(String productoId, int cantidad, ProductoDto request) {
        request.setStock(request.getStock() - cantidad);
        try {
            webClient.put()
                .uri("/api/catalogo/{id}", productoId)
                .bodyValue(request)
                .retrieve()
                .toBodilessEntity()
                .block();
        } catch (Exception e) {
            throw new RuntimeException(
                "Error al actualizar stock del producto: " + productoId, e);
        }
    }

    public boolean verificarDisponibilidad(String productoId, Integer cantidad) {
        ProductoDto producto = obtenerProducto(productoId);
        return producto != null && 
               producto.getStock() >= cantidad;
    }
}
