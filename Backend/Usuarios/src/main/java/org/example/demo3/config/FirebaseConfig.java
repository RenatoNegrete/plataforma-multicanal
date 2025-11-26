package org.example.demo3.config;

import com.google.auth.oauth2.GoogleCredentials;
import com.google.firebase.FirebaseApp;
import com.google.firebase.FirebaseOptions;
import com.google.firebase.auth.FirebaseAuth;

import jakarta.annotation.PostConstruct;
import jakarta.enterprise.context.ApplicationScoped;
import jakarta.enterprise.context.Dependent;
import jakarta.enterprise.inject.Produces;
import java.io.FileInputStream;
import java.io.IOException;

@ApplicationScoped
public class FirebaseConfig {

    private FirebaseAuth firebaseAuth;

    @PostConstruct
    public void init() {
        try {
            // Leer ruta desde variable de entorno
            String credentialsPath = System.getenv("FIREBASE_CREDENTIALS_PATH");
            
            // Si no existe la variable, usar ruta por defecto (para desarrollo local sin Docker)
            if (credentialsPath == null || credentialsPath.isEmpty()) {
                credentialsPath = "firebase-credentials.json";
            }

            System.out.println("Intentando cargar Firebase desde: " + credentialsPath);

            FileInputStream serviceAccount = new FileInputStream(credentialsPath);

            FirebaseOptions options = FirebaseOptions.builder()
                .setCredentials(GoogleCredentials.fromStream(serviceAccount))
                .build();

            if (FirebaseApp.getApps().isEmpty()) {
                FirebaseApp.initializeApp(options);
                System.out.println("Firebase inicializado correctamente");
            }

            this.firebaseAuth = FirebaseAuth.getInstance();

        } catch (IOException e) {
            System.err.println("Error inicializando Firebase: " + e.getMessage());
            throw new RuntimeException("Error inicializando Firebase", e);
        }
    }

    @Produces
    @Dependent
    public FirebaseAuth getFirebaseAuth() {
        return firebaseAuth;
    }
}
