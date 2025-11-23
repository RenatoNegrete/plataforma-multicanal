package org.example.demo3.Repositorio;

import jakarta.enterprise.context.ApplicationScoped;
import jakarta.persistence.EntityManager;
import jakarta.persistence.PersistenceContext;
import org.example.demo3.Modelo.Usuario;

import java.util.List;
@ApplicationScoped
public class UsuarioRepositorio {

    @PersistenceContext(unitName = "MyPU")
    private EntityManager em;

    // Crear usuario
    public void crear(Usuario usuario) {
        em.persist(usuario);
    }

    // Listar todos
    public List<Usuario> listarTodos() {
        return em.createQuery("SELECT u FROM Usuario u", Usuario.class)
                .getResultList();
    }

    // Buscar por ID
    public Usuario buscarPorId(Long id) {
        return em.find(Usuario.class, id);
    }

    // Actualizar
    public void actualizar(Usuario usuario) {
        em.merge(usuario);
    }

    // Eliminar
    public void eliminar(Long id) {
        Usuario u = em.find(Usuario.class, id);
        if (u != null) {
            em.remove(u);
        }
    }

}
