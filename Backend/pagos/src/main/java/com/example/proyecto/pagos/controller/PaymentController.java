package com.example.proyecto.pagos.controller;

import com.example.proyecto.pagos.dto.PagosRequest;
import com.example.proyecto.pagos.service.PagosService;
import com.stripe.model.PaymentIntent;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/pagos")
public class PaymentController {

    private final PagosService pagosService;

    public PaymentController(PagosService pagosService) {
        this.pagosService = pagosService;
    }

    @PostMapping("/crear-intencion")
    public ResponseEntity<?> crearIntencionPago(@RequestBody PagosRequest request) {
        try {
            PaymentIntent intent = pagosService.createPaymentIntent(request);
            return ResponseEntity.ok(
                    java.util.Map.of("clientSecret", intent.getClientSecret())
            );
        } catch (Exception e) {
            return ResponseEntity.status(500).body(e.getMessage());
        }
    }

    @GetMapping("/{id}")
    public ResponseEntity<?> obtenerIntent(@PathVariable String id) {
        try {
            PaymentIntent intent = pagosService.getPaymentIntent(id);
            return ResponseEntity.ok(intent);
        } catch (Exception e) {
            return ResponseEntity.status(404).body("Intent no encontrado");
        }
    }
}
