package org.example.demo3.service;

import org.example.demo3.dto.RegistroRequest;
import org.example.demo3.entity.Usuario;
import com.google.firebase.auth.FirebaseAuth;
import com.google.firebase.auth.FirebaseAuthException;
import com.google.firebase.auth.UserRecord;

import jakarta.enterprise.context.ApplicationScoped;
import jakarta.inject.Inject;
import jakarta.persistence.EntityManager;
import jakarta.persistence.NoResultException;
import jakarta.persistence.PersistenceContext;
import jakarta.transaction.Transactional;
import jakarta.ws.rs.WebApplicationException;
import jakarta.ws.rs.core.Response;
import java.time.LocalDateTime;

@ApplicationScoped
public class UsuarioService {

    @PersistenceContext(unitName = "usuariosPU")
    private EntityManager em;

    @Inject
    private FirebaseAuth firebaseAuth;

    @Transactional
    public Usuario registrarUsuario(RegistroRequest request) {
        
        // 1. Validar que el email no exista
        try {
            em.createNamedQuery("Usuario.findByEmail", Usuario.class)
              .setParameter("email", request.getEmail())
              .getSingleResult();
            
            throw new WebApplicationException(
                "El email ya está registrado", 
                Response.Status.CONFLICT
            );
        } catch (NoResultException e) {
            // Email no existe, continuar
        }

        String firebaseUid = null;

        try {
            // 2. Crear usuario en Firebase
            UserRecord.CreateRequest firebaseRequest = 
                new UserRecord.CreateRequest()
                    .setEmail(request.getEmail())
                    .setPassword(request.getPassword())
                    .setEmailVerified(false);

            UserRecord firebaseUser = firebaseAuth.createUser(firebaseRequest);
            firebaseUid = firebaseUser.getUid();

            // 3. Crear usuario en BD
            Usuario usuario = new Usuario();
            usuario.setFirebaseUid(firebaseUid);
            usuario.setEmail(request.getEmail());
            usuario.setNombre(request.getNombre());
            usuario.setTelefono(request.getTelefono());
            usuario.setDireccion(request.getDireccion());
            usuario.setFechaRegistro(LocalDateTime.now());

            em.persist(usuario);

            return usuario;

        } catch (FirebaseAuthException e) {
            // Rollback: eliminar de Firebase si falla BD
            if (firebaseUid != null) {
                try {
                    firebaseAuth.deleteUser(firebaseUid);
                } catch (FirebaseAuthException ex) {
                    // Log error
                }
            }

            if (e.getAuthErrorCode().name().equals("EMAIL_ALREADY_EXISTS")) {
                throw new WebApplicationException(
                    "El email ya está registrado en Firebase", 
                    Response.Status.CONFLICT
                );
            }

            throw new WebApplicationException(
                "Error al crear usuario: " + e.getMessage(), 
                Response.Status.INTERNAL_SERVER_ERROR
            );
        }
    }

    public Usuario obtenerPorFirebaseUid(String firebaseUid) {
        try {
            return em.createNamedQuery("Usuario.findByFirebaseUid", Usuario.class)
                     .setParameter("firebaseUid", firebaseUid)
                     .getSingleResult();
        } catch (NoResultException e) {
            throw new WebApplicationException(
                "Usuario no encontrado", 
                Response.Status.NOT_FOUND
            );
        }
    }

    @Transactional
    public Usuario actualizarPerfil(String firebaseUid, Usuario datosActualizados) {
        Usuario usuario = obtenerPorFirebaseUid(firebaseUid);

        usuario.setNombre(datosActualizados.getNombre());
        usuario.setTelefono(datosActualizados.getTelefono());
        usuario.setDireccion(datosActualizados.getDireccion());

        return em.merge(usuario);
    }
}
