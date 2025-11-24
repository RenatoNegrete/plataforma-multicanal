package org.example.demo3.resource;

import java.util.List;

import org.example.demo3.dto.RegistroRequest;
import org.example.demo3.entity.Usuario;
import org.example.demo3.service.UsuarioService;

import jakarta.inject.Inject;
import jakarta.validation.Valid;
import jakarta.ws.rs.*;
import jakarta.ws.rs.core.Context;
import jakarta.ws.rs.core.MediaType;
import jakarta.ws.rs.core.Response;
import jakarta.ws.rs.core.SecurityContext;

@Path("/usuarios")
@Produces(MediaType.APPLICATION_JSON)
@Consumes(MediaType.APPLICATION_JSON)
public class UsuarioResource {

    @Inject
    private UsuarioService usuarioService;

    @POST
    @Path("/register")
    public Response registrar(@Valid RegistroRequest request) {
        Usuario usuario = usuarioService.registrarUsuario(request);
        return Response.ok(usuario).build();
    }

    @GET
    public List<Usuario> listar() {
        return usuarioService.listarUsuarios();
    }

    @GET
    @Path("/{id}")
    public Usuario obtener(@PathParam("id") Long id) {
        return usuarioService.obtenerUsuario(id);
    }

    @PUT
    @Path("/{id}")
    public Response actualizar(@PathParam("id") Long id, Usuario request) {
        Usuario u = usuarioService.actualizarUsuario(id, request);
        return Response.ok(u).build();
    }

    @DELETE
    @Path("/{id}")
    public Response eliminar(@PathParam("id") Long id) {
        usuarioService.eliminarUsuario(id);
        return Response.noContent().build();
    }

    @GET
    @Path("/health")
    public Response health() {
        return Response.ok("{\"status\":\"UP\"}").build();
    }

}
