package com.example.proyecto.ordenes.service;

import org.springframework.stereotype.Service;
import org.springframework.web.reactive.function.client.WebClient;

import com.example.proyecto.ordenes.dto.CotizacionEnvioRequest;

@Service
public class EnviosClient {

    private final WebClient webClient;

    public EnviosClient(WebClient.Builder webClientBuilder) {
        this.webClient = webClientBuilder
            .baseUrl("http://envios-service:8000")
            .build();
    }

    public void cotizarEnvio(CotizacionEnvioRequest request) {
        try {
            webClient.post()
                .uri("/api/envios/cotizar")
                .bodyValue(request)
                .retrieve()
                .toBodilessEntity()
                .block();
        } catch (Exception e) {
            throw new RuntimeException(
                "Error al cotizar envío", e);
        }
    }

}
