package com.example.api_gateway.api_gateway.controller;

import java.util.HashMap;
import java.util.Map;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.http.HttpEntity;
import org.springframework.http.HttpMethod;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.client.RestTemplate;

@RestController
@RequestMapping("/auth")
public class AuthController {

    @Value("${firebase.apiKey}")
    private String firebaseApiKey;

    private final RestTemplate restTemplate = new RestTemplate();

    @PostMapping("/register")
    public ResponseEntity<?> register(@RequestBody Map<String, Object> request) {

        String email = (String) request.get("email");
        String password = (String) request.get("password");

        // 1. Registrar en Firebase
        String firebaseUrl =
                "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" + firebaseApiKey;

        Map<String, Object> body = new HashMap<>();
        body.put("email", email);
        body.put("password", password);
        body.put("returnSecureToken", true);

        ResponseEntity<Map<String, Object>> firebaseResponse =
                restTemplate.exchange(
                firebaseUrl,
                HttpMethod.POST,
                new HttpEntity<>(body),
                new ParameterizedTypeReference<Map<String, Object>>() {}
        );

        if (!firebaseResponse.getStatusCode().is2xxSuccessful()) {
            return ResponseEntity.status(500).body("Error registrando usuario en Firebase");
        }

        Map<String, Object> firebaseData = firebaseResponse.getBody();
        String uid = (String) firebaseData.get("localId");

        // 2. Enviar datos extra al microservicio de usuarios
        Map<String, Object> extraData = new HashMap<>();
        extraData.put("email", request.get("email"));
        extraData.put("nombre", request.get("nombre"));
        extraData.put("telefono", request.get("telefono"));
        extraData.put("direccion", request.get("direccion"));

        restTemplate.postForEntity(
            "http://localhost:8085/usuarios-service/api/usuarios/register",
            extraData,
            Void.class
        );

        return ResponseEntity.ok(firebaseData);
    }

    @PostMapping("/login")
    public ResponseEntity<?> login(@RequestBody Map<String, Object> request) {

        String email = (String) request.get("email");
        String password = (String) request.get("password");

        String loginUrl =
            "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" + firebaseApiKey;

        Map<String, Object> body = new HashMap<>();
        body.put("email", email);
        body.put("password", password);
        body.put("returnSecureToken", true);

        ResponseEntity<Map<String, Object>> firebaseResponse =
                restTemplate.exchange(
                loginUrl,
                HttpMethod.POST,
                new HttpEntity<>(body),
                new ParameterizedTypeReference<Map<String, Object>>() {}
        );

        if (!firebaseResponse.getStatusCode().is2xxSuccessful()) {
            return ResponseEntity.status(401).body("Credenciales inválidas");
        }

        return ResponseEntity.ok(firebaseResponse.getBody());
    }

}
