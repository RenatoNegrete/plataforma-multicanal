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
import java.util.List;

@ApplicationScoped
public class UsuarioService {

    @PersistenceContext(unitName = "usuariosPU")
    private EntityManager em;

    @Transactional
    public Usuario registrarUsuario(RegistroRequest request) {
        
        try {
            em.createNamedQuery("Usuario.findByEmail", Usuario.class)
              .setParameter("email", request.getEmail())
              .getSingleResult();
            
            throw new WebApplicationException(
                "El email ya está registrado", 
                Response.Status.CONFLICT
            );
        } catch (NoResultException e) {

        }

        Usuario usuario = new Usuario();
        usuario.setEmail(request.getEmail());
        usuario.setNombre(request.getNombre());
        usuario.setTelefono(request.getTelefono());
        usuario.setDireccion(request.getDireccion());
        usuario.setFechaRegistro(LocalDateTime.now());

        em.persist(usuario);

        return usuario;
    }

    public Usuario obtenerUsuario(Long id) {
        Usuario usuario = em.find(Usuario.class, id);
        if (usuario == null) {
            throw new WebApplicationException("Usuario no encontrado", Response.Status.NOT_FOUND);
        }
        return usuario;
    }

    public List<Usuario> listarUsuarios() {
        return em.createQuery("SELECT u FROM Usuario u", Usuario.class)
                 .getResultList();
    }

    @Transactional
    public Usuario actualizarUsuario(Long id, Usuario actualizar) {
        Usuario usuario = obtenerUsuario(id);

        usuario.setNombre(actualizar.getNombre());
        usuario.setTelefono(actualizar.getTelefono());
        usuario.setDireccion(actualizar.getDireccion());

        em.merge(usuario);
        return usuario;
    }

    @Transactional
    public void eliminarUsuario(Long id) {
        Usuario usuario = obtenerUsuario(id);
        em.remove(usuario);
    }

    public Usuario obtenerUsuarioPorEmail(String email) {
        try {
            return em.createNamedQuery("Usuario.findByEmail", Usuario.class)
                    .setParameter("email", email)
                    .getSingleResult();
        } catch (NoResultException e) {
            throw new WebApplicationException(
                "Usuario no encontrado",
                Response.Status.NOT_FOUND
            );
        }
    }

    public Long contarUsuarios() {
        return em.createQuery("SELECT COUNT(u) FROM Usuario u", Long.class)
                .getSingleResult();
    }
}
