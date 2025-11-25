package com.example.proyecto.ordenes.service;

import org.springframework.stereotype.Service;
import org.springframework.web.reactive.function.client.WebClient;

import com.example.proyecto.ordenes.dto.IntencionPagoRequest;

@Service
public class PagosClient {

    private final WebClient webClient;

    public PagosClient(WebClient.Builder webClientBuilder) {
        this.webClient = webClientBuilder
            .baseUrl("http://pagos-service:8080")
            .build();
    }

    public String crearIntencionPago(IntencionPagoRequest request) {
        try {
            return webClient.post()
                .uri("/api/pagos/crear-intencion")
                .bodyValue(request)
                .retrieve()
                .bodyToMono(String.class)
                .block();
        } catch (Exception e) {
            throw new RuntimeException(
                "Error al crear intención de pago", e);
        }
    }
}
