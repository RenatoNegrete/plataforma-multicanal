package org.example.demo3.servicio;


import jakarta.enterprise.context.ApplicationScoped;
import jakarta.inject.Inject;
import jakarta.transaction.Transactional;
import org.example.demo3.Modelo.Usuario;
import org.example.demo3.Repositorio.UsuarioRepositorio;

@ApplicationScoped
public class UsuarioServicio {

    @Inject
    private UsuarioRepositorio repository;

    @Transactional
    public void crearUsuario(Usuario usuario) {
        repository.crear(usuario);
    }

    @Transactional
    // Buscar usuario por ID
    public Usuario buscarUsuarioPorId(Long id) {
        return repository.buscarPorId(id);
    }

    @Transactional
    public void actualizarUsuario(Usuario usuario) {
        repository.actualizar(usuario);
    }
    @Transactional
    public void eliminarUsuario(Long id) {
        repository.eliminar(id);
    }

}
