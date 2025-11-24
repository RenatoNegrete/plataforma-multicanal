package com.example.proyecto.pagos.service;

import com.example.proyecto.pagos.dto.PagosRequest;
import com.stripe.Stripe;
import com.stripe.model.PaymentIntent;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import java.util.HashMap;
import java.util.Map;

@Service
public class PagosService {

    @Value("${stripe.secret-key}")
    private String secretKey;

    public PaymentIntent createPaymentIntent(PagosRequest request) throws Exception {
        // Configurar API key
        Stripe.apiKey = secretKey;

        // Crear parámetros
        Map<String, Object> params = new HashMap<>();
        params.put("amount", request.getAmount());  // SIN DIVIDIR
        params.put("currency", request.getCurrency());
        params.put("description", request.getDescription());
        params.put("automatic_payment_methods", Map.of("enabled", true));


        // Crear Intent
        return PaymentIntent.create(params);
    }

    public PaymentIntent getPaymentIntent(String id) throws Exception {
        Stripe.apiKey = secretKey;
        return PaymentIntent.retrieve(id);
    }
}
